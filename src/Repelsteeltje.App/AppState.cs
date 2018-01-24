using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace Repelsteeltje.App
{
    public class AppState
    {
        public AppState(string id, DirectoryInfo location, NameTypes nameTypes)
        {
            Id = string.IsNullOrWhiteSpace(id) ? "Unknown" : id;
            Location = location ?? new DirectoryInfo(".");
            NameTypes = nameTypes;
            CurrentNameType = nameTypes.IsSingleGender() ? nameTypes : NameTypes.Boys;
            Rnd = new MT19937Generator();
            OwnBoyVetos = new List<Name>(Vetos.Load(Path.Combine(Location.FullName, Id + ".boys.vetos.txt")).Select(n => (Name)n));
            OwnGirlVetos = new List<Name>(Vetos.Load(Path.Combine(Location.FullName, Id + ".girls.vetos.txt")).Select(n => (Name)n));
        }

        public string Id { get; }
        public DirectoryInfo Location { get; }
        public NameTypes NameTypes { get; }

        public Ranks Boys { get; set; }
        public Ranks Girls { get; set; }
        public Ranks this[NameTypes nameType]
        {
            get
            {
                switch (nameType)
                {
                    case NameTypes.Boys: return GetRanks(NameTypes.Boys);
                    case NameTypes.Girls: return GetRanks(NameTypes.Girls);
                    default: return null;
                }
            }
            private set
            {
                switch (nameType)
                {
                    case NameTypes.Boys: Boys = value; break;
                    case NameTypes.Girls: Girls = value; break;
                }
            }
        }

        public Ranks CurrentRanks =>this[CurrentNameType]; 
        public Votes CurrentVotes => CurrentRanks.Votes;

        public List<Name> OwnBoyVetos { get; }
        public List<Name> OwnGirlVetos { get; }
        public List<Name> OwnVetos
        {
            get
            {
                switch (CurrentNameType)
                {
                    case NameTypes.Boys: return OwnBoyVetos;
                    case NameTypes.Girls: return OwnGirlVetos;
                    default: return new List<Name>();
                }
            }
        }


        public IGenerator Rnd { get; set; }

        public NameTypes CurrentNameType { get; set; }

        public Rank First { get; set; }
        public Rank Second { get; set; }

        public DateTime LastUpdate { get; private set; }

        public IEnumerable<FileInfo> FilesToWatch
        {
            get
            {
                return Location.EnumerateFiles()
                    .Where(file => !file.Name.StartsWith(Id, StringComparison.InvariantCultureIgnoreCase) &&
                        (Veto.IsFile(file) || Vote.IsFile(file)));

            }
        }

        public List<Rank> GetNamesWithoutVotes()
        {
            var names = CurrentRanks.ToDictionary(rank => rank.Name, rank => rank);
            foreach (var vote in CurrentVotes)
            {
                names.Remove(vote.Left);
                names.Remove(vote.Right);
            }
            return names.Values.ToList();
        }

        public Rank GetRandom()
        {
            return CurrentRanks[Rnd.Next(CurrentRanks.Count)];
        }

        public void SetFirstAndSecond()
        {
            var noVotes = GetNamesWithoutVotes();
            if (noVotes.Any())
            {
                First = noVotes[Rnd.Next(noVotes.Count)];
                noVotes.Remove(First);
            }
            else
            {
                First = GetRandom();
            }
            if (noVotes.Any())
            {
                Second = noVotes[Rnd.Next(noVotes.Count)];
            }
            else
            {
                Second = First;
                while (Second == First)
                {
                    Second = GetRandom();
                }
            }
        }

        public bool HasChangedFiles()
        {
            if (!FilesToWatch.Any())
            {
                return false;
            }
            var newest = FilesToWatch.Max(file =>
            {
                file.Refresh();
                return file.LastWriteTimeUtc;
            });

            if (newest > LastUpdate)
            {
                LastUpdate = newest;
                return true;
            }
            return false;
        }

        public void UpdateIfNeeded()
        {
            if (HasChangedFiles())
            {
                Boys = null;
                Girls = null;
            }
        }

        public void UpdateCurrentNameType()
        {
            if (NameTypes.IsSingleGender()) { return; }
            if (Rnd.NextDouble() < 0.8) { return; }

            else if (CurrentNameType == NameTypes.Boys)
            {
                CurrentNameType = NameTypes.Girls;
            }
            CurrentNameType = NameTypes.Boys;
        }

        private Ranks GetRanks(NameTypes tp)
        {
            if (tp == NameTypes.Boys && Boys != null) { return Boys; }
            if (tp == NameTypes.Girls && Girls != null) { return Girls; }

            var votes = Location.EnumerateFiles(string.Format("*.{0}.xml", tp.ToString().ToLowerInvariant()));
            var vetos = Location.EnumerateFiles(string.Format("*.{0}.vetos.txt", tp.ToString().ToLowerInvariant()));

            var ranks = new Ranks(
                Names.FromDirectory(Location, tp),
                votes.SelectMany(file => Votes.Load(file)),
                vetos.SelectMany(file => Vetos.Load(file)))
            {
                NameType = tp
            };
            ranks.Recalculate();
            this[tp] = ranks;
            return ranks;
        }
    }
}
