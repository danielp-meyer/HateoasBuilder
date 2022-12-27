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
        List<Tuple<string, string?>> RelHrefPairs { get; set; } = new List<Tuple<string, string?>>();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseUrl">The base URL which all presented links will use</param>
        /// <exception cref="ArgumentNullException">The <paramref name="baseUrl"/> must not be null, empty, or whitespace.</exception>
        public LinkBuilder(string baseUrl) => BaseUrl = baseUrl.CheckIfNullOrWhiteSpace(nameof(baseUrl));

        public LinkBuilder(string baseUrl, string relLabel, string? rawRelativeUrl) : this(baseUrl) => RelHrefPairs.Add(relLabel, rawRelativeUrl);

        // /// <summary>
        // /// Link colleciton indexer
        // /// </summary>
        // /// <param name="index">Index which to retrieve.</param>
        // /// <returns>Link object in the collection to return</returns>
        // [JsonProperty]
        // public Link this[int index] { get { return Build().ToArray()[index]; } }

        /// <summary>
        /// The base URL which all presented links will use
        /// </summary>
        [JsonIgnore]
        public string BaseUrl
        {
            get => baseUrl.Trim();
            private set => baseUrl = value;
        }

        /// <summary>
        /// Build all added links and yield as a collection of links.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="baseUrl"/> must not be null, empty, or whitespace.</exception>
        public IEnumerable<Link> Build(bool encode = false)
        {
            return RelHrefPairs
                .Select(p =>
                {
                    var relativeurl = encode
                        ? System.Web.HttpUtility.UrlEncode(p.Item2?.Trim())
                        : p.Item2?.Trim();
                    var href = String.Concat(BaseUrl, '/', p.Item2?.Trim()).Trim('/');

                    return new Link(p.Item1, href);
                });
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>This LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public LinkBuilder AddLink(string relLabel, string rawRelativeUrl)
        {
            relLabel.CheckIfNullOrWhiteSpace(nameof(relLabel));
            rawRelativeUrl.CheckIfNullOrWhiteSpace(nameof(rawRelativeUrl));

            RelHrefPairs.Add(relLabel, rawRelativeUrl);

            return this;
        }

        /// <summary>
        /// Add a link based on a format string and necessary parameters
        /// </summary>
        /// <param name="relLabel"></param>
        /// <param name="relativeUrlFormat"></param>
        /// <param name="formattedItems"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public LinkBuilder AddFormattedLink(string relLabel, string relativeUrlFormat, params object[] formattedItems)
        {
            if (formattedItems == null) throw new ArgumentNullException(nameof(formattedItems));
            relativeUrlFormat.CheckIfNullOrWhiteSpace(nameof(relativeUrlFormat));

            return AddLink(relLabel, String.Format(relativeUrlFormat, formattedItems));
        }

        public LinkBuilder AddQueryLink(string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return AddRouteLink(relLabel, relativeUrl).AddParameters(queryPairs);
        }

        public LinkBuilder AddRouteLink(string relLabel, string relativeUrl, params object[] routeItems)
        {
            if (routeItems.Any(ri => ri == null)) throw new ArgumentException($"No elements in the collection can be null.", nameof(routeItems));

            var items = routeItems.Select(ri => ri?.ToString());
            var route = items.Count() > 0
                ? String.Join('/', items)
                : null;

            return AddLink(relLabel, String.Concat(relativeUrl, '/', route));
        }

        /// <summary>
        /// Add a list of parameters to the end of the last URL added to the list of links.
        /// </summary>
        public LinkBuilder AddParameters(params object[] queryPairs)
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

        // /// <summary>
        // /// Add a link based on a format string and necessary parameters
        // /// </summary>
        // /// <param name="relLabel"></param>
        // /// <param name="relativeUrlFormat"></param>
        // /// <param name="formattedItems"></param>
        // /// <returns></returns>
        // /// <exception cref="ArgumentNullException"></exception>
        // public LinkBuilder AddFormattedLink(string relLabel, string? relPathFormat = "", params object[] formatItems)
        // {
        //     if (String.IsNullOrWhiteSpace(relLabel)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(relLabel));
        //     if (String.IsNullOrWhiteSpace(relativeUrlFormat)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(relativeUrlFormat));

        //     if (formattedItems.Length < 1 && !String.IsNullOrWhiteSpace(relativeUrlFormat))
        //     {
        //         RelHrefPairs.Add(relLabel);
        //         RelHrefPairs.Add(String.Concat(BaseUrl, relativeUrlFormat));
        //     }
        //     else if (formattedItems.Length > 0 && !formattedItems.Any(i => i == null || String.IsNullOrWhiteSpace(i.ToString())))
        //     {
        //         RelHrefPairs.Add(relLabel);
        //         RelHrefPairs.Add(String.Concat(BaseUrl, String.Format(relativeUrlFormat, formattedItems)));
        //     }

        //     return this;
    }

    // public LinkBuilder AddFormattedLinkIf(bool condition, string relLabel, string relPathFormat, params object[] formatItems)
    // {
    //     if (String.IsNullOrWhiteSpace(relLabel)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(relLabel));
    //     if (String.IsNullOrWhiteSpace(relPathFormat)) throw new ArgumentException("Parameter cannot be null, empty, or whitespace.", nameof(relLabel));

    //     if (condition)
    //         AddFormattedLink(relLabel, relPathFormat, formatItems);

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