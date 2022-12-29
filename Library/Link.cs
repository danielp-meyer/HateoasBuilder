using System.ComponentModel.DataAnnotations;

namespace MeyerCorp.HateoasBuilder
{
    public class Link
    {
        const string GET = "GET";

        [Required]
        public string Rel { get; } = default!;

        [Required]
        public string Href { get; } = default!;

        public string Method { get; } = GET;

        public Link(string relLabel, string href, string method = GET)
        {
            Rel = relLabel;
            Href = href;
            Method = method;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Link value))
                return false;
            else
                return value.Rel == Rel
                    && value.Href == Href
                    && value.Method == Method;
        }

        public override int GetHashCode()
        {
            return 17
                .HashThis(Rel)
                .HashThis(Href);
        }

        public override string ToString()
        {
            return $"{Rel}-> {Method}: {Href}";
        }
    }
}