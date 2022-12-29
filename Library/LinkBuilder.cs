using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeyerCorp.HateoasBuilder
{
    /// <summary>
    /// Object used to create a collection of HATEOAS links for API endpoints.
    /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lastIgnored"></param>
        /// <param name="baseUrl">Base URL</param>
        /// <exception cref="ArgumentException"><paramref name="baseUrl"/> cannot be null, empty, or whitespace.</exception>
        internal LinkBuilder(bool lastIgnored, string baseUrl)
        {
            BaseUrl = baseUrl.CheckIfNullOrWhiteSpace(nameof(baseUrl));
            LastIgnored = lastIgnored;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseUrl">Base URL</param>
        /// <param name="relLabel">Rel label for first link.</param>
        /// <param name="rawRelativeUrl">Relative URL for the first link.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="baseUrl"/> must not be null, empty, or whitespace.</exception>
        internal LinkBuilder(string baseUrl, string relLabel, string? rawRelativeUrl) : this(baseUrl) => RelHrefPairs.Add(relLabel, rawRelativeUrl);

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
        /// Set when the last link added was conditionally ignored so that any AddParameter calls made after this are ignored as well.
        /// </summary>
        [JsonIgnore]
        internal bool LastIgnored { get; set; } = false;

        /// <summary>
        /// Build all added links and yield as a collection of links.
        /// </summary>
        /// <param name="encode">URL encode output if true.</param>
        public IEnumerable<Link> Build(bool encode = false)
        {
            return RelHrefPairs.Select(p =>
            {
                var url = new StringBuilder();
                var relativeurl = p.Item2.GetUrl(encode);

                url.Append(baseUrl);
                if (!String.IsNullOrWhiteSpace(relativeurl)) url.AppendFormat("/{0}", relativeurl);

                return new Link(p.Item1, url.ToString());
            });
        }

        /// <summary>
        /// Build all added links and yield as a collection of links all of which are URL encoded.
        /// </summary>
        public IEnumerable<Link> BuildEncoded() => Build(true);

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>This LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public LinkBuilder AddLink(string relLabel, string rawRelativeUrl)
        {
            LastIgnored = false;
            RelHrefPairs.Add(relLabel, rawRelativeUrl);

            return this;
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="rawRelativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public LinkBuilder AddLink(bool condition, string relLabel, string rawRelativeUrl)
        {
            LastIgnored = !condition;

            return condition
                ? AddLink(relLabel, rawRelativeUrl)
                : this;
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="relativeUrl">The hypertext link indicating where more data can be found.</param>
        /// <param name="queryPairs">Values which will be concatenated as a query parameter list for the URL of the last link added.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        public LinkBuilder AddQueryLink(string relLabel, string relativeUrl, params object[] queryPairs)
        {
            return AddRouteLink(relLabel, relativeUrl).AddParameters(queryPairs);
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
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
        public LinkBuilder AddQueryLink(bool condition, string relLabel, string relativeUrl, params object[] queryPairs)
        {
            LastIgnored = !condition;

            return condition
                ? AddQueryLink(relLabel, relativeUrl, queryPairs)
                : this;
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
        /// <param name="relLabel">The label which will be used for the hyperlink.</param>
        /// <param name="routeItems">Items which will be concatenated with delimiting slashes to create a route after the base URL.</param>
        /// <returns>A LinkBuilder object which can be used to add more links before calling the Build method.</returns>
        /// <remarks>
        /// Use the <paramref name="condition"/> to decide whether to ignore adding this new link.
        /// For example, when considering pagination, you might consider checking whether you're on page one and so`you'll want
        /// to ignore adding the link when page 1. 
        /// <code>HttpContext.AddLink(page==1, "next",...);</code> 
        /// </remarks>
        public LinkBuilder AddRouteLink(string relLabel, params object[] routeItems)
        {
            if (routeItems == null) throw new ArgumentNullException(nameof(routeItems));
            if (routeItems.Length > 1 && routeItems.Any(ri => ri == null)) throw new ArgumentException($"No elements in the collection can be null.", nameof(routeItems));

            RelHrefPairs.AddRouteLink(relLabel, routeItems);

            return this;
        }

        /// <summary>
        /// Create a name and hyperlink pair based on the current HttpContext which can be added to an API's HTTP response.
        /// </summary>
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
        public LinkBuilder AddRouteLink(bool condition, string relLabel, params object[] routeItems)
        {
            LastIgnored = !condition;

            return condition
                ? AddRouteLink(relLabel, routeItems)
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

        /// <summary>
        /// Add a link based on a format string and necessary parameters
        /// </summary>
        /// <param name="relLabel"></param>
        /// <param name="relativeUrlFormat"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// <param name="queryPairs">Values which will be concatenated as a query parameter list for the URL of the last link added.</param>
        /// <returns>This Link Builder.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>This will be ignore and not run if the previous Add Link method that it run on was ignore because of a conditional parameter.</remarks>
        public LinkBuilder AddParameters(params object[] queryPairs)
        {
            if (!LastIgnored)
            {
                if (queryPairs == null) throw new ArgumentNullException(nameof(queryPairs));

                RelHrefPairs.Last().Item2.QueryItems.AddRange(queryPairs);
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