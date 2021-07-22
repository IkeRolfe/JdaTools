using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JdaTools.Connection
{
    internal class MocaHttpHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }
}