using System.ComponentModel.DataAnnotations;

namespace MeyerCorp.HateoasBuilder
{
    public class Link
    {
        [Required]
        public string Rel { get; set; } = default!;

        [Required]
        public string Href { get; set; } = default!;

        public override bool Equals(object obj)
        {
            var value = obj as Link;

            if (value == null)
                return false;
            else
                return
                    value.Rel == Rel
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