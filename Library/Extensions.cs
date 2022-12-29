﻿using Microsoft.AspNetCore.Http;
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
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddLink(this string baseUrl, string relLabel, string? rawRelativeUrl)
        {
            return baseUrl.AddLink(true, relLabel, rawRelativeUrl);
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
                return baseUrl.AddLink(relLabel, rawRelativeUrl);
            else
            {
                var url = baseUrl.CheckIfNullOrWhiteSpace(nameof(baseUrl));
                var rel = relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));

                return new LinkBuilder(url);
            }
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
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddLink(this HttpContext httpContext, string relLabel, string? rawRelativeUrl)
        {
            return httpContext.AddLink(true, relLabel, rawRelativeUrl);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddRouteLink(this HttpContext httpContext, string relLabel, params object[] routeItems)
        {
            if (routeItems == null) throw new ArgumentNullException(nameof(routeItems));
            // Consider the first item as the relativeUrl...
            if (routeItems.Length > 1 && routeItems.Any(ri => ri == null)) throw new ArgumentException("Collection cannot contain null elements.", nameof(routeItems));

            return httpContext
                .ToBaseUrl()
                .AddRouteLink(relLabel, routeItems);
        }

        public static LinkBuilder AddRouteLink(this string baseUrl, string relLabel, params object[] routeItems)
        {
            var builder = new LinkBuilder(baseUrl);

            builder.AddRouteLink(relLabel, routeItems);

            return builder;
        }

        public static LinkBuilder AddRouteLink(this HttpContext httpContext, bool condition, string relLabel, params object[] routeItems)
        {
            return condition
                ? httpContext.AddRouteLink(relLabel, routeItems)
                : new LinkBuilder(!condition, httpContext);
        }

        public static LinkBuilder AddRouteLink(this string baseUrl, bool condition, string relLabel, params object[] routeItems)
        {
            return condition
                ? baseUrl.AddRouteLink(relLabel, routeItems)
                : new LinkBuilder(!condition, baseUrl);
        }

        public static LinkBuilder AddQueryLink(this string baseUrl, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return baseUrl
                .AddRouteLink(relLabel, relativeUrl)
                .AddParameters(queryPairs);
        }

        public static LinkBuilder AddQueryLink(this HttpContext httpContext, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return httpContext
                .ToBaseUrl()
                .AddQueryLink(relLabel, relativeUrl, queryPairs);
        }

        public static LinkBuilder AddQueryLink(this string baseUrl, bool condition, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return condition
                ? baseUrl.AddRouteLink(relLabel, relativeUrl).AddParameters(queryPairs)
                : new LinkBuilder(!condition, baseUrl);
        }

        public static LinkBuilder AddQueryLink(this HttpContext httpContext, bool condition, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return condition
                ? httpContext.AddQueryLink(relLabel, relativeUrl, queryPairs)
                : new LinkBuilder(!condition, httpContext);
        }

        public static LinkBuilder AddFormattedLink(this string baseUrl, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return baseUrl.AddLink(relLabel, String.Format(relPathFormat, formatItems));
        }

        public static LinkBuilder AddFormattedLink(this HttpContext httpContext, string relLabel, string relPathFormat, params object[] formatItems)
        {
            if (formatItems == null) throw new ArgumentNullException(nameof(formatItems));

            return httpContext.AddLink(relLabel, String.Format(relPathFormat, formatItems));
        }

        public static LinkBuilder AddFormattedLink(this string baseUrl, bool condition, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return condition
                ? baseUrl.AddLink(relLabel, String.Format(relPathFormat, formatItems))
                : new LinkBuilder(!condition, baseUrl);
        }

        public static LinkBuilder AddFormattedLink(this HttpContext httpContext, bool condition, string relLabel, string relPathFormat, params object[] formatItems)
        {
            return condition
                ? httpContext.AddFormattedLink(relLabel, relPathFormat, formatItems)
                : new LinkBuilder(!condition, httpContext);
        }

        public static string ToSelfHref(this IEnumerable<Link> links)
        {
            return links.ToHref("self");
        }

        public static string ToHref(this IEnumerable<Link> links, string rel)
        {
            if (links == null) throw new ArgumentNullException(nameof(links));
            rel.CheckIfNullOrWhiteSpace(nameof(rel));

            return links.Single(l => l.Rel == rel).Href;
        }

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
        /// <param name="hash"></param>
        /// <param name="value"></param>
        /// <param name="seed"></param>
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
