using System.IO;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Logging.Models;
using Nwpie.Foundation.Http.Common.Utilities;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Extensions
{
    public static class RequestExtension
    {
        public static async Task<HttpRequestLog> FormatRequest(this HttpRequest request, HttpContext context)
        {
            var log = new HttpRequestLog()
            {
                Scheme = request.Scheme,
                Host = request.Host.ToString(),
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                Headers = ApiUtils.FormatHeader(request.Headers)
            };

            if (false == request.ContentLength > 0)
            {
                return log;
            }

            // Allows request back at the beginning of its stream.
            request.EnableBuffering();
            using (var stream = new StreamReader(request.Body))
            {
                stream.BaseStream.Seek(0, SeekOrigin.Begin);
                log.Body = await stream.ReadToEndAsync();

                // Reset the request body stream position so the next middleware can read it
                context.Request.Body.Position = 0;
            }

            return log;
        }
    }
}
