using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nwpie.Foundation.Configuration.SDK.Extensions
{
    public class ConfigServerParser
    {
        private ConfigServerParser(string configKey)
        {
            m_ConfigKey = configKey;
        }

        public static IDictionary<string, string> Parse<TConfig>(string configKey, string input)
        {
            return new ConfigServerParser(configKey)
                .ParseStream<TConfig>(input);
        }

        protected IDictionary<string, string> ParseStream<TConfig>(string input)
        {
            m_Data.Clear();
            if (typeof(TConfig) == typeof(string))
            {
                m_Data.Add(m_ConfigKey, input);
            }
            else
            {
                var byteArray = Encoding.UTF8.GetBytes(input);
                var stream = new MemoryStream(byteArray);
                m_Reader = new JsonTextReader(new StreamReader(stream))
                {
                    DateParseHandling = DateParseHandling.None
                };

                var jsonConfig = JObject.Load(m_Reader);
                VisitJObject(jsonConfig);
            }

            return m_Data;
        }

        protected void VisitJObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                EnterContext(property.Name);
                VisitProperty(property);
                ExitContext();
            }
        }

        protected void VisitProperty(JProperty property)
        {
            VisitToken(property.Value);
        }

        protected void VisitToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitJObject(token.Value<JObject>());
                    break;

                case JTokenType.Array:
                    VisitArray(token.Value<JArray>());
                    break;

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    VisitPrimitive(token.Value<JValue>());
                    break;

                default:
                    const string strUnsupportedToken = "Unsupported JSON token '{0}' was found. Path '{1}', line {2} position {3}. ";
                    throw new FormatException(string.Format(strUnsupportedToken, m_Reader.TokenType, m_Reader.Path, m_Reader.LineNumber, m_Reader.LinePosition));
            }
        }

        protected void VisitArray(JArray array)
        {
            for (var index = 0; index < array.Count; index++)
            {
                EnterContext(index.ToString());
                VisitToken(array[index]);
                ExitContext();
            }
        }

        protected void VisitPrimitive(JValue data)
        {
            var key = $"{m_ConfigKey}{ConfigurationPath.KeyDelimiter}{m_CurrentPath}";
            if (m_Data.ContainsKey(key))
            {
                throw new FormatException($"Key (={key}) is duplicated. ");
            }

            m_Data[key] = data.ToString(CultureInfo.InvariantCulture);
        }

        protected void EnterContext(string context)
        {
            m_Context.Push(context);
            m_CurrentPath = ConfigurationPath.Combine(m_Context.Reverse());
        }

        protected void ExitContext()
        {
            m_Context.Pop();
            m_CurrentPath = ConfigurationPath.Combine(m_Context.Reverse());
        }

        protected readonly IDictionary<string, string> m_Data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        protected readonly Stack<string> m_Context = new Stack<string>();
        protected string m_CurrentPath;
        protected string m_ConfigKey;
        protected JsonTextReader m_Reader;
    }
}
