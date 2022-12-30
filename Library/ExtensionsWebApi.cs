using Microsoft.AspNetCore.Http;
using System;

namespace MeyerCorp.HateoasBuilder
{
    public static partial class Extensions
    {
        /// <summary>
        /// Extract the base URL from the HTTP context object of a Web API controller.
        /// </summary>
        /// <param name="httpContext">HTTP context object of a Web API controller.</param>
        /// <returns>String representing the base URL</returns>
        /// <exception cref="ArgumentNullException"><paramref name="httpContext"/> cannot be null</exception>
        internal static string ToBaseUrl(this HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var request = httpContext.Request;
            var baseurl = $"{request.Scheme}://{request.Host}";

            return baseurl;
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpContext.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddLink(this HttpContext httpContext, bool condition, string relLabel, string? rawRelativeUrl)
        {
            return httpContext
                .ToBaseUrl()
                .AddLink(condition, relLabel, rawRelativeUrl);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddLink(this HttpContext httpContext, string relLabel, string? rawRelativeUrl) => httpContext.AddLink(true, relLabel, rawRelativeUrl);

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="routeItems">Items which will be concatenated with delimiting slashes to create a route after the base URL.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddRouteLink(this HttpContext httpContext, string relLabel, params object[] routeItems)
        {
            return httpContext.ToBaseUrl().AddRouteLink(true, relLabel, routeItems);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="routeItems">Items which will be concatenated with delimiting slashes to create a route after the base URL.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpContext.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddRouteLink(this HttpContext httpContext, bool condition, string relLabel, params object[] routeItems)
        {
            return httpContext.ToBaseUrl().AddRouteLink(condition, relLabel, routeItems);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <param name="queryPairs">Values which will be concatenated as a query parameter list for the URL of the last link added.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddQueryLink(this HttpContext httpContext, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return httpContext.AddQueryLink(true, relLabel, relativeUrl, queryPairs);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <param name="queryPairs">Collection of values for the format string.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpContext.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddQueryLink(this HttpContext httpContext, bool condition, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return httpContext.ToBaseUrl().AddQueryLink(condition, relLabel, relativeUrl, queryPairs);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relPathFormat">A format string template which the resulting URL will be based.</param>
        /// <param name="formatItems">Collection of values for the format string.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddFormattedLink(this HttpContext httpContext, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return httpContext.ToBaseUrl().AddFormattedLink(relLabel, relPathFormat, formatItems);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
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
        /// <code>HttpContext.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddFormattedLink(this HttpContext httpContext, bool condition, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return httpContext.ToBaseUrl().AddFormattedLink(condition, relLabel, relPathFormat, formatItems);
        }

        /// <summary>
        /// Add a link to an external site
        /// </summary>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">Link label</param>
        /// <param name="relPathFormat">Link absolute path or absolute path template</param>
        /// <param name="formatItems">Template items</param>
        /// <returns>This link builder w/ the <paramref name="relLabel"/> and <paramref name="relPathFormat"/> result added to the RelHrefPairs colleciton property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="relLabel"/> cannot be null, empty, or whitespace</exception>
        /// <exception cref="ArgumentException"><paramref name="relPathFormat"/> can be null, empty, or whitespace only when the <paramref name="formatItems"/>parameter is empty</exception>
        /// <remarks>
        /// The link generated by this will not use the base URL that is set in the original object.
        /// </remarks>
        public static LinkBuilder AddExternalLink(this string baseUrl, bool condition, string relLabel, string relPathFormat, params object[] formatItems)
        {
            var linkbuilder = new LinkBuilder(baseUrl);

            linkbuilder.AddExternalLink(condition, relLabel, String.Format(relPathFormat, formatItems));

            return linkbuilder;
        }

        // public static LinkBuilder AddFormattedLinks(this string baseUrl, string rel, string format, IEnumerable<string> items)
        // {
        //     if (String.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(baseUrl));

        //     var output = new LinkBuilder(baseUrl);

        //     return output.AddFormattedLinks(rel, format, items);
        // }
    }
}
