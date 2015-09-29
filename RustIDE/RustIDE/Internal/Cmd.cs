using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustIDE.Internal
{
    public static class Cmd
    {
        public static void RunCmd(string cmd, string workingdir = null)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            if (workingdir != null)
            {
                startInfo.WorkingDirectory = workingdir;
            }
            startInfo.Arguments = "/C " + cmd;
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void RunCmdSilantly(string cmd, string workingdir = null)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;
            if (workingdir != null)
            {
                startInfo.WorkingDirectory = workingdir;
            }
            process.StartInfo = startInfo;
            process.Start();
        }

        public static string buffer { get; set; } = "";

        public static void RunCmdStdRedirect(string cmd, string workingdir = null)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (workingdir != null)
            {
                startInfo.WorkingDirectory = workingdir;
            }

            process.StartInfo = startInfo;
            process.Start();
            

            process.WaitForExit();
  
            buffer += process.StandardOutput.ReadToEnd();
            buffer += process.StandardError.ReadToEnd();
        }
    }
}
