using System;
using Optional;

namespace JDict
{
    public struct PriorityTag
    {
        private int raw;

        private Kind kind;

        private PriorityTag(int raw, Kind kind)
        {
            this.raw = raw;
            this.kind = kind;
        }

        private enum Kind
        {
            none,
            news,
            ichi,
            spec,
            gai,
            nf
        }

        public int? CompareTo(PriorityTag other)
        {
            if (this.kind == Kind.none && other.kind != Kind.none)
            {
                return -1;
            }
            if (this.kind != Kind.none && other.kind == Kind.none)
            {
                return 1;
            }

            if (this.kind != other.kind)
                return null;

            return this.raw.CompareTo(other.raw);
        }

        public static PriorityTag News1 { get; } = new PriorityTag(1, Kind.news);

        public static PriorityTag News2 { get; } = new PriorityTag(2, Kind.news);

        public static PriorityTag Ichi1 { get; } = new PriorityTag(1, Kind.ichi);

        public static PriorityTag Ichi2 { get; } = new PriorityTag(2, Kind.ichi);

        public static PriorityTag Spec1 { get; } = new PriorityTag(1, Kind.spec);

        public static PriorityTag Spec2 { get; } = new PriorityTag(2, Kind.spec);

        public static PriorityTag Gai1 { get; } = new PriorityTag(1, Kind.gai);

        public static PriorityTag Gai2 { get; } = new PriorityTag(2, Kind.gai);

        public static PriorityTag Nf(int rating) => new PriorityTag(rating, Kind.nf);

        public static Option<PriorityTag> FromString(string str)
        {
            if (TryParse(str, out var tag))
            {
                return tag.Some();
            }

            return Option.None<PriorityTag>();


            bool TryParse(string s, out PriorityTag t)
            {
                var firstDigitIndex = s.IndexOfAny(new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});
                if (firstDigitIndex == -1)
                {
                    t = default;
                    return false;
                }

                if (Enum.TryParse<Kind>(s.Substring(0, firstDigitIndex), out var kind) &&
                    int.TryParse(s.Substring(firstDigitIndex), out var value))
                {
                    t = new PriorityTag(value, kind);
                    return true;
                }

                t = default;
                return false;
            }
        }

        public override string ToString()
        {
            if (kind == Kind.nf)
            {
                return $"{kind}{raw:D2}";
            }

            return $"{kind}{raw}";
        }
    }
}