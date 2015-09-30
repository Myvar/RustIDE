using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustIDE.Internal.Crates
{
    public class Create
    {
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public string documentation { get; set; }
        public int downloads { get; set; }
        public string homepage { get; set; }
        public string id { get; set; }
        public List<string> keywords { get; set; }
        public string license { get; set; }
        public Dictionary<string,string> links { get; set; }
        public string max_version { get; set; }
        public string name { get; set; }
        public string repository { get; set; }
        public DateTime updated_at { get; set; }
    }
}
