using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Repelsteeltje
{
    public struct Veto
    {
        private static readonly Regex VetoFilePattern = new Regex(@"^.+\.(boys|girls)\.vetos\.txt$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string value;

        private Veto(string v) => value = v?.ToUpperInvariant();

        public bool IsRegex
        {
            get
            {
                return !string.IsNullOrEmpty(value) &&
                    value.Length > 2 &&
                    value[0] == '/' &&
                    value[value.Length - 1] == '/';
            }
        }

        public Regex Pattern
        {
            get
            {
                if (IsRegex)
                {
                    return new Regex(value.Substring(1, value.Length - 2), RegexOptions.IgnoreCase);
                }
                else
                {
                    return new Regex("^" + value + "$", RegexOptions.IgnoreCase);
                }
            }
        }

        public override string ToString() => value ?? string.Empty;

        public static implicit operator Veto(string s) => new Veto(s);
        public static implicit operator Veto(Name name) => new Veto(name);
        public static explicit operator Name(Veto veto) => new Name(veto.value);

        public static bool IsFile(FileInfo file) => VetoFilePattern.IsMatch(file.Name);
    }
}
