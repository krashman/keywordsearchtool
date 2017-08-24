using KeywordResearchTool.BLL.Models;
using KeywordResearchTool.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordResearchTool.ConsoleUI
{
    class Display
    {
        static void Main(string[] args)
        {
            bool isDone = false;

            do
            {
                Run();
                isDone = AskToContinue();
            } while (!isDone);

        }

        private static bool AskToContinue()
        {
            Console.Write("Search again? (y/n): ");
            string input = Console.ReadLine();

            if (input.ToLower().Equals("y")) return false;

            return true;
        }

        private static void Run()
        {
            Console.Write("Enter search term: ");
            string query = Console.ReadLine();
            Console.Write("Which URL do you want to search: ");
            string urlToSearch = Console.ReadLine();

            if (!String.IsNullOrEmpty(urlToSearch))
            {
                GoogleSearchAPI searchAPI = new GoogleSearchAPI(query, urlToSearch);
                DisplayResults(searchAPI.QueryResults(), query);
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }

        private static void DisplayResults(SerpRank rank, string query)
        {
            if (rank == null)
            {
                Console.Write(string.Format("No results found in Top {0} results.", GoogleSearchAPI.MAX_NUMBER_RESULTS_TO_CHECK));
                return;
            }

            string output = string.Format("Searcn term: {0}\nHighest rank: {1}\nSite URL: {2}",
                                            query, rank.HighestRankNumber, rank.SiteUrl);
            Console.WriteLine(output);
        }
    }
}
