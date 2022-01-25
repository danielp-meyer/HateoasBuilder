using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeyerCorp.HateoasBuilder
{
    public class LinkBuilder
    {
        public LinkBuilder(string baseUrl) { BaseUrl = baseUrl; }

        [JsonProperty]
        public Link this[int index] { get { return GetLinks().ToArray()[index]; } }

        [JsonIgnore]
        public string BaseUrl { get; private set; }

        [JsonIgnore]
        List<string> RelHrefPairs { get; set; } = new List<string>();

        public LinkBuilder AddLinkExternal(string baseUrl, string rel, string format, params object[] formatItems)
        {
            if (String.IsNullOrWhiteSpace(rel)) throw new ArgumentNullException(nameof(rel), "Parameter cannot be null, empty or whitespace.");
            if (String.IsNullOrWhiteSpace(format)) throw new ArgumentNullException(nameof(rel), "Parameter cannot be null, empty or whitespace.");

            if (formatItems != null && formatItems.Length > 0 && !formatItems.Any(i => i == null || String.IsNullOrWhiteSpace(i.ToString())))
            {
                RelHrefPairs.Add(rel);
                RelHrefPairs.Add(String.Concat(baseUrl.TrimEnd('/'), '/', String.Format(format, formatItems)));
            }

            return this;
        }

        public LinkBuilder AddLink(string? rel, string? format, params object[] formatItems)
        {
            if (String.IsNullOrWhiteSpace(rel)) throw new ArgumentNullException(nameof(rel), "Parameter cannot be null, empty or whitespace.");
            if (String.IsNullOrWhiteSpace(format)) throw new ArgumentNullException(nameof(format), "Parameter cannot be null, empty or whitespace.");

            if (formatItems.Length < 1 && !String.IsNullOrWhiteSpace(format))
            {
                RelHrefPairs.Add(rel);
                RelHrefPairs.Add(String.Concat(BaseUrl.TrimEnd('/'), '/', format));
            }
            else if (formatItems.Length > 0 && !formatItems.Any(i => i == null || String.IsNullOrWhiteSpace(i.ToString())))
            {
                RelHrefPairs.Add(rel);
                RelHrefPairs.Add(String.Concat(BaseUrl.TrimEnd('/'), '/', String.Format(format, formatItems)));
            }

            return this;
        }

        public LinkBuilder AddLinks(string rel, string format, IEnumerable<string> items)
        {
            if (!String.IsNullOrWhiteSpace(format) && items != null)
            {
                foreach (var item in items.Where(i => i != null && !String.IsNullOrWhiteSpace(i.ToString())))
                {
                    var formatitems = item.Split(',');

                    AddLink(rel, format, formatitems);
                }
            }

            return this;
        }

        public IEnumerable<Link> GetLinks()
        {
            var output = new List<Link>();

            for (var index = 0; index < RelHrefPairs.Count - 1; index += 2)
            {
                output.Add(new Link
                {
                    Rel = RelHrefPairs[index],
                    Href = String.Concat(RelHrefPairs[index + 1].ToString()),
                });
            }

            return output;
        }
    }
}