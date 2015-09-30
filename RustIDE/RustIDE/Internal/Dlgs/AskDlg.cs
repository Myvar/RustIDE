using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsDevKit.UI.Dialogs
{
    public partial class AskDlg : Form
    {
        public string Quiestion { get; set; }
        public string Awnser { get; set; }

        public AskDlg()
        {
            InitializeComponent();
        }

        private void AskDlg_Load(object sender, EventArgs e)
        {
            label1.Text = Quiestion;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Awnser = textBox1.Text;
            this.Close();
        }
    }
}
