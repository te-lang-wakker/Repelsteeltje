using Qowaiv.Statistics;
using System;

namespace Repelsteeltje
{
    public class Rank : IComparable<Rank>
    {
        public Rank() => Rating = 1600;

        public Rank(Name name) : this() => Name = name;

        public Elo Rating { get; set; }
        public Name Name { get; set; }

        public override string ToString() => string.Format("{0:0000} {1}", Rating, Name.Display);

        public int CompareTo(Rank other) => other.Rating.CompareTo(Rating);
    }
}
