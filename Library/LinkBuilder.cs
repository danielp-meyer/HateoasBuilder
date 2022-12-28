using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeyerCorp.HateoasBuilder
{
    public class LinkBuilder
    {
        private string baseUrl = default!;

        [JsonIgnore]
        List<Tuple<string, LinkInformation>> RelHrefPairs { get; set; } = new List<Tuple<string, LinkInformation>>();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseUrl">The base URL which all presented links will use</param>
        /// <exception cref="ArgumentNullException">The <paramref name="baseUrl"/> must not be null, empty, or whitespace.</exception>
        internal LinkBuilder(string baseUrl) => BaseUrl = baseUrl.CheckIfNullOrWhiteSpace(nameof(baseUrl));

        internal LinkBuilder(bool lastIgnored, string baseUrl)
        {
            BaseUrl = baseUrl.CheckIfNullOrWhiteSpace(nameof(baseUrl));
            this.LastIgnored = lastIgnored;
        }

        internal LinkBuilder(string baseUrl, string relLabel, string? rawRelativeUrl) : this(baseUrl) => RelHrefPairs.Add(relLabel, rawRelativeUrl);

        // internal LinkBuilder(HttpContext httpContext) : this(httpContext.ToBaseUrl()) { }

        internal LinkBuilder(bool lastIgnored, HttpContext httpContext) : this(lastIgnored, httpContext.ToBaseUrl()) { }

        /// <summary>
        /// The base URL which all presented links will use
        /// </summary>
        [JsonIgnore]
        public string BaseUrl
        {
            get => baseUrl.Trim();
            private set => baseUrl = value;
        }

        [JsonIgnore]
        internal bool LastIgnored { get; set; } = false;

        /// <summary>
        /// Build all added links and yield as a collection of links.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="baseUrl"/> must not be null, empty, or whitespace.</exception>
        public IEnumerable<Link> Build(bool encode = false)
        {
            return RelHrefPairs.Select(p =>
            {
                var url = new StringBuilder();
                var relativeurl = p.Item2.GetUrl(encode);

                url.Append(baseUrl);
                if (String.IsNullOrWhiteSpace(relativeurl)) url.AppendFormat("/{0}", relativeurl);

                return new Link(p.Item1, url.ToString());
            });
        }

        /// <summary>
        /// Build all added links and yield as a collection of links all of which are URL encoded.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="baseUrl"/> must not be null, empty, or whitespace.</exception>
        public IEnumerable<Link> BuildEncoded() => Build(true);

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>This LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public LinkBuilder AddLink(string relLabel, string rawRelativeUrl)
        {
            var rel = relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));

            LastIgnored = false;
            RelHrefPairs.Add(rel, rawRelativeUrl);

            return this;
        }

        public LinkBuilder AddLink(bool condition, string relLabel, string rawRelativeUrl)
        {
            LastIgnored = !condition;

            return condition
                ? AddLink(relLabel, rawRelativeUrl)
                : this;
        }

        public LinkBuilder AddQueryLink(string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return AddRouteLink(relLabel, relativeUrl).AddParameters(queryPairs);
        }

        public LinkBuilder AddQueryLink(bool condition, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            LastIgnored = !condition;

            return condition
                ? AddQueryLink(relLabel, relativeUrl, relativeUrl, queryPairs)
                : this;
        }

        public LinkBuilder AddRouteLink(string relLabel, string relativeUrl, params object[] routeItems)
        {
            if (routeItems.Any(ri => ri == null)) throw new ArgumentException($"No elements in the collection can be null.", nameof(routeItems));

            var items = routeItems.Select(ri => ri?.ToString());
            var route = items.Count() > 0
                ? String.Join('/', items)
                : null;

            var url = String.IsNullOrWhiteSpace(relativeUrl)
                ? String.Empty
                : String.Concat(relativeUrl, '/');

            return AddLink(relLabel, String.Concat(relativeUrl, route));
        }

        public LinkBuilder AddRouteLink(bool condition, string relLabel, string relativeUrl, params object[] routeItems)
        {
            LastIgnored = !condition;

            return condition
                ? AddRouteLink(relLabel, relativeUrl, routeItems)
                : this;
        }

        /// <summary>
        /// Add a link based on a format string and necessary parameters
        /// </summary>
        /// <param name="relLabel"></param>
        /// <param name="relativeUrlFormat"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public LinkBuilder AddFormattedLink(string relLabel, string relativeUrlFormat, params object[] arguments)
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            relativeUrlFormat.CheckIfNullOrWhiteSpace(nameof(relativeUrlFormat));

            return AddLink(relLabel, String.Format(relativeUrlFormat, arguments));
        }

        public LinkBuilder AddFormattedLink(bool condition, string relLabel, string relativeUrlFormat, params object[] arguments)
        {
            LastIgnored = !condition;

            return condition
                 ? AddFormattedLink(relLabel, relativeUrlFormat, relativeUrlFormat, arguments)
                 : this;
        }

        /// <summary>
        /// Add a list of parameters to the end of the last URL added to the list of links.
        /// </summary>
        public LinkBuilder AddParameters(params object[] queryPairs)
        {
            if (!LastIgnored)
            {
                if (queryPairs == null) throw new ArgumentNullException(nameof(queryPairs));

                var query = new StringBuilder();

                for (var index = 0; index < queryPairs.Length; index += 2)
                {
                    var first = queryPairs[index]?.ToString().Trim();
                    var second = queryPairs.Length > index + 1
                        ? queryPairs[index + 1]?.ToString().Trim()
                        : null;

                    query.Append($"{first}={second}&");
                }

                var queries = query.ToString().Length > 0
                    ? query.ToString().Trim('&')
                    : String.Empty;

                if (RelHrefPairs.Count < 1)
                    throw new InvalidOperationException("At least one link must be added before query parameters can be added.");
                else
                {
                    var rel = RelHrefPairs.Last().Item1;
                    var href = RelHrefPairs.Last().Item2;

                    RelHrefPairs.RemoveAt(RelHrefPairs.Count - 1);

                    AddLink(rel, String.Concat(href, '?', queries));
                }
            }

            return this;
        }

        // /// <summary>
        // /// Add a link to an external site
        // /// </summary>
        // /// <param name="baseUrl">Base URL of the external site</param>
        // /// <param name="relLabel">Link label</param>
        // /// <param name="relPathFormat">Link relative path or relative path template</param>
        // /// <param name="formatItems">Template items</param>
        // /// <returns>This link builder w/ the <paramref name="relLabel"/> and <paramref name="relPathFormat"/> result added to the RelHrefPairs colleciton property</returns>
        // /// <exception cref="ArgumentNullException"><paramref name="relLabel"/> cannot be null, empty, or whitespace</exception>
        // /// <exception cref="ArgumentException"><paramref name="relPathFormat"/> can be null, empty, or whitespace only when the <paramref name="formatItems"/>parameter is empty</exception>
        // public LinkBuilder AddLinkExternal(string baseUrl, string relLabel, string relPathFormat, params object[] formatItems)
        // {
        //     if (String.IsNullOrWhiteSpace(relLabel)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(relLabel));
        //     if (formatItems.Length > 0 && String.IsNullOrWhiteSpace(relPathFormat))
        //         throw new ArgumentException(nameof(relLabel), "Parameter cannot be null, empty, or whitespace.");

        //     RelHrefPairs.Add(relLabel);
        //     RelHrefPairs.Add(String.Concat(baseUrl.TrimEnd('/'), '/', String.Format(relPathFormat, formatItems)));

        //     return this;
        // }

        // public LinkBuilder AddFormattedLinks(string relLabel, string relPathFormat, IEnumerable<string> items)
        // {
        //     if (String.IsNullOrWhiteSpace(relLabel)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(relLabel));
        //     if (String.IsNullOrWhiteSpace(relPathFormat)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(relPathFormat));
        //     if (items == null) throw new ArgumentNullException(nameof(relPathFormat));

        //     if (!String.IsNullOrWhiteSpace(relPathFormat) && items != null)
        //     {
        //         foreach (var item in items.Where(i => i != null && !String.IsNullOrWhiteSpace(i.ToString())))
        //         {
        //             var formatitems = item.Split(',');

        //             AddFormattedLink(relLabel, relPathFormat, formatitems);
        //         }
        //     }

        //     return this;
        // }
    }
}