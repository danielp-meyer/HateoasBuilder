using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeyerCorp.HateoasBuilder
{
    public class LinkBuilder
    {
        private string? baseUrl;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseUrl">The base URL which all presented links will use</param>
        /// <exception cref="ArgumentNullException">The <paramref name="baseUrl"/> must not be null, empty or whitespace.</exception>
        public LinkBuilder(string baseUrl)
        {
            if (String.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));
            else
                BaseUrl = baseUrl;
        }

        /// <summary>
        /// Link colleciton indexer
        /// </summary>
        /// <param name="index">Index which to retrieve.</param>
        /// <returns>Link object in the collection to return</returns>
        [JsonProperty]
        public Link this[int index] { get { return Build().ToArray()[index]; } }

        /// <summary>
        /// The base URL which all presented links will use
        /// </summary>
        [JsonIgnore]
        public string BaseUrl
        {
            get => $"{baseUrl}/";
            private set => baseUrl = value.TrimEnd('/');
        }


        [JsonIgnore]
        List<string> RelHrefPairs { get; set; } = new List<string>();

        /// <summary>
        /// Add a link to an external site
        /// </summary>
        /// <param name="baseUrl">Base URL of the external site</param>
        /// <param name="relLabel">Link label</param>
        /// <param name="relPathFormat">Link relative path or relative path template</param>
        /// <param name="formatItems">Template items</param>
        /// <returns>This link builder w/ the <paramref name="relLabel"/> and <paramref name="relPathFormat"/> result added to the RelHrefPairs colleciton property</returns>
        /// <exception cref="ArgumentNullException"><paramref name="relLabel"/> cannot be null, empty or whitespace</exception>
        /// <exception cref="ArgumentException"><paramref name="relPathFormat"/> can be null, empty or whitespace only when the <paramref name="formatItems"/>parameter is empty</exception>
        public LinkBuilder AddLinkExternal(string baseUrl, string relLabel, string relPathFormat, params object[] formatItems)
        {
            if (String.IsNullOrWhiteSpace(relLabel)) throw new ArgumentNullException(nameof(relLabel), "Parameter cannot be null, empty or whitespace.");
            if (formatItems.Length > 0 && String.IsNullOrWhiteSpace(relPathFormat))
                throw new ArgumentException(nameof(relLabel), "Parameter cannot be null, empty or whitespace.");

            RelHrefPairs.Add(relLabel);
            RelHrefPairs.Add(String.Concat(baseUrl.TrimEnd('/'), '/', String.Format(relPathFormat, formatItems)));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relLabel"></param>
        /// <param name="relPathFormat"></param>
        /// <param name="formatItems"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public LinkBuilder AddFormattedLink(string? relLabel, string? relPathFormat, params object[] formatItems)
        {
            if (String.IsNullOrWhiteSpace(relPathFormat)) throw new ArgumentNullException(nameof(relPathFormat), "Parameter cannot be null, empty or whitespace.");

            if (formatItems.Length < 1 && !String.IsNullOrWhiteSpace(relPathFormat))
            {
                RelHrefPairs.Add(relLabel);
                RelHrefPairs.Add(String.Concat(BaseUrl, relPathFormat));
            }
            else if (formatItems.Length > 0 && !formatItems.Any(i => i == null || String.IsNullOrWhiteSpace(i.ToString())))
            {
                RelHrefPairs.Add(relLabel);
                RelHrefPairs.Add(String.Concat(BaseUrl, String.Format(relPathFormat, formatItems)));
            }

            return this;
        }

        public LinkBuilder AddFormattedLinkIf(bool condition, string? relLabel, string? relPathFormat, params object[] formatItems)
        {
            if (condition)
                AddFormattedLink(relLabel, relPathFormat, formatItems);

            return this;
        }

        public LinkBuilder AddFormattedLinks(string relLabel, string relPathFormat, IEnumerable<string> items)
        {
            if (!String.IsNullOrWhiteSpace(relPathFormat) && items != null)
            {
                foreach (var item in items.Where(i => i != null && !String.IsNullOrWhiteSpace(i.ToString())))
                {
                    var formatitems = item.Split(',');

                    AddFormattedLink(relLabel, relPathFormat, formatitems);
                }
            }

            return this;
        }

        public LinkBuilder AddQueryLink(string relLabel, params object[] queryPairs)
        {
            var relPathFormat = new StringBuilder();

            for (var index = 0; index < queryPairs.Length; index += 2)
            {
                relPathFormat.Append($"{queryPairs[index].ToString().Trim()}={queryPairs[index + 1]?.ToString().Trim()}&");
            }

            var queries = relPathFormat.ToString().Trim().Length > 0
                ? relPathFormat.ToString().Trim('&')
                : String.Empty;

            AddFormattedLink(relLabel, "{0}", String.Concat(relLabel?.Trim(), '?', queries));

            return this;
        }

        /// <summary>
        /// Build all added links and yield as a collection of links.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="baseUrl"/> must not be null, empty or whitespace.</exception>
        public IEnumerable<Link> Build()
        {
            var output = new List<Link>();

            for (var index = 0; index < RelHrefPairs.Count - 1; index += 2)
            {
                output.Add(new Link
                {
                    Rel = RelHrefPairs[index].Trim(),
                    Href = String.Concat(RelHrefPairs[index + 1].ToString().Trim()),
                });
            }

            return output;
        }
    }
}