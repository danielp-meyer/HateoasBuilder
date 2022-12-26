using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MeyerCorp.HateoasBuilder
{
    public static class Extensions
    {
        internal static void Add(this List<Tuple<string, string>> list, string relLabel, string rawRelativeUrl)
        {
            relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));
            rawRelativeUrl.CheckIfNullOrWhiteSpace(nameof(rawRelativeUrl));

            list.Add(new Tuple<string, string>(relLabel.Trim(), rawRelativeUrl.Trim()));
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddLink(this string baseUrl, string relLabel, string rawRelativeUrl)
        {
            baseUrl.CheckIfNullOrWhiteSpace(nameof(baseUrl));
            relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));
            rawRelativeUrl.CheckIfNullOrWhiteSpace(nameof(rawRelativeUrl));

            return new LinkBuilder(baseUrl.Trim(), relLabel, rawRelativeUrl.Trim());
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="httpContext">The current HttpContext in an Web API controller.</param>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public static LinkBuilder AddLink(this HttpContext httpContext, string relLabel, string rawRelativeUrl)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var request = httpContext.Request;
            var baseurl = $"{request.Scheme}://{request.Host}/";

            return baseurl.AddLink(relLabel, rawRelativeUrl);
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

        public static LinkBuilder AddQueryLink(this string baseUrl, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return baseUrl.AddRouteLink(relLabel, relativeUrl).AddParameters(queryPairs);
        }

        public static LinkBuilder AddQueryLink(this HttpContext httpContext, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return httpContext.AddRouteLink(relLabel, relativeUrl).AddParameters(queryPairs);
        }

        public static LinkBuilder AddRouteLink(this HttpContext httpContext, string relLabel, params object[] routeItems)
        {
            if (routeItems == null) throw new ArgumentNullException(nameof(routeItems));
            if (routeItems.Any(ri => ri == null)) throw new ArgumentException("Collection cannot contain null elements.", nameof(routeItems));

            var output = String.Join('/', routeItems.Select(ri => ri.ToString().Trim()));

            return httpContext.AddLink(relLabel, output);
        }

        public static LinkBuilder AddRouteLink(this string baseUrl, string relLabel, params object[] routeItems)
        {
            if (routeItems == null) throw new ArgumentNullException(nameof(routeItems));
            if (routeItems.Any(ri => ri == null)) throw new ArgumentException("Collection cannot contain null elements.", nameof(routeItems));

            var output = String.Join('/', routeItems.Select(ri => ri.ToString().Trim()));

            return baseUrl.AddLink(relLabel, output);
        }

        // public static LinkBuilder AddRouteLink(this string baseUrl, string relLabel, string? relPathFormat = "", params object[] formatItems)
        // {
        //     if (String.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Parameter cannot be null, empty or whitespace.", nameof(baseUrl));
        //     if (String.IsNullOrWhiteSpace(relLabel)) throw new ArgumentException("Parameter cannot be null, empty or whitespace.", nameof(relLabel));

        //     var output = new LinkBuilder(baseUrl);

        //     return output.AddFormattedLink(relLabel, relPathFormat, formatItems);
        // }


        // public static LinkBuilder AddFormattedLinks(this string baseUrl, string rel, string format, IEnumerable<string> items)
        // {
        //     if (String.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Parameter cannot be null, empty or whitespace.", nameof(baseUrl));

        //     var output = new LinkBuilder(baseUrl);

        //     return output.AddFormattedLinks(rel, format, items);
        // }

        public static string ToSelfHref(this IEnumerable<Link> links)
        {
            return links.ToHref("self");
        }

        public static string ToHref(this IEnumerable<Link> links, string rel)
        {
            if (links == null) throw new ArgumentNullException(nameof(links));
            if (String.IsNullOrWhiteSpace(rel)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(rel));

            return links.Single(l => l.Rel == rel).Href;
        }

        // public static TResult[] ToNullFilteredArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        // {
        //     return source
        //         .Select(v => selector(v))
        //         .Where(v => v != null)
        //         .ToArray();
        // }

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

        // public static ArgumentException ToNullOrWhitespace(this string parameterName)
        // {
        //     return new ArgumentException("Parameter cannot be null, empty, or whitespace.", parameterName);
        // }

        public static string CheckIfNullOrWhiteSpace(this string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", parameterName);
            else
                return value.Trim();
        }
    }
}
