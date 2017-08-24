using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using static KeywordResearchTool.BLL.Services.GoogleSearchResult;
using System.Diagnostics;
using KeywordResearchTool.BLL.Models;

namespace KeywordResearchTool.BLL.Services
{
    public class GoogleSearchAPI
    {
        private static HttpClient _client = new HttpClient();
        private static string _query;
        private static string _siteUrl = "http://doogieads.org"; // default value
        private static string _apiUrl = "https://www.googleapis.com/customsearch/v1?";
        private static string _apiKey = "AIzaSyA0IIHY3gheEeQY1wqhy5sO9ObSkByQNYM"; // key param
        private static string _searchEngineId = "013905875247098905288:iumtcaotfgg"; // cx param
        private static int _rankNumber = 1;

        public const int MAX_NUMBER_RESULTS_TO_CHECK = 250;

        public GoogleSearchAPI(string query, string siteUrl)
        {
            _query = query;
            _siteUrl = siteUrl;
        }

        public SerpRank QueryResults()
        {
            if (!String.IsNullOrEmpty(_query))
            {
                return GetRank(_query).Result;
            }

            return null;
        }

        public static async Task<SerpRank> GetRank(string query)
        {
            if (String.IsNullOrEmpty(query) || _rankNumber > MAX_NUMBER_RESULTS_TO_CHECK)
                return null;

            SerpRank rank = null;

            try
            {
                using (_client)
                {
                    Debug.Write("Callout made!!!!!!");
                    string requestUrl = string.Format("{0}key={1}&cx={2}&q={3}&start={4}",
                        _apiUrl, _apiKey, _searchEngineId, Uri.EscapeDataString(query), _rankNumber.ToString());

                    HttpResponseMessage response = await _client.GetAsync(requestUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        RootObject rootResult = JsonConvert.DeserializeObject<RootObject>(result);
                        rank = FindHighestRank(rootResult);

                        if (rank == null || rank.HighestRankNumber == 0)
                            await GetRank(query);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }

            return rank;
        }

        private static SerpRank FindHighestRank(RootObject rootResult)
        {
            if (rootResult == null || rootResult.items == null || rootResult.items.Count == 0)
                return null;

            SerpRank highestRank = null;
            bool matchFound = false;
            int resultIndex = 0;

            try
            {
                do
                {
                    Item item = rootResult.items[resultIndex];

                    if (item.link.ToLower().Contains(_siteUrl) ||
                        item.displayLink.ToLower().Contains(_siteUrl))
                    {
                        highestRank = new SerpRank();
                        highestRank.HighestRankNumber = _rankNumber;
                        highestRank.SiteUrl = item.link;
                        matchFound = true;
                    }
                    else
                    {
                        resultIndex++;
                        _rankNumber++;
                    }

                } while (!matchFound && resultIndex < rootResult.items.Count);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }

            return highestRank;
        }
    }
}
