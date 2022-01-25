using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeyerCorp.HateoasBuilder
{
    public static class Extensions
    {
        public static LinkBuilder AddLink(this HttpContext httpContext, string relLabel, string? relPathFormat, params object[] formatItems)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var request =httpContext.Request;
            var baseurl = $"{request.Scheme}://{request.Host}/";


            return baseurl.AddLink(relLabel, relPathFormat, formatItems);
        }

        //public static LinkBuilder AddLink(this HttpContext httpContext, string relLabel, string? relPathFormat, params object[] formatItems)
        //{
        //    if (String.IsNullOrWhiteSpace(relLabel)) throw new ArgumentException("A link type label must be set.", nameof(relLabel));
        //    if (String.IsNullOrWhiteSpace(relPath)) throw new ArgumentException("A link path must be set.", nameof(relPathFormat));

        //    return httpContext
        //            .Request
        //            .PathBase
        //            .Value
        //            .AddLink(relLabel, relPathFormat, formatItems);
        //}

        public static LinkBuilder AddLink(this string baseUrl, string relLabel, string? relPathFormat, params object[] formatItems)
        {
            if (String.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Parameter cannot be null, empty or whitespace.", nameof(baseUrl));

            var output = new LinkBuilder(baseUrl);

            return output.AddLink(relLabel, relPathFormat, formatItems);
        }

        public static LinkBuilder AddLinks(this string baseUrl, string rel, string format, IEnumerable<string> items)
        {
            if (String.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Parameter cannot be null, empty or whitespace.", nameof(baseUrl));

            var output = new LinkBuilder(baseUrl);

            return output.AddLinks(rel, format, items);
        }

        public static string ToSelfHref(this IEnumerable<Link> links)
        {
            return links.ToHref("self");
        }

        public static string ToHref(this IEnumerable<Link> links, string rel)
        {
            return links.SingleOrDefault(l => l.Rel == rel).Href;
        }

        public static TResult[] ToNullFilteredArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source
                .Select(v => selector(v))
                .Where(v => v != null)
                .ToArray();
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
