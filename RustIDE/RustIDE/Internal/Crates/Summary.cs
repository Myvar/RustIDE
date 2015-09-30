using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustIDE.Internal.Crates
{
    public class Summary
    {
        public List<Create> just_updated = new List<Create>();
        public List<Create> most_downloaded = new List<Create>();
        public List<Create> new_crates = new List<Create>();

        public int num_crates { get; set; }
        public int num_downloads { get; set; }
    }
}
