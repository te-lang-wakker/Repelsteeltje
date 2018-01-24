using Qowaiv.Formatting;
using System;
using System.Diagnostics;

namespace Repelsteeltje
{
    [DebuggerDisplay("{Display}")]
    public struct Name : IComparable<Name>
    {
        private readonly string value;

        public Name(string str)
        {
            str = StringFormatter.ToNonDiacritic((str ?? string.Empty).ToUpperInvariant().Trim());
            foreach (var ch in "- .,")
            {
                str = str.Replace(ch.ToString(), "");
            }
            value = str;
        }

        public bool IsEmpty() => string.IsNullOrEmpty(value);

        public string Display
        {
            get
            {
                if (string.IsNullOrEmpty(value)) { return string.Empty; }

                var first = value.Substring(0, 1).ToUpperInvariant();
                var rest = value.Substring(1).ToLowerInvariant();
                return first + rest;
            }
        }
        public override string ToString() => value;

        public static implicit operator Name(string str) => new Name(str);
        public static implicit operator string(Name id) => id.ToString();
        public override int GetHashCode() => value == null ? 0 : value.GetHashCode();
        public override bool Equals(object obj) => base.Equals(obj);

        public int CompareTo(Name other) => string.CompareOrdinal(value, other.value);
    }
}
