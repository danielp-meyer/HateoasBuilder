using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class TestHttpRequest : HttpRequest
    {
        public override HttpContext HttpContext => throw new System.NotImplementedException();

        public override string Method { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override string? Scheme { get ; set; }
        public override bool IsHttps { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override HostString Host { get; set ; }
        public override PathString PathBase { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override PathString Path { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override QueryString QueryString { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override IQueryCollection Query { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override string Protocol { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override IHeaderDictionary Headers => throw new System.NotImplementedException();

        public override IRequestCookieCollection Cookies { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override long? ContentLength { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override string ContentType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override Stream Body { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override bool HasFormContentType => throw new System.NotImplementedException();

        public override IFormCollection Form { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}