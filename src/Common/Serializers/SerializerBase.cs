using System.Text;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;

namespace Nwpie.Foundation.Common.Serializers
{
    public abstract class SerializerBase : CObject, ISerializer
    {
        public SerializerBase()
        {
            Initialization();
        }

        public abstract void Initialization();

        public abstract T Deserialize<T>(string serialized);
        public virtual T Deserialize<T>(string serialized, bool ignoreException)
        {
            try
            {
                return Deserialize<T>(serialized);
            }
            catch
            {
                if (ignoreException)
                {
                    return default(T);
                }

                throw;
            }
        }
        public abstract string Serialize<T>(T deserialized);

        public virtual T Deserialize<T>(byte[] data)
        {
            return Deserialize<T>(m_Encoding.GetString(data, 0, data.Length));
        }

        public virtual byte[] SerializeToBytes<T>(T target)
        {
            return m_Encoding.GetBytes(Serialize(target));
        }

        public Encoding Encoding
        {
            get => m_Encoding;
            set => m_Encoding = value;
        }

        protected Encoding m_Encoding = Encoding.UTF8;
    }
}
