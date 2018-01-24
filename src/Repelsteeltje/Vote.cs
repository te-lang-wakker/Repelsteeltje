using Qowaiv;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Repelsteeltje
{
    [Serializable]
    public class Vote
    {
        private static readonly Regex VoteFilePattern = new Regex(@"^.+\.(boys|girls)\.xml$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("left")]
        public string Left { get; set; }

        [XmlAttribute("right")]
        public string Right { get; set; }

        [XmlAttribute("w")]
        public int Win { get; set; }
        [XmlAttribute("d")]
        public int Draw { get; set; }

        [XmlAttribute("l")]
        public int Loss { get; set; }

        public int Total => Win + Draw + Loss;

        public bool Contains(Name name) => name.Display == Left || name.Display == Right;

        public void Wins(Name name)
        {
            if (name == Left)
            {
                Win++;
            }
            else
            {
                Loss++;
            }
        }

        public Percentage Score => Total == 0 ? Percentage.Zero : (Win + Draw * 0.5) / Total;

        public static bool IsFile(FileInfo file) => VoteFilePattern.IsMatch(file.Name);
    }
}
