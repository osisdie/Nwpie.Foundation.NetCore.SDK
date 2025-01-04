using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Notification.Enums;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Notification.Models;
using Nwpie.Foundation.Http.Common.Utilities;
using Nwpie.Foundation.Notification.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Notification.Lambda.Service.ServiceCore.Notification.Services
{
    public class NotificationServiceCore : CObject
    {
        public NotificationServiceCore(NotifySend_RequestModel requestDto)
            : base()
        {
            m_RequestDto = requestDto;
            m_SdkEnv = ServiceContext.SdkEnv; // Use global env

            Initialize();
        }

        protected void Initialize()
        {
            if (false == IsInitialized)
            {
                lock (InitializationLock)
                {
                    if (false == IsInitialized)
                    {
                        ServiceHostUrls = ServiceContext.Configuration
                            .GetKeyValueListInSection($"sdk.base.host_url", new Regex("^[a-z]+$"))
                            ?.ToDictionary(x => x.Key, x => x.Value);
                        AdminEnvMap = ServiceContext.Configuration
                            .GetKeyValueListInSection($"sdk.api.list", new Regex("^Nwpie.admin.[a-z]+$"))
                            ?.ToDictionary(x => x.Key, x => x.Value);

                        TryLoadRemoteConfig().ConfigureAwait(false).GetAwaiter().GetResult();
                        IsInitialized = true;
                    }
                }
            }
        }

        public async Task<IServiceResponse<string>> SendAsync()
        {
            m_SmtpCfg = SmtpCfgMap[m_SdkEnv] ?? SmtpCfgMap.First().Value;
            m_LineCfg = LineCfgMap[m_SdkEnv] ?? LineCfgMap.First().Value;
            m_SlackCfg = SlackCfgMap[m_SdkEnv] ?? SlackCfgMap.First().Value;

            var func = GetSender(m_RequestDto.Kind);
            var sentStatus = await func(m_RequestDto);
            if (sentStatus.IsSuccess && m_RequestDto.AndKinds?.Count() > 0)
            {
                foreach (var kind in m_RequestDto.AndKinds)
                {
                    func = GetSender(kind.Key);
                    m_RequestDto.ToList = kind.Value;

                    await func(m_RequestDto);
                }
            }

            if (false == sentStatus.IsSuccess &&
                m_RequestDto.OrKinds?.Count() > 0)
            {
                foreach (var kind in m_RequestDto.OrKinds)
                {
                    func = GetSender(kind.Key);
                    m_RequestDto.ToList = kind.Value;

                    sentStatus = await func(m_RequestDto);
                    if (sentStatus.IsSuccess)
                    {
                        break;
                    }
                }
            }

            return sentStatus;
        }

        public Task<IServiceResponse<string>> SmsSendAsync(NotifySend_RequestModel param) =>
            throw new NotImplementedException();

        public async Task<IServiceResponse<string>> SlackSendAsync(NotifySend_RequestModel param)
        {
            var result = new ServiceResponse<string>(true);
            if (string.IsNullOrWhiteSpace(param.ToList))
            {
                param.ToList = "#general";
            }

            if (null == m_SlackCfg?.WebHookUrl)
            {
                throw new ArgumentNullException(nameof(Slack_Option));
            }

            var receivers = param.ToList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var allResponse = new List<IServiceResponse<string>>();
            foreach (var receiver in receivers)
            {
                var msg = new
                {
                    channel = receiver,
                    username = string.IsNullOrWhiteSpace(param.Sender)
                        ? m_ClientApiName
                        : param.Sender,
                    text = $"{param.Title ?? string.Empty} - {param.Message}"
                };

                var jsonData = JsonConvert.SerializeObject(msg);
                var response = await ApiUtils.HttpPost(url: m_SlackCfg.WebHookUrl,
                    jsonData: jsonData,
                    timeoutSecs: ConfigConst.DefaultHttpTimeout
                );

                allResponse.Add(response);
            }

            if (0 == allResponse.Count ||
                allResponse.Any(o => false == o.IsSuccess))
            {
                result.Error(StatusCodeEnum.Error, allResponse.Where(o => false == o.IsSuccess && o.ErrMsg.HasValue())?.First()?.ErrMsg);
            }

            return result;
        }

        public async Task<IServiceResponse<string>> SendMail(IMail mail)
        {
            var sendResult = await mail.SendAsync();
            var result = new ServiceResponse<string>(sendResult as ServiceResponse)
            {
                Data = sendResult?.Data.ToString()
            };

            return result;
        }

        public async Task<IServiceResponse<string>> MailSendAsync(NotifySend_RequestModel param)
        {
            var mailToList = param.ToList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var subject = param.Title;
            if (ServiceContext.IsDebugOrDevelopment())
            {
                subject = $"[{m_SdkEnv}] {subject}";
            }

            var body = param.Message;
            if (null == m_SmtpCfg)
            {
                throw new ArgumentNullException(nameof(Smtp_Option));
            }

            if (string.IsNullOrWhiteSpace(m_SmtpCfg.BodyTemplate) &&
                File.Exists(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    ConfigConst.DefaultAppDataFolder,
                    ConfigConst.DefaultEmailTemplateFile
                )))
            {
                m_SmtpCfg.BodyTemplate = File.ReadAllText(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    ConfigConst.DefaultAppDataFolder,
                    ConfigConst.DefaultEmailTemplateFile
                ));
            }

            var mail = new SmtpMailService(
                new ConfigOptions<Smtp_Option>(m_SmtpCfg)
            );

            if (m_SmtpCfg.BodyTemplate.HasValue())
            {
                mail.Template = m_SmtpCfg.BodyTemplate;
            };

            var template = body;
            if (m_SmtpCfg.BodyTemplate.HasValue())
            {
                template = m_SmtpCfg.BodyTemplate
                    .Replace("{{title}}", subject)
                    .Replace("{{simple_content}}", body)
                    .Replace("{{complete_content}}", MailConfig.Footer)
                    .Replace("{{receive_name}}", param.Greeting ?? string.Empty);
            }

            mail.AddReceiver(mailToList);
            mail.Subject = subject;
            mail.Body = template;

            return await SendMail(mail);
        }

        public async Task<IServiceResponse<string>> LineSendAsync(NotifySend_RequestModel param)
        {
            var result = new ServiceResponse<string>(true);
            if (string.IsNullOrWhiteSpace(param.ToList))
            {
                throw new ArgumentNullException(nameof(param.ToList));
            }

            if (null == m_LineCfg)
            {
                throw new ArgumentNullException(nameof(LINE_Option));
            }

            var receivers = param.ToList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var allResponse = new List<IServiceResponse<string>>();
            foreach (var receiver in receivers)
            {
                var msg = new
                {
                    to = receiver,
                    messages = new[] {
                        new
                        {
                            type = "text",
                            text = $"{param.Title ?? string.Empty} - {param.Message}"
                        }
                    }
                };

                var jsonData = JsonConvert.SerializeObject(msg);
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { CommonConst.AuthHeaderName, m_LineCfg.ApiKey.AttachBearer() }
                };

                var response = await ApiUtils.HttpPost(url: m_LineCfg.HostUrl,
                    jsonData: jsonData,
                    headers: headers,
                    timeoutSecs: ConfigConst.DefaultHttpTimeout
                );

                allResponse.Add(response);
            }

            if (0 == allResponse.Count() ||
                allResponse.Any(o => false == o.IsSuccess))
            {
                result.Error(StatusCodeEnum.Error,
                    allResponse.Where(o => false == o.IsSuccess && o.ErrMsg.HasValue())?.First()?.ErrMsg);
            }

            return result;
        }

        protected Func<NotifySend_RequestModel, Task<IServiceResponse<string>>> GetSender(byte? kind)
        {
            Func<NotifySend_RequestModel, Task<IServiceResponse<string>>> func;
            var channel = (NotifyChannelEnum)kind;
            switch (channel)
            {
                case NotifyChannelEnum.Email: //1.email
                    func = MailSendAsync;
                    break;
                case NotifyChannelEnum.SMS: // 2.sms
                    func = SmsSendAsync;
                    break;
                case NotifyChannelEnum.Line: // 3.line
                    func = LineSendAsync;
                    break;
                case NotifyChannelEnum.Slack: // 4.slack
                    func = SlackSendAsync;
                    break;
                default:
                    func = async (req) => await Task.Run(() =>
                    {
                        return new ServiceResponse<string>()
                            .Error(StatusCodeEnum.InvalidContractRequest, $"Invalid channel(={channel}). ");
                    });
                    break;
            }

            return func;
        }

        protected async Task TryLoadRemoteConfig()
        {
            var multipleKeys = new List<ConfigItem>() {
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Auth_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Notification_Smtp_Options_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Notification_LINE_Options_ConfigKey },
                new ConfigItem(){ ConfigKey = SysConfigKey.Default_Notification_Slack_Options_ConfigKey },
            };

            var whiteEnvs = new List<string>();
            ServiceContext.Configuration.Bind("sdk.white.env", whiteEnvs);
            var client = ComponentMgr.Instance.TryResolve<IConfigClient>();
            foreach (var env in whiteEnvs)
            {
                var apiName = $"{CommonConst.SdkPrefix.ToLower()}.admin.{env}";
                var apiKey = AdminEnvMap[$"{CommonConst.SdkPrefix.ToLower()}.admin.{env}"];
                var baseUrl = ServiceHostUrls[env];

                var response = await client.GetLatest(multipleKeys,
                    apiName: apiName,
                    apiKey: apiKey
                );

                if (response?.Data?.Count() != multipleKeys.Count())
                {
                    Logger.LogError($"Missing Keys [{string.Join(",", multipleKeys.Select(o => o.ConfigKey))}] from ConfigServer. ");
                    continue;
                }

                AuthCfgMap.TryAdd(env, JsonConvert.DeserializeObject<Auth_Option>(
                    response?.Data?[SysConfigKey.Default_Auth_ConfigKey] ?? "")
                );
                SmtpCfgMap.TryAdd(env, JsonConvert.DeserializeObject<Smtp_Option>(
                    response?.Data?[SysConfigKey.Default_Notification_Smtp_Options_ConfigKey] ?? "")
                );
                LineCfgMap.TryAdd(env, JsonConvert.DeserializeObject<LINE_Option>(
                    response?.Data?[SysConfigKey.Default_Notification_LINE_Options_ConfigKey] ?? "")
                );
                SlackCfgMap.TryAdd(env, JsonConvert.DeserializeObject<Slack_Option>(
                    response?.Data?[SysConfigKey.Default_Notification_Slack_Options_ConfigKey] ?? "")
                );
            }
        }

        public static Dictionary<string, string> ServiceHostUrls = null;
        public static Dictionary<string, string> AdminEnvMap = null;
        public static bool IsInitialized = false;
        public static object InitializationLock = new object();
        public static ConcurrentDictionary<string, Auth_Option> AuthCfgMap = new ConcurrentDictionary<string, Auth_Option>(StringComparer.OrdinalIgnoreCase);
        public static ConcurrentDictionary<string, LINE_Option> LineCfgMap = new ConcurrentDictionary<string, LINE_Option>(StringComparer.OrdinalIgnoreCase);
        public static ConcurrentDictionary<string, Smtp_Option> SmtpCfgMap = new ConcurrentDictionary<string, Smtp_Option>(StringComparer.OrdinalIgnoreCase);
        public static ConcurrentDictionary<string, Slack_Option> SlackCfgMap = new ConcurrentDictionary<string, Slack_Option>(StringComparer.OrdinalIgnoreCase);

        protected LINE_Option m_LineCfg;
        protected Smtp_Option m_SmtpCfg;
        protected Slack_Option m_SlackCfg;
        protected string m_ClientApiName = "";
        protected string m_ClientApiKey = "";
        protected HttpRequest m_Request;
        protected NotifySend_RequestModel m_RequestDto;
        protected string m_SdkEnv = EnvironmentEnum.Debug.GetDisplayName();
    }
}
