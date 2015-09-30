using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustIDE.Internal.Dlgs
{
    public partial class NewProjectDialog : MaterialForm
    {
        public NewProjectDialog()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            if(materialSingleLineTextField1.Text != "")
            {
                if (materialSingleLineTextField1.Text != "")
                {
                    CargoApi.CreateNewCargoProject(materialSingleLineTextField1.Text, materialSingleLineTextField2.Text);
                    Globals.CurrentProject = Path.Combine(materialSingleLineTextField2.Text, materialSingleLineTextField1.Text);
                    Globals.TreeViewLoaded = false;
                    Close();
                    return;
                }
            }
            MessageBox.Show("Please Review you input data.");
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                materialSingleLineTextField2.Text = dlg.SelectedPath;
            }
        }
    }
}
