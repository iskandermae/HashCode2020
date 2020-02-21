using System;
using System.Collections.Generic;
using System.Linq;

namespace copete
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >0) { 
                Solve(args[0]);
                return;
            }

            List<string> files = new List<string> { "a.txt", "b.txt", "d.txt", "e.txt", "f.txt" };

            //List<string> files = new List<string> { "c.txt" }; for this file swith to Adj2

            int score = 0;
            foreach (string file in files)
            {
                int s = Solve(file);
                Console.WriteLine($"File score {file} = {s}");
                score += s;
            }
            Console.WriteLine($"Total score = {score}");
        }


        public static int Solve(string file_name)
        {
            List<int> BookScores;
            List<Lib> Libs = new List<Lib>();
            int NBooks;
            int NLibs;
            int NDays;
            Dictionary<int, List<int>> LibsPerBook = new Dictionary<int, List<int>>();

            string inFileName = file_name;
            System.IO.StreamReader file = new System.IO.StreamReader(FileHelper.InputFileName(inFileName));

            List<int> gn = GetNumbers(file.ReadLine());

            NBooks = gn[0];
            NLibs = gn[1];
            NDays = gn[2];
            for (var j = 0; j < NBooks; j++)
            {
                LibsPerBook.Add(j, new List<int>());
            }

            BookScores = GetNumbers(file.ReadLine());
            string line;
            int libNo = 0;
            while (!string.IsNullOrWhiteSpace(line = file.ReadLine()))
            {
                gn = GetNumbers(line);
                Lib b = new Lib(libNo, gn[0], gn[1], gn[2]);

                b.SetBooksId(GetNumbers(file.ReadLine()));
                b.BookScoreInt = BookScores;
                Libs.Add(b);
                foreach (int book in b.SortedBooksId)
                {
                    LibsPerBook[book].Add(libNo);
                }
                libNo++;
            }

            file.Close();

            List<double> BookScoreAdjusted = AdjustBooksValues(BookScores, LibsPerBook);

            Libs.ForEach(l => l.BookScore = BookScoreAdjusted);
            Libs.ForEach(l => l.AdjBooksId());

            List<string> result = new List<string>();
            int daysLeft = NDays;
            List<Lib> LeftLibs = new List<Lib>(Libs);
            int Libs_count = Libs.Count;
            int Libs_count_per_batch = Libs.Count / 100;
            int bestLibs = 0;
            int totalScore = 0;
            int curBatch = 0;
            int maxDays = Libs.Select(l => l.Days).Max();
            while (LeftLibs.Count > 0) {
                if (Libs_count > 100 && curBatch* Libs_count_per_batch < Libs_count - LeftLibs.Count)
                {
                    curBatch++;
                    Console.Write($" {curBatch}");
                }
                foreach (Lib ll in LeftLibs) { ll.CalculateScore(daysLeft); }
                foreach (Lib ll in LeftLibs) { ll.Adj(maxDays); }

                LeftLibs.Sort((a, b) => -a.CurrScoreAdj.CompareTo(b.CurrScoreAdj));
                Lib bestLib = LeftLibs[0];
                if (bestLib.CurrScoreAdj > 0)
                {
                    var books = bestLib.CalculateScoreGetBooks(daysLeft);
                    if (bestLib.CurrScore == 0) {
                        LeftLibs.Remove(bestLib);
                        continue;
                    };
                    result.Add(string.Format("{0} {1}", bestLib.LibNo, books.Count));
                    result.Add(string.Join(' ', books.Select(i => i.ToString())));
                    foreach (int book in books)
                    {
                        foreach (int libid in LibsPerBook[book])
                        {
                            Lib lib = Libs[libid];
                            lib.SortedBooksId.Remove(book);
                        }
                    }
                    totalScore += bestLib.CurrScore;
                    LeftLibs.Remove(bestLib);
                    bestLibs++;
                    daysLeft -= bestLib.Days;
                } else break;
            }
            List<string> result2 = new List<string>(){bestLibs.ToString()};
            result2.AddRange(result);
            System.IO.File.WriteAllLines(FileHelper.OutputFileName(inFileName), result2);
            Console.WriteLine($" {inFileName}");
            return totalScore;
        }

        private static List<double> AdjustBooksValues(List<int> bookScore, Dictionary<int, List<int>> libsPerBook)
        {
            List<double> r = new List<double>();
            //int av = bookScore.Sum()*1.0/ bookScore.Count;

            int frequentBookNo = libsPerBook.Select(l => l.Value.Count).Max();
            for (int i =0; i< bookScore.Count; i++)
            {
                r.Add(bookScore[i] - (libsPerBook[i].Count) / frequentBookNo);
            }
            return r;
        }

        private static List<int> GetNumbers(string v)
        {
            List<int> f = new List<int>();
            foreach (string s in v.Split(' '))
            {
                f.Add(int.Parse(s));
            }
            return f;
        }
        /*
        /// <summary>
        ///   Print the solution.
        /// </summary>
        static void PrintSolution(
            in RoutingModel routing,
            in RoutingIndexManager manager,
            in Assignment solution)
        {
            var totalScore = PhotoDataModel.Max * data.DistanceMatrix.GetLength(0) - solution.ObjectiveValue();
            Console.WriteLine("Total score (with returning to start point): {0}", totalScore);
            // Inspect solution.
            Console.WriteLine("Route:");
            long routeDistance = 0;
            long index = routing.Start(0);
            while (routing.IsEnd(index) == false)
            {
                Console.Write("{0} -> ", manager.IndexToNode((int)index));
                index = solution.Value(routing.NextVar(index));
            }
            Console.WriteLine("{0}", manager.IndexToNode((int)index));
            Console.WriteLine();
            List<string> res = new List<string>() { data.DistanceMatrix.GetLength(0).ToString() };
            index = routing.Start(0);
            int imgNumber = manager.IndexToNode((int)index);
            res.Add(data.GetImage(imgNumber).ToShortString());
            Console.WriteLine(data.GetImage((int)index));
            if (!routing.IsEnd(index))
                while (true)
                {
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    if (routing.IsEnd(index)) break; // hack - skip last node (we return to the first)
                    var imageIndex = manager.IndexToNode((int)index);
                    long currDist = PhotoDataModel.Max - routing.GetArcCostForVehicle(previousIndex, index, 0);
                    Console.Write(data.GetImage((int)imageIndex).ToString());
                    Console.Write($" + score {currDist}");
                    Console.WriteLine();
                    res.Add(data.GetImage((int)imageIndex).ToShortString());
                    routeDistance += currDist;
                }

            Console.WriteLine("Total score: {0}", routeDistance);
            string resFileName = $@"C:\Workspaces\VStudio 2019\google-or\My\Solver\input\my_res.txt";
            System.IO.File.WriteAllLines(resFileName, res.Select(x => x.ToString()));
        }
        */
    }
}
