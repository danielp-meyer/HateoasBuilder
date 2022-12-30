using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeyerCorp.HateoasBuilder
{
    internal class LinkInformation 
    {
        internal bool IsExternal { get; }
        readonly string RelativeUrl = String.Empty;

        internal LinkInformation(string? rawRelativeUrl)
        {
            RelativeUrl = rawRelativeUrl?.Trim() ?? String.Empty;
        }

        internal LinkInformation(IEnumerable<object>? routeItems, IEnumerable<object>? queryItems)
        {
            if (routeItems != null) RouteItems.AddRange(routeItems);
            if (queryItems != null) QueryItems.AddRange(queryItems);
        }

        public LinkInformation(string? rawRelativeUrl, bool isExternal) : this(rawRelativeUrl)
        {
            IsExternal = isExternal;
        }

        internal List<object> RouteItems { get; } = new List<object>();
        internal List<object> QueryItems { get; } = new List<object>();

        internal string GetUrl(bool encode)
        {
            if (RouteItems.Count == 0 && QueryItems.Count == 0)
                return RelativeUrl;
            else
            {
                var routes = RouteItems
                    .Where(ri => !String.IsNullOrWhiteSpace(ri?.ToString()))
                    .Select(i => i?.ToString());

                var queries = Concatenate(QueryItems);

                var route = string.Join('/', routes);
                var output = new StringBuilder();

                output.Append(RelativeUrl.Trim());
                output.Append(route.Trim());
                if (!string.IsNullOrWhiteSpace(queries)) output.AppendFormat("?{0}", queries);

                return encode
                    ? System.Web.HttpUtility.UrlEncode(output.ToString())
                    : output.ToString();
            }
        }

        static string Concatenate(IEnumerable<object> queryItems)
        {
            if (queryItems == null) throw new ArgumentNullException(nameof(queryItems));

            var output = new StringBuilder();
            var items = queryItems.ToArray();

            for (var index = 0; index < queryItems.Count(); index += 2)
            {
                var name = items[index]?.ToString().Trim();
                var value = items.Length > index + 1
                     ? items[index + 1]?.ToString().Trim() ?? String.Empty
                     : String.Empty;

                if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Parameter names cannot be null, empty, or whitespace.");

                output.AppendFormat("{0}={1}&", name, value);
            }

            return output.ToString().Trim('&');
        }
    }
}