using Qowaiv.Statistics;
using System.Collections.Generic;
using System.Linq;

namespace Repelsteeltje
{
    public class Ranks : List<Rank>
    {
        private readonly Dictionary<Name, Rank> lookup = new Dictionary<Name, Rank>();
        public Ranks(IEnumerable<Name> names, IEnumerable<Vote> votes, IEnumerable<Veto> vetos)
        {
            Vetos = new HashSet<Name>();

            foreach (var veto in vetos)
            {
                if (veto.IsRegex)
                {
                    var pattern = veto.Pattern;
                    foreach (var name in names.Where(n => pattern.IsMatch(n)))
                    {
                        Vetos.Add(name);
                    }
                }
                else
                {
                    Vetos.Add(veto.ToString());
                }
            }
            foreach (var name in names)
            {
                if (Vetos.Contains(name)) { continue; }
                var rank = new Rank(name);
                Add(rank);
                lookup[name] = rank;
            }
            Votes = new Votes(votes);
        }

        public Votes Votes { get; }
        public NameTypes NameType { get; set; }
        public HashSet<Name> Vetos { get; }

        public void Veto(Rank rank)
        {
            Vetos.Add(rank.Name);
            Remove(rank);
            lookup.Remove(rank.Name);
        }

        public void Recalculate()
        {
            for (var k = 30.0; k > 1; k *= 0.8)
            {
                foreach (var vote in Votes)
                {
                    Calculate(vote, k);
                }
            }
            var avg = this.Select(r => r.Rating).Avarage();
            var delta = 1600 - avg;

            foreach (var rank in this)
            {
                rank.Rating += delta;
            }
            Sort();
        }

        public void Calculate(Vote vote, double k)
        {
            lookup.TryGetValue(vote.Left, out Rank l);
            lookup.TryGetValue(vote.Right, out Rank r);

            if (l == null)
            {
                if (r != null)
                {
                    r.Rating += vote.Score > 0.5 ? -k : k / 10;
                }
                return;
            }
            if (r == null)
            {
                l.Rating += vote.Score < 0.5 ? -k : k / 10;
                return;
            }

            var z = Elo.GetZScore(l.Rating, r.Rating);

            l.Rating += k * vote.Total * (vote.Score - z);
            r.Rating += k * vote.Total * (z - vote.Score);
        }
    }
}
