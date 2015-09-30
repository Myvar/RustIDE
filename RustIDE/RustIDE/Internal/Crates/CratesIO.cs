using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RustIDE.Internal.Crates
{
    public static class CratesIO
    {
        private static WebClient wc = new WebClient();

        public static async Task<SearchResult> Search(string term)
        {
            var s = await wc.DownloadStringTaskAsync("https://crates.io/api/v1/crates?q=" + term + "&page=1&per_page=10");
            return JsonConvert.DeserializeObject<SearchResult>(s);
        }

        public static async Task<Summary> Summery()
        {
            var s = await wc.DownloadStringTaskAsync("https://crates.io/summary");
            return JsonConvert.DeserializeObject<Summary>(s);
        }

    }
}
