using System.Net;
using System.Net.Http;

namespace Saint.Ikki.Fx.Shared.Models.Infrastructure
{
    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler() => AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    }
}
