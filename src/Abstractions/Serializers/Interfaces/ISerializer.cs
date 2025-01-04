using System.Text;

namespace Nwpie.Foundation.Abstractions.Serializers.Interfaces
{
    public interface ISerializer
    {
        void Initialization();
        T Deserialize<T>(string serialized, bool ignoreException);

        TTarget Deserialize<TTarget>(byte[] data);
        TTarget Deserialize<TTarget>(string data);
        string Serialize<TTarget>(TTarget target);
        byte[] SerializeToBytes<TTarget>(TTarget target);
        Encoding Encoding { get; set; }
    }
}
