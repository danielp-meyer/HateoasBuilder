using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class TestHttpRequestData : HttpRequestData
    {
        public TestHttpRequestData(string url) : base(new TestFunctionContext())
        {
            Url = new Uri(url);
        }

        public override Stream Body => throw new NotImplementedException();

        public override HttpHeadersCollection Headers => throw new NotImplementedException();

        public override IReadOnlyCollection<IHttpCookie> Cookies => throw new NotImplementedException();

        public override Uri Url {get;}

        public override IEnumerable<ClaimsIdentity> Identities => throw new NotImplementedException();

        public override string Method => throw new NotImplementedException();

        public override HttpResponseData CreateResponse()
        {
            throw new NotImplementedException();
        }
    }
}