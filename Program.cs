using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace cygniumTest
{
    class Program{
        
        public class SearchResult{
            public string word;            
            public string resultsGoogleEngine;          
            public string resultsBingEngine;            
        }

        class searchEngine {
            public string url;
            public HttpWebRequest request;
            public StreamReader responseReader;
            public dynamic obj;

            public void configurateRequest(String word){
                this.request = (HttpWebRequest)WebRequest.Create(url + word);
                this.request.Method = "GET";
            }

            public void executeReader() {
                this.responseReader = new StreamReader(request.GetResponse().GetResponseStream());
                this.obj = JsonConvert.DeserializeObject(this.responseReader.ReadToEnd());
            }

        }

        class GoogleSearchEngine:searchEngine {
            
            public GoogleSearchEngine(String googleUrl) {
                this.url = googleUrl;
            }
            public string numberResults(){                
                return this.obj["queries"]["request"][0]["totalResults"];
            }
        }

        class BingSearchEngine:searchEngine {
            
            public BingSearchEngine(String bingUrl) {
                this.url = bingUrl;
            }
            public void addHeaders(){
                this.request.Headers.Add("Ocp-Apim-Subscription-Key", "1c9fecb53d2c4fbe904fbf4e3edc80b5");
            }
            public string numberResults(){
                return this.obj["webPages"]["totalEstimatedMatches"];
            }
        }

        static string googleUrl = "https://www.googleapis.com/customsearch/v1?key=AIzaSyAyDl3R3AupEjd2IndVYy1NWSdZQ6sUSvw&cx=008641693920004576983:k4cnpd2iwke&q=";
        static string bingUrl = "https://api.cognitive.microsoft.com/bing/v5.0/search?q=";

        public List<SearchResult> executeSearchs(String[] args) {
            var p = new Program();
            List<SearchResult> list = new List<SearchResult>();
            
            GoogleSearchEngine googleSearch = new GoogleSearchEngine(googleUrl);
            BingSearchEngine bingSearch = new BingSearchEngine(bingUrl);
                
            for (int i = 0; i < args.Length; i++){
                SearchResult searchResult = new SearchResult();
                
                googleSearch.configurateRequest(args[i]);
                googleSearch.executeReader();
                
                bingSearch.configurateRequest(args[i]);
                bingSearch.addHeaders();
                bingSearch.executeReader();
                
                searchResult.word = args[i];
                searchResult.resultsGoogleEngine = googleSearch.numberResults(); 
                searchResult.resultsBingEngine = bingSearch.numberResults(); 
                Console.WriteLine(searchResult.word + ": Google: " + searchResult.resultsGoogleEngine + " Bing: " + searchResult.resultsBingEngine);
                list.Add(searchResult);
            }
            return list;
        }

        public void compareResults(List<SearchResult> searchRessults) {
            String googleWinner, bingWinner, totalWinner;
            Double googleVotes, bingVotes, totalVotes;
            
            googleWinner = bingWinner = totalWinner = "";
            googleVotes = bingVotes = totalVotes = -1;
   
            foreach (SearchResult result in searchRessults) {
                Double votes = Double.Parse(result.resultsGoogleEngine) + Double.Parse(result.resultsBingEngine);
                if (Double.Parse(result.resultsGoogleEngine) > googleVotes) {
                    googleWinner = result.word;
                    googleVotes = Double.Parse(result.resultsGoogleEngine);
                }
                if (Double.Parse(result.resultsBingEngine) > bingVotes)
                {
                    bingWinner = result.word;
                    bingVotes = Double.Parse(result.resultsBingEngine);
                }
                if (votes > totalVotes)
                {
                    totalWinner = result.word;
                    totalVotes = votes;
                }
            }
            Console.WriteLine("Google winner: " + googleWinner);
            Console.WriteLine("Bing winner: " + bingWinner);
            Console.WriteLine("Total winner: " + totalWinner);
        
        }

        static void Main(string[] args){
            var p = new Program();
            try
            {
                List<SearchResult> searchResultsList = p.executeSearchs(args);
                p.compareResults(searchResultsList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally {
                Environment.Exit(0);
            }
        }
    }
}
