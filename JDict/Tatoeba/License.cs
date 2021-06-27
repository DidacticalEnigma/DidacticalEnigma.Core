using System;

namespace JDict
{
    public class License : IEquatable<License>
    {
        public string Identifier { get; }

        public override string ToString()
        {
            return Identifier;
        }

        private License(string identifier)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }

        public License Create(string identifier)
        {
            return new License(identifier);
        }

        public bool Equals(License other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Identifier, other.Identifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((License) obj);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public static bool operator ==(License left, License right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(License left, License right)
        {
            return !Equals(left, right);
        }

        // Attribution 4.0 International (CC BY 4.0) 
        // https://creativecommons.org/licenses/by/4.0/
        public static readonly License CcBy40 = new License("CC BY 4.0");

        // Attribution-NonCommercial 4.0 International (CC BY-NC 4.0) 
        // https://creativecommons.org/licenses/by-nc/4.0/
        public static readonly License CcByNc40 = new License("CC BY-NC 4.0");

        // Attribution-NonCommercial-NoDerivs 3.0 Unported (CC BY-NC-ND 3.0) 
        // https://creativecommons.org/licenses/by-nc-nd/3.0/
        public static readonly License CcByNcNd30 = new License("CC BY-NC-ND 3.0");

        // Attribution-ShareAlike 4.0 International (CC BY-SA 4.0) 
        // https://creativecommons.org/licenses/by-sa/4.0/
        public static readonly License CcBySa40 = new License("CC BY-SA 4.0");

        public static License Of(string s)
        {
            return new License(s);
        }
    }
}