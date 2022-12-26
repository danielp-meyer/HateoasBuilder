using System.ComponentModel.DataAnnotations;

namespace MeyerCorp.HateoasBuilder
{
    public class Link
    {
        [Required]
        public string Rel { get; } = default!;

        [Required]
        public string Href { get; } = default!;

        public Link(string relLabel, string href)
        {
            Rel = relLabel;
            Href = href;
        }

        public override bool Equals(object obj)
        {
            var value = obj as Link;

            if (value == null)
                return false;
            else
                return value.Rel == Rel
                    && value.Href == Href;
        }

        public override int GetHashCode()
        {
            return 17
                .HashThis(Rel)
                .HashThis(Href);
        }

        public override string ToString()
        {
            return $"{Rel}: {Href}";
        }
    }
}