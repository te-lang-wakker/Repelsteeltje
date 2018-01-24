using System;
using System.IO;
using System.Linq;

namespace Repelsteeltje.App
{
    public static class Program
    {
        public static void Main()
        {
            var id = Config.Id;
            var location = Config.FilesLocation;
            var nameTypes = Config.NameTypes;
            if (!Login(id, location, nameTypes)) { return; }

            var state = new AppState(id, location, nameTypes);

            while (true)
            {
                state.UpdateIfNeeded();
                state.UpdateCurrentNameType();
                state.SetFirstAndSecond();

                var first = state.First;
                var second = state.Second;

                var vote = state.CurrentVotes.Select(first.Name, second.Name, id);

                ShowVoteScreen(state, first, second);

                var key = Console.ReadKey();
                while (!Input.Contains(key.Key))
                {
                    key = Console.ReadKey();
                }
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        vote.Wins(first.Name);
                        break;

                    case ConsoleKey.RightArrow:
                        vote.Wins(second.Name);
                        break;

                    case ConsoleKey.UpArrow:
                        vote.Draw++;
                        break;

                    case ConsoleKey.OemPeriod:
                    case ConsoleKey.DownArrow:
                        HandleVeto(state, vote);
                        continue;
                    case ConsoleKey.X:
                        continue;
                }
                state.CurrentRanks.Recalculate();

                SaveVotes(state);
                LogRankings(state);
            }
        }

        private static void SaveVotes(AppState state)
        {
            var fileName = string.Format("{0}.{1}.xml", state.Id, state.CurrentNameType.ToString().ToLowerInvariant());
            var toSave = state.CurrentVotes.Select(state.Id);
            toSave.Save(Path.Combine(state.Location.FullName, fileName));
        }

        private static void LogRankings(AppState state)
        {
            using (var writer = new StreamWriter(Path.Combine(state.Location.FullName, state.CurrentNameType.ToString() + ".rankings.txt"), false))
            {
                foreach (var rank in state.CurrentRanks)
                {
                    writer.WriteLine(rank);
                }
            }
        }

        private static void ShowVoteScreen(AppState state, Rank first, Rank second)
        {
            Console.Clear();
            Console.WriteLine(new string('=', 36));
            var max = Math.Min(10, state.CurrentRanks.Count);
            for (var i = 0; i < max; i++)
            {
                Console.WriteLine("{0,4}  {1}", i + 1, state.CurrentRanks[i]);
            }
            if (max == 10)
            {
                Console.WriteLine("...");
                Console.WriteLine("{0,4}  {1}", state.CurrentRanks.Count, state.CurrentRanks.Last());
            }
            Console.WriteLine(new string('-', 36));
            Console.WriteLine("[{2}] {0} - {1}",
                string.Format(Config.Format, first.Name.Display),
                string.Format(Config.Format, second.Name.Display),
                state.CurrentNameType);
            Console.WriteLine("Left [<]  Right[>]  Draw[^]  Veto[.]  Skip[x]");
        }

        private static bool Login(string id, DirectoryInfo location, NameTypes nameTypes)
        {
            Console.WriteLine("Select names for {0}", nameTypes);
            Console.WriteLine("Format: {0}", Config.Format);
            Console.Write("User ID: {0}", id);
            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("{empty}");
                return false;
            }
            else { Console.WriteLine(); }
            Console.WriteLine("Files location: {0}{1}", location, location.Exists ? "" : "*");
            if (!location.Exists)
            {
                location.Create();
            }
            Console.WriteLine();
            Console.WriteLine("ready?");
            Console.ReadLine();
            return true;
        }

        public static void HandleVeto(AppState state, Vote vote)
        {
            Console.WriteLine("Veto:");
            Console.WriteLine("Left [<]  Right[>]  Both[^]  Skip[x]");
            var key = Console.ReadKey();
            while (!Input.Contains(key.Key))
            {
                key = Console.ReadKey();
            }
            switch (key.Key)
            {
                case ConsoleKey.X:
                    break;

                case ConsoleKey.LeftArrow:
                    Veto(state, state.First);
                    vote.Wins(state.Second.Name);
                    break;

                case ConsoleKey.RightArrow:
                    Veto(state, state.Second);
                    vote.Wins(state.First.Name);
                    break;

                default:
                    Veto(state, state.First);
                    Veto(state, state.Second);
                    break;
            }
        }
        private static void Veto(AppState state, Rank veto)
        {
            state.CurrentRanks.Veto(veto);
            state.OwnVetos.Add(veto.Name);

            var file = Vetos.GetFile(state.Location, state.CurrentNameType, state.Id);

            Vetos.Save(file, state.OwnVetos);
        }

        private static readonly ConsoleKey[] Input = new[]
        {
            ConsoleKey.LeftArrow,
            ConsoleKey.RightArrow,
            ConsoleKey.UpArrow,
            ConsoleKey.DownArrow, ConsoleKey.OemPeriod,
            ConsoleKey.X,
        };
    }
}
