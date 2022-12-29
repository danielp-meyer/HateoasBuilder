using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeyerCorp.HateoasBuilder
{
    /// <summary>
    /// Methods for starting the HateoasBuilder call chain optimized for Web API and Azure Functions-based applications.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Add a new item to the Tuple collection automatically adding the <param name="rawRelativeUrl"/> to a new LinkInformation object.
        /// </summary>
        /// <param name="list">Object to which to add a new member.</param>
        /// <param name="relLabel">Label value for the link item.</param>
        /// <param name="rawRelativeUrl">Full relative URL value to add to the LinkInformation item.</param>
        /// <exception cref="ArgumentException"><paramref name="relLabel"/> cannot be null, empty, or whitespace</exception>
        internal static void Add(this List<Tuple<string, LinkInformation>> list, string relLabel, string? rawRelativeUrl)
        {
            var rel = relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));

            list.Add(new Tuple<string, LinkInformation>(rel, new LinkInformation(rawRelativeUrl)));
        }

        /// <summary>
        /// Add a new item to the Tuple collection automatically adding the <param name="routeItems"/> 
        /// collection to the route items collection of the new LinkInformation object.
        /// </summary>
        /// <param name="list">Object to which to add a new member.</param>
        /// <param name="relLabel">Label value for the link item.</param>
        /// <param name="routeItems"></param>
        /// <exception cref="ArgumentNullException"><paramref name="routeItems"/> cannot be null</exception>
        /// <exception cref="ArgumentException"><paramref name="relLabel"/> cannot be null, empty, or whitespace</exception>
        internal static void AddRouteLink(this List<Tuple<string, LinkInformation>> list, string relLabel, params object[] routeItems)
        {
            if (routeItems == null) throw new ArgumentNullException(nameof(routeItems));

            var rel = relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));
            var linkinformation = new LinkInformation(routeItems, null);

            list.Add(new Tuple<string, LinkInformation>(rel, linkinformation));
        }

        /// <summary>
        /// Add a new item to the Tuple collection automatically adding the <param name="queryItems"/> 
        /// collection to the route items collection of the new LinkInformation object.
        /// </summary>
        /// <param name="list">Object to which to add a new member.</param>
        /// <param name="relLabel">Label value for the link item.</param>
        /// <param name="queryItems"></param>
        /// <exception cref="ArgumentNullException"><paramref name="queryItems"/> cannot be null</exception>
        /// <exception cref="ArgumentException"><paramref name="relLabel"/> cannot be null, empty, or whitespace</exception>
        internal static void AddQueryLink(this List<Tuple<string, LinkInformation>> list, string relLabel, params object[] queryItems)
        {
            if (queryItems == null) throw new ArgumentNullException(nameof(queryItems));

            var rel = relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));
            var linkinformation = new LinkInformation(null, queryItems);

            list.Add(new Tuple<string, LinkInformation>(rel, linkinformation));
        }

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
        /// <param name="baseUrl">The base URL to use for all links added to the Link Builder.</param>
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
        public static LinkBuilder AddLink(this string baseUrl, bool condition, string relLabel, string? rawRelativeUrl)
        {
            if (condition)
            {
                var rel = relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));

                return new LinkBuilder(baseUrl, rel, rawRelativeUrl);
            }
            else
                return new LinkBuilder(!condition, baseUrl);
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
        /// <param name="baseUrl">Value for the base URL for the new link builder object.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddLink(this string baseUrl, string relLabel, string? rawRelativeUrl) => baseUrl.AddLink(true, relLabel, rawRelativeUrl);

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
        /// <param name="baseUrl">Value for the base URL for the new link builder object.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="routeItems">Items which will be concatenated with delimiting slashes to create a route after the base URL.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddRouteLink(this string baseUrl, string relLabel, params object[] routeItems)
        {
            return baseUrl.AddRouteLink(true, relLabel, routeItems);
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
        /// <param name="baseUrl">Value for the base URL for the new link builder object.</param>
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
        public static LinkBuilder AddRouteLink(this string baseUrl, bool condition, string relLabel, params object[] routeItems)
        {
            var builder = new LinkBuilder(!condition, baseUrl);

            if (condition) builder.AddRouteLink(relLabel, routeItems);

            return builder;
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="baseUrl">Value for the base URL for the new link builder object.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <param name="queryPairs">Values which will be concatenated as a query parameter list for the URL of the last link added.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddQueryLink(this string baseUrl, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return baseUrl.AddQueryLink(true, relLabel, relativeUrl, queryPairs);
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
        /// <param name="baseUrl">Value for the base URL for the new link builder object.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <param name="queryPairs">Values which will be concatenated as a query parameter list for the URL of the last link added.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpContext.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddQueryLink(this string baseUrl, bool condition, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return baseUrl.AddRouteLink(condition, relLabel, relativeUrl).AddParameters(queryPairs);
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
        /// <param name="baseUrl">Value for the base URL for the new link builder object.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relPathFormat">A format string template which the resulting URL will be based.</param>
        /// <param name="formatItems">Collection of values for the format string.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddFormattedLink(this string baseUrl, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return baseUrl.AddFormattedLink(true, relLabel, relPathFormat, formatItems);
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
        /// <param name="baseUrl">Value for the base URL for the new link builder object.</param>
        /// <param name="condition">Condition on which to ignore adding this new link.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relPathFormat">A format string template which the resulting URL will be based.</param>
        /// <param name="formatItems">Collection of values for the format string.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpContext.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public static LinkBuilder AddFormattedLink(this string baseUrl, bool condition, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return baseUrl.AddLink(condition, relLabel, String.Format(relPathFormat, formatItems));
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
        /// Find the URL for a "self" rel label in a list of links.
        /// </summary>
        /// <param name="links">
        /// A collection of links yielded by the <see cref="LinkBuilder.Build(bool)"/> 
        /// or <see cref="LinkBuilder.BuildEncoded()"/> method.
        /// </param>
        /// <returns>
        /// A Link object which has the "self" relLabel./> value as its rel label.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="links"/> cannot be null.</exception>
        public static string ToSelfHref(this IEnumerable<Link> links)
        {
            return links.ToHref("self");
        }

        /// <summary>
        /// Find the URL for a given rel label in a list of links.
        /// </summary>
        /// <param name="links">
        /// A collection of links yielded by the <see cref="LinkBuilder.Build(bool)"/> 
        /// or <see cref="LinkBuilder.BuildEncoded()"/> method.
        /// </param>
        /// <param name="relLabel">The label which will be used to filter the list of link objects.</param>
        /// <returns>
        /// A Link object which has the <paramref name="relLabel"/> value as its rel label.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="links"/> cannot be null.</exception>
        /// <exception cref="ArgumentException"><paramref name="relLabel"/> cannot be null, empty, or whitespace.</exception>"
        public static string ToHref(this IEnumerable<Link> links, string relLabel)
        {
            if (links == null) throw new ArgumentNullException(nameof(links));
            relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));

            return links.Single(l => l.Rel == relLabel).Href;
        }

        /// <summary>
        /// Check if a string value is null, empty, or whitespace.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <param name="parameterName">The name of the parameter being checked in the method where this call is originating.</param>
        /// <returns>
        /// The value trimmed of whitespace.</returns>
        /// <exception cref="ArgumentException">If the value is null, empty, or whitespace.</exception>
        public static string CheckIfNullOrWhiteSpace(this string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", parameterName);
            else
                return value.Trim();
        }

        /// <summary>
        /// Extension method allowing convenient sequential generation of hash values for overriding GetHash methods.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hash">Hash value to continue the call chain on.</param>
        /// <param name="value">Value to hash</param>
        /// <param name="seed">Seed value</param>
        /// <returns></returns>
        internal static int HashThis<T>(this int hash, T value, int seed = 23)
        {
            var check = value == null ? (int?)null : hash * seed + value.GetHashCode();

            System.Diagnostics.Debug.WriteLine($"Value: {value} Value Hash: {value?.GetHashCode()} Full Result: {check}");

            return value == null
                ? hash
                : hash * seed + value.GetHashCode();
        }
    }
}
