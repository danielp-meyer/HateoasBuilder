using System;
using System.Collections.Generic;
using System.Text;

namespace MeyerCorp.HateoasBuilder
{
    internal class LinkInformation
    {
        readonly string RelativeUrl = String.Empty;

        internal LinkInformation(string? rawRelativeUrl)
        {
            RelativeUrl = rawRelativeUrl ?? String.Empty;
        }

        internal List<string> RouteItems { get; } = new List<string>();
        internal List<string> QueryItems { get; } = new List<string>();

        internal string GetUrl(bool encode)
        {
            if (RouteItems.Count == 0 && QueryItems.Count == 0)
                return RelativeUrl.Trim();
            else
            {
                var route = string.Join('/', RouteItems);
                var query = string.Join('&', QueryItems);
                var output = new StringBuilder();

                output.Append(route);
                if (string.IsNullOrWhiteSpace(query)) output.AppendFormat("?{0}", query);

                return encode
                    ? System.Web.HttpUtility.UrlEncode(output.ToString())
                    : output.ToString();
            }
        }
    }
}