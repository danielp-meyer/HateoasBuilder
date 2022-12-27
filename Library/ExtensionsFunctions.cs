using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Linq;

namespace MeyerCorp.HateoasBuilder
{
    public static partial class Extensions
    {
        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddLink(this HttpRequestData httpRequestData, string relLabel, string? rawRelativeUrl)
        {
            if (httpRequestData == null) throw new ArgumentNullException(nameof(httpRequestData));

            var pathlength = httpRequestData.Url.AbsolutePath.Length;
            var urilength = httpRequestData.Url.AbsoluteUri.Length;
            var baseurl = httpRequestData.Url.AbsoluteUri.Substring(0, urilength - pathlength);

            return baseurl.AddLink(relLabel, rawRelativeUrl);
        }

        public static LinkBuilder AddFormattedLink(this HttpRequestData httpRequestData, string relLabel, string relPathFormat, params object[] formatItems)
        {
            if (formatItems == null) throw new ArgumentNullException(nameof(formatItems));

            return httpRequestData.AddLink(relLabel, String.Format(relPathFormat, formatItems));
        }

        public static LinkBuilder AddQueryLink(this HttpRequestData httpRequestData, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return httpRequestData.AddRouteLink(relLabel, relativeUrl).AddParameters(queryPairs);
        }

        public static LinkBuilder AddRouteLink(this HttpRequestData httpRequestData, string relLabel, params object[] routeItems)
        {
            if (routeItems == null) throw new ArgumentNullException(nameof(routeItems));
            // Consider the first item as the relativeUrl...
            if (routeItems.Length > 1 && routeItems.Any(ri => ri == null)) throw new ArgumentException("Collection cannot contain null elements.", nameof(routeItems));

            var output = String.Join('/', routeItems.Select(ri => ri?.ToString()));

            return httpRequestData.AddLink(relLabel, output);
        }
    }
}