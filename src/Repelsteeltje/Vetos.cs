using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Repelsteeltje
{
    public static class Vetos
    {
        public static FileInfo GetFile(DirectoryInfo root, NameTypes tp, string id)
        {
            var fileName = string.Format("{0}.{1}.vetos.txt", id, tp.ToString().ToLowerInvariant());
            return new FileInfo(Path.Combine(root.FullName, fileName));
        }

        public static IEnumerable<Veto> Load(string file) => Load(new FileInfo(file));

        public static IEnumerable<Veto> Load(FileInfo file)
        {
            if (!file.Exists) { return Enumerable.Empty<Veto>(); }
            return Load(file.OpenRead());
        }
        public static IEnumerable<Veto> Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        yield return line;
                    }

                }
            }
        }

        public static void Save(string file, IEnumerable<Name> names) => Save(new FileInfo(file), names);

        public static void Save(FileInfo file, IEnumerable<Name> names) => Save(new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.Write), names);

        public static void Save(Stream stream, IEnumerable<Name> names)
        {
            using (var writer = new StreamWriter(stream))
            {
                foreach (var name in names.OrderBy(n => n))
                {
                    writer.WriteLine(name);
                }
            }
        }
    }
}
