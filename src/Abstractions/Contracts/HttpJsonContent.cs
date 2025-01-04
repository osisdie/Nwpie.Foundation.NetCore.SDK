using System.Text;

namespace Nwpie.Foundation.Abstractions.Contracts
{
    public class HttpJsonContent : System.Net.Http.StringContent
    {
        public HttpJsonContent(string content)
            : this(content, Encoding.UTF8)
        {
        }

        public HttpJsonContent(string content, Encoding encoding)
            : base(content, encoding, "application/json")
        {
        }
    }

    public class XmlContent : System.Net.Http.StringContent
    {
        public XmlContent(string content)
            : this(content, Encoding.UTF8)
        {
        }

        public XmlContent(string content, Encoding encoding)
            : base(content, encoding, "application/xml")
        {
        }
    }
}
