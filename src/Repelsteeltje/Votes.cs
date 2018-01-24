using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Repelsteeltje
{
    [Serializable]
    public class Votes : List<Vote>
    {
        public Votes() { }
        public Votes(IEnumerable<Vote> collection) : base(collection) { }

        public Vote Select(Name l, Name r, string id)
        {
            var compare = l.CompareTo(r);
            var left = compare < 0 ? r : l;
            var right = compare < 0 ? l : r;

            var vote = this.FirstOrDefault(v => (Name)v.Left == left && (Name)v.Right == right && v.Id == id);

            if (vote == null)
            {
                vote = new Vote()
                {
                    Id = id,
                    Left = left,
                    Right = right,
                };
                Add(vote);
            }
            return vote;
        }

        /// <summary>Saves the <see cref="Votes"/> to a file.</summary>
        /// <param name="file">
        /// The file to safe to.
        /// </param>
        public void Save(string file) => Save(new FileInfo(file));

        /// <summary>Saves the <see cref="Votes"/> to a file.</summary>
        /// <param name="file">
        /// The file to safe to.
        /// </param>
        public void Save(FileInfo file)
        {
            using (var stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
            {
                Save(stream);
            }
        }

        /// <summary>Gets all <see cref="Votes"/> with a specific ID.</summary>
        public Votes Select(string id) => new Votes(this.Where(vote => vote.Id == id));

        /// <summary>Saves the <see cref="Votes"/> to a stream.</summary>
        /// <param name="stream">
        /// The stream to safe to.
        /// </param>
        public void Save(Stream stream) => serializer.Serialize(stream, this);

        /// <summary>Loads a <see cref="Votes"/> from stream.</summary>
        /// <param name="stream">
        /// The stream to load from.
        /// </param>
        public static Votes Load(Stream stream) => (Votes)serializer.Deserialize(stream);

        /// <summary>Loads a <see cref="Votes"/> from stream.</summary>
        /// <param name="file">
        /// The file to load from.
        /// </param>
        public static Votes Load(FileInfo file)
        {
            if (!file.Exists) { return new Votes(); }
            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                return Load(stream);
            }
        }
        /// <summary>Loads a <see cref="Votes"/> from stream.</summary>
        /// <param name="file">
        /// The file to load from.
        /// </param>
        public static Votes Load(string file) => Load(new FileInfo(file));

        /// <summary>The <see cref="XmlSerializer"/> to load and save the <see cref="Votes"/>.</summary>
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Votes));
    }
}
