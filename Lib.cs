using System;
using System.Collections.Generic;
using System.Linq;


namespace copete
{
    public class Lib
    {

        public int LibNo;

        private int NBooks;
        public int Days;
        private int BooksPerDay;
        public List<double> BookScore { get; set; }
        public List<int> BookScoreInt { get; set; }

        public List<int> SortedBooksId { get; internal set; }
        public int CurrScore { get; internal set; }
        public double CurrScoreAdj { get; internal set; }


        public Lib(int libNo, int v1, int v2, int v3)
        {
            LibNo = libNo;
            this.NBooks = v1;
            this.Days = v2;
            this.BooksPerDay = v3;
        }

        public void Adj(int maxDays)
        {
            CurrScoreAdj = CurrScore * 1.0 * (1.0 - Days * 0.85 / maxDays);
        }

        /// <summary>
        ///             !!!!!!!!!!!!!   USE THIS for "C" file
        /// </summary>
        /// <param name="maxDays"></param>
        public void Adj2(int maxDays)
        {
            CurrScoreAdj = /*CurrScore **/ 1.0 * (1.0 - Days * 0.85 / maxDays);
        }

        internal void CalculateScore(int nDays)
        {
            int daysLeft = nDays - Days;
            int score = 0;
            int bookIndex = 0;
            int SortedBooksId_Count = SortedBooksId.Count;
            if (SortedBooksId_Count == 0) {
                CurrScore = 0;
                return;
            }
            while (daysLeft > 0)
            {
                int k = 0;
                while (k < BooksPerDay) { 
                    var book = SortedBooksId[bookIndex];
                    score += BookScoreInt[book];
                    bookIndex++;
                    if (bookIndex >= SortedBooksId_Count) break;
                    k++;
                }
                if (bookIndex >= SortedBooksId_Count) break;
                daysLeft--;
            }
            /* wrong code (we loose books at the end, when duplicates are thrown away) 
            if (daysLeft == 0 && bookIndex + 1 < SortedBooksId_Count)
                SortedBooksId.RemoveRange(bookIndex + 1, SortedBooksId_Count - bookIndex - 1);*/
            CurrScore = score;
        }

        public List<int> CalculateScoreGetBooks(int nDays)
        {
            List<int> books = new List<int>();
            int daysLeft = nDays - Days;
            int score = 0;
            int bookIndex = 0;
            int SortedBooksId_Count = SortedBooksId.Count;
            if (SortedBooksId_Count == 0)
            {
                CurrScore = 0;
                return books;
            }
            while (daysLeft > 0)
            {
                int k = 0;
                while (k < BooksPerDay)
                {
                    var book = SortedBooksId[bookIndex];
                    score += BookScoreInt[book];
                    books.Add(book);
                    bookIndex++;
                    if (bookIndex >= SortedBooksId_Count) break;
                    k++;
                }
                if (bookIndex >= SortedBooksId_Count) break;
                daysLeft--;
            }
            CurrScore = score;
            return books;
        }

        internal void SetBooksId(List<int> list)
        {
            SortedBooksId = list;
        }
        internal void AdjBooksId()
        {
            SortedBooksId.Sort((a, b) => -BookScore[a].CompareTo(BookScore[b]));
        }    
    }
}
