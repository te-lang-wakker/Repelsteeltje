using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Repelsteeltje
{
    [Serializable]
    public class Names
    {
        private readonly HashSet<Name> boys = new HashSet<Name>();
        private readonly HashSet<Name> girls = new HashSet<Name>();

        public IEnumerable<Name> Boys => boys.OrderBy(b => b);
        public IEnumerable<Name> Girls => girls.OrderBy(b => b);

        public void AddBoys(IEnumerable<Name> names)
        {
            foreach (var name in names)
            {
                AddBoy(name);
            }
        }
        public bool AddBoy(Name name) => boys.Add(name);


        public void AddGirls(IEnumerable<Name> names)
        {
            foreach (var name in names)
            {
                AddGirl(name);
            }
        }
        public bool AddGirl(Name name) => girls.Add(name);

        public static IEnumerable<Name> FromDirectory(DirectoryInfo directory, NameTypes types)
        {
            if (types.HasFlag(NameTypes.Boys))
            {
                foreach (var boy in ReadNames(new FileInfo(Path.Combine(directory.FullName, "boys.txt"))))
                {
                    yield return boy;
                }
            }
            if (types.HasFlag(NameTypes.Girls))
            {
                foreach(var girl in ReadNames(new FileInfo(Path.Combine(directory.FullName, "girls.txt"))))
                {
                    yield return girl;
                }
            }
        }

        public static IEnumerable<Name> ReadNames(FileInfo file)
        {
            if (file.Exists)
            {
                using (var reader = file.OpenText())
                {
                    string name;
                    while ((name = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            yield return name;
                        }
                    }
                }
            }
        }
    }
}
