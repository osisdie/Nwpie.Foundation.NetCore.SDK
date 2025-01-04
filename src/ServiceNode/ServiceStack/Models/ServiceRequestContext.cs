using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.ServiceNode;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.ServiceNode;
using Nwpie.Foundation.Http.Common.Utilities;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Models
{
    /// <summary>
    /// ServiceRequestContext now depends on AsyncLocal.LogicalCallContext.
    /// </summary>
    public class ServiceRequestContext
    {
        private ServiceRequestContext() { }

        public static ServiceRequestContext Current
        {
            get
            {
                if (m_InstanceOfCurrentThread == null)
                {
                    lock (m_Lock)
                    {
                        if (m_InstanceOfCurrentThread == null)
                        {
                            m_InstanceOfCurrentThread = new ServiceRequestContext();
                        }
                    }
                }

                return m_InstanceOfCurrentThread;
            }
        }

        public static void Cleanup()
        {
            m_InstanceOfCurrentThread = null;
        }

        public string CallingService
        {
            get
            {
                var callingService = GetFromLogicalCallContext(SNConst.HTTPHeader_CallingService);
                return callingService;
            }
            set
            {
                SetToLogicalCallContext(SNConst.HTTPHeader_CallingService, value);
            }
        }

        public string ExecutingService
        {
            get
            {
                var execService = GetFromLogicalCallContext(SNConst.HTTPHeader_ExecutingService);
                return execService;
            }
            set
            {
                SetToLogicalCallContext(SNConst.HTTPHeader_ExecutingService, value);
            }
        }

        public string RequestId
        {
            get
            {
                var reqId = GetFromLogicalCallContext(SNConst.HTTPHeader_RequestID);
                return reqId;
            }
            set
            {
                SetToLogicalCallContext(SNConst.HTTPHeader_RequestID, value);
            }
        }

        public long Depth
        {
            get
            {
                var depthStr = GetFromLogicalCallContext(SNConst.HTTPHeader_Depth);
                if (string.IsNullOrEmpty(depthStr))
                {
                    return 0;
                }

                return long.Parse(depthStr);
            }
            set
            {
                SetToLogicalCallContext(SNConst.HTTPHeader_Depth, value.ToString());
            }
        }

        public long SequenceNum
        {
            get
            {
                var snStr = GetFromLogicalCallContext(SNConst.HTTPHeader_SequenceNum);
                if (string.IsNullOrEmpty(snStr))
                {
                    return 0;
                }

                return long.Parse(snStr);
            }
            set
            {
                SetToLogicalCallContext(SNConst.HTTPHeader_SequenceNum, value.ToString());
            }
        }

        private string GetFromLogicalCallContext(string key)
        {
            string value;
            var objValue = CallContext.LogicalGetData(key);
            if (objValue != null)
            {
                value = objValue as string;
            }
            else // HTTP call boundary.
            {
                value = GetFromHttpRequestHeaders(key);
                if (false == string.IsNullOrEmpty(value))
                {
                    SetToLogicalCallContext(key, value);
                }
            }

            return value;
        }

        private void SetToLogicalCallContext(string key, string value)
        {
            CallContext.LogicalSetData(key, value);
        }

        private string GetFromHttpRequestHeaders(string key)
        {
            string value = null;
            if (true == HttpHelper.HttpContext?.Request?.Headers?.ContainsKey(key))
            {
                value = HttpHelper.HttpContext.Request.Headers[key];
            }

            return value;
        }

        public string DumpString
        {
            get
            {
                return string.Join(", ", new List<string>
                {
                    $"{SNConst.HTTPHeader_RequestID}:{Current.RequestId}",
                    $"{SNConst.HTTPHeader_Depth}:{Current.Depth}",
                    $"{SNConst.HTTPHeader_SequenceNum}:{Current.SequenceNum}",
                    $"{SNConst.HTTPHeader_ExecutingService}:{Current.ExecutingService}",
                    $"{SNConst.HTTPHeader_CallingService}:{Current.CallingService}"
                });
            }
        }

        public string DumpCurrentHttpHeaders
        {
            get
            {
                if (null != HttpHelper.HttpContext)
                {
                    return string.Join(", ", new List<string>
                    {
                        $"{SNConst.HTTPHeader_RequestID}:{HttpHelper.HttpContext.Request?.Headers?[SNConst.HTTPHeader_RequestID]}",
                        $"{SNConst.HTTPHeader_Depth}:{HttpHelper.HttpContext.Request?.Headers?[SNConst.HTTPHeader_Depth]}",
                        $"{SNConst.HTTPHeader_SequenceNum}:{HttpHelper.HttpContext.Request?.Headers?[SNConst.HTTPHeader_SequenceNum]}",
                        $"{SNConst.HTTPHeader_ExecutingService}:{HttpHelper.HttpContext.Request?.Headers?[SNConst.HTTPHeader_ExecutingService]}",
                        $"{SNConst.HTTPHeader_CallingService}:{HttpHelper.HttpContext.Request?.Headers?[SNConst.HTTPHeader_CallingService]}"
                    });
                }

                return string.Empty;
            }
        }

        public static void AttachToHttpHeaders(NameValueCollection headers)
        {
            try
            {
                if (ServiceContext.MeasurementTraceEnabled)
                {
                    if (!string.IsNullOrEmpty(headers?[SNConst.HTTPHeader_RequestID]))
                    {
                        headers.Remove(SNConst.HTTPHeader_RequestID);
                    }

                    if (!string.IsNullOrEmpty(headers?[SNConst.HTTPHeader_Depth]))
                    {
                        headers.Remove(SNConst.HTTPHeader_Depth);
                    }

                    if (!string.IsNullOrEmpty(headers?[SNConst.HTTPHeader_SequenceNum]))
                    {
                        headers.Remove(SNConst.HTTPHeader_SequenceNum);
                    }

                    if (!string.IsNullOrEmpty(headers?[SNConst.HTTPHeader_CallingService]))
                    {
                        headers.Remove(SNConst.HTTPHeader_CallingService);
                    }

                    headers[SNConst.HTTPHeader_SequenceNum] = Current.SequenceNum.ToString();
                    headers[SNConst.HTTPHeader_RequestID] = Current.RequestId;
                    headers[SNConst.HTTPHeader_Depth] = Current.Depth.ToString();
                    headers[SNConst.HTTPHeader_CallingService] = Current.ExecutingService;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        private static readonly ILogger Logger = LogMgr.CreateLogger(typeof(ServiceRequestContext));
        private static readonly object m_Lock = new object();

        private static ServiceRequestContext m_InstanceOfCurrentThread;
    }
}
