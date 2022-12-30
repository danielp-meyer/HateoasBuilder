using Microsoft.Azure.Functions.Worker.Http;
using System;

namespace MeyerCorp.HateoasBuilder
{
    public static partial class Extensions
    {
        /// <summary>
        /// Extract the base URL from the HTTP context object of a Web API controller.
        /// </summary>
        /// <param name="httpRequestData">HTTP context object of a Web API controller.</param>
        /// <returns>String representing the base URL</returns>
        /// <exception cref="ArgumentNullException"><paramref name="httpRequestData"/> cannot be null</exception>
        internal static string ToBaseUrl(this HttpRequestData httpRequestData)
        {
            if (httpRequestData == null) throw new ArgumentNullException(nameof(httpRequestData));

            var pathlength = httpRequestData.Url.AbsolutePath.Length;
            var urilength = httpRequestData.Url.AbsoluteUri.Length;
            var baseurl = httpRequestData.Url.AbsoluteUri.Substring(0, urilength - pathlength);

            return baseurl;
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpRequestData.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddLink(this HttpRequestData httpRequestData, bool condition, string relLabel, string? rawRelativeUrl)
        {
            return httpRequestData
                .ToBaseUrl()
                .AddLink(condition, relLabel, rawRelativeUrl);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddLink(this HttpRequestData httpRequestData, string relLabel, string? rawRelativeUrl) => httpRequestData.AddLink(true, relLabel, rawRelativeUrl);

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="routeItems">Items which will be concatenated with delimiting slashes to create a route after the base URL.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddRouteLink(this HttpRequestData httpRequestData, string relLabel, params object[] routeItems)
        {
            return httpRequestData.ToBaseUrl().AddRouteLink(true, relLabel, routeItems);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="routeItems">Items which will be concatenated with delimiting slashes to create a route after the base URL.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpRequestData.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddRouteLink(this HttpRequestData httpRequestData, bool condition, string relLabel, params object[] routeItems)
        {
            return httpRequestData.ToBaseUrl().AddRouteLink(condition, relLabel, routeItems);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <param name="queryPairs">Values which will be concatenated as a query parameter list for the URL of the last link added.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddQueryLink(this HttpRequestData httpRequestData, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return httpRequestData.AddQueryLink(true, relLabel, relativeUrl, queryPairs);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <param name="queryPairs">Collection of values for the format string.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpRequestData.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddQueryLink(this HttpRequestData httpRequestData, bool condition, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return httpRequestData.ToBaseUrl().AddQueryLink(condition, relLabel, relativeUrl, queryPairs);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relPathFormat">A format string template which the resulting URL will be based.</param>
        /// <param name="formatItems">Collection of values for the format string.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddFormattedLink(this HttpRequestData httpRequestData, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return httpRequestData.ToBaseUrl().AddFormattedLink(relLabel, relPathFormat, formatItems);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relPathFormat">A format string template which the resulting URL will be based.</param>
        /// <param name="formatItems">Collection of values for the format string.</param>
        /// <returns>
        /// A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpRequestData.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddFormattedLink(this HttpRequestData httpRequestData, bool condition, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return httpRequestData.ToBaseUrl().AddFormattedLink(condition, relLabel, relPathFormat, formatItems);
        }

        //     public static partial class Extensions
        //     {
        //         /// <summary>
        //         /// Create a name and hyperlink pair based on the current HttpRequestData which can be added to an API's HTTP response.
        //         /// </summary>
        //         /// <param name="httpRequestData">The current HttpRequestData in an Web API controller.</param>
        //         /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        //         /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        //         /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        //         public static LinkBuilder AddLink(this HttpRequestData httpRequestData, string relLabel, string? rawRelativeUrl)
        //         {
        //             if (httpRequestData == null) throw new ArgumentNullException(nameof(httpRequestData));

        //             var pathlength = httpRequestData.Url.AbsolutePath.Length;
        //             var urilength = httpRequestData.Url.AbsoluteUri.Length;
        //             var baseurl = httpRequestData.Url.AbsoluteUri.Substring(0, urilength - pathlength);

        //             return baseurl.AddLink(relLabel, rawRelativeUrl);
        //         }

        //         public static LinkBuilder AddFormattedLink(this HttpRequestData httpRequestData, string relLabel, string relPathFormat, params object[] formatItems)
        //         {
        //             if (formatItems == null) throw new ArgumentNullException(nameof(formatItems));

        //             return httpRequestData.AddLink(relLabel, String.Format(relPathFormat, formatItems));
        //         }

        //         public static LinkBuilder AddQueryLink(this HttpRequestData httpRequestData, string relLabel, string relativeUrl, params object[] queryPairs)
        //         {
        //             return httpRequestData.AddRouteLink(relLabel, relativeUrl).AddParameters(queryPairs);
        //         }

        //         public static LinkBuilder AddRouteLink(this HttpRequestData httpRequestData, string relLabel, params object[] routeItems)
        //         {
        //             if (routeItems == null) throw new ArgumentNullException(nameof(routeItems));
        //             // Consider the first item as the relativeUrl...
        //             if (routeItems.Length > 1 && routeItems.Any(ri => ri == null)) throw new ArgumentException("Collection cannot contain null elements.", nameof(routeItems));

        //             var output = String.Join('/', routeItems.Select(ri => ri?.ToString()));

        //             return httpRequestData.AddLink(relLabel, output);
        //         }
        //     }
    }
}