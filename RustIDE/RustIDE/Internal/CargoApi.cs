using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustIDE.Internal
{
    public class CargoApi
    {
        public static void CreateNewCargoProject(string name, string path)
        {
            //cargo new <name> --bin
        }

        public static void Build(string path)
        {
            //cargo build
            Cmd.RunCmdStdRedirect("cargo build", path);
        }
    }
}
