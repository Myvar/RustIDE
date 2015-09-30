using FastColoredTextBoxNS;
using MaterialSkin;
using MaterialSkin.Controls;
using OsDevKit.UI.Dialogs;
using RustIDE.Internal;
using RustIDE.Internal.Crates;
using RustIDE.Internal.Dlgs;
using RustIDE.Internal.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toml;
namespace RustIDE
{
    public partial class MainForm : MaterialForm
    {
        public MainForm()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Globals.Load();
            materialTabControl1.TabPages.Clear();
            AddEditorTab("");

        }


        private void AddEditorTab(string name)
        {
            TabPage p = new TabPage(name);
            var textbox = new FastColoredTextBox() { Dock = DockStyle.Fill };
            p.Controls.Add(textbox);
            textbox.TextChanged += Textbox_TextChanged;
            textbox.KeyDown += Textbox_KeyDown;
            materialTabControl1.TabPages.Add(p);
        }

        private void Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            var fastColoredTextBox1 = sender as FastColoredTextBox;
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                File.WriteAllText((fastColoredTextBox1.Parent as TabPage).Name, fastColoredTextBox1.Text);
            }
        }

        Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        Style PurpleStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        Style BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        Style BlackStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var fastColoredTextBox1 = sender as FastColoredTextBox;

            e.ChangedRange.ClearStyle(GreenStyle);
            e.ChangedRange.ClearStyle(PurpleStyle);
            e.ChangedRange.ClearStyle(BlueStyle);
            e.ChangedRange.ClearStyle(BlackStyle);

            fastColoredTextBox1.AddStyle(GreenStyle);
            fastColoredTextBox1.AddStyle(BlueStyle);
            fastColoredTextBox1.AddStyle(BlackStyle);
            fastColoredTextBox1.AddStyle(PurpleStyle);

            e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(GreenStyle, "\\/\\*.+\\*\\/", RegexOptions.Multiline);

            e.ChangedRange.SetStyle(PurpleStyle, @"let\s|fn\s|\sin\s|for\s|while\s|match\s|\smut\s");

            e.ChangedRange.SetStyle(BlueStyle, @"fn (?<range>([A-za-z]([A-Za-z]|[0-9])+))");
            e.ChangedRange.SetStyle(BlueStyle, "\".+\"");
            e.ChangedRange.SetStyle(BlueStyle, "[0-9]");


            e.ChangedRange.SetStyle(BlackStyle, @"([A-za-z]([A-Za-z]|[0-9])+)!");



            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers("{", "}");
            e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");
        }

        public void StartDebug()
        {
            var toml = File.ReadAllText(Path.Combine(Globals.CurrentProject, "Cargo.toml"));
            bool founddep = false;
            string exename = "";
            foreach (var i in toml.Replace("\r\n", "\n").Split('\n'))
            {

                if (founddep)
                {
                    if (!string.IsNullOrEmpty(i))
                    {
                        if (i.Split('=')[0].Trim() == "name")
                        {
                            exename = i.Split('=')[1].Trim().Trim('"');
                            break;
                        }
                    }
                }
                if (i.Trim() == "[package]")
                {
                    founddep = true;
                }
            }

            foreach (TabPage i in materialTabControl1.TabPages)
            {
                File.WriteAllText(i.Name, (i.Controls[0] as FastColoredTextBox).Text);
            }

            CargoApi.Build(Globals.CurrentProject);
            string exe = Path.Combine(Globals.CurrentProject, "target", "debug", exename + ".exe");
            Cmd.RunCmd(exe + "&pause");
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories())
                if (!directory.Name.Contains(".") && directory.Name != "target")
                    directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            foreach (var file in directoryInfo.GetFiles())
                if(!file.Name.StartsWith("."))
                    directoryNode.Nodes.Add(new TreeNode(file.Name));
            return directoryNode;
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Globals.Save();
        }

        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
            StartDebug();
        }

        public void TreeViewUpdater_Tick(object sender, EventArgs e)
        {
            if (!Globals.TreeViewLoaded)
            {
                if (!string.IsNullOrEmpty(Globals.CurrentProject))
                {
                    treeView1.Nodes.Clear();
                    foreach (TreeNode i in CreateDirectoryNode(new DirectoryInfo(Globals.CurrentProject)).Nodes)
                    {
                        treeView1.Nodes.Add(i);
                    }
                }
                Globals.TreeViewLoaded = true;
                treeView1.ExpandAll();
            }

            richTextBox1.Text = Cmd.buffer;
            richTextBox1.Select(richTextBox1.Text.Length, 0);

            if (Globals.CurrentProject == "")
            {
                materialFlatButton3.Enabled = false;
                materialFlatButton4.Enabled = false;
            }
            else
            {
                materialFlatButton3.Enabled = true;
                materialFlatButton4.Enabled = true;
            }
        }

        public void LoadEditor(string path)
        {
            bool FoundPage = false;
            var file = new FileInfo(path);

            foreach (TabPage i in materialTabControl1.TabPages)
            {
                if (i.Text == file.Name)
                {
                    FoundPage = true;
                    foreach (var x in i.Controls)
                    {
                        if (x is FastColoredTextBox)
                        {
                            var textbox = x as FastColoredTextBox;
                            textbox.OpenFile(path);
                            i.Name = path;
                            break;
                        }
                    }
                    break;
                }
                if (i.Text == "")
                {
                    materialTabControl1.TabPages.Remove(i);
                }
            }

            if (!FoundPage)
            {
                AddEditorTab(file.Name);
                LoadEditor(path);
            }
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            var dlg1 = new FolderBrowserDialog();

            dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            DialogResult result = dlg1.ShowDialog();
            if (result == DialogResult.OK)
            {
                var p = dlg1.SelectedPath;
                Globals.CurrentProject = p;
                Globals.TreeViewLoaded = false;
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string file = Path.Combine(Globals.CurrentProject, treeView1.SelectedNode.FullPath);
            if (File.Exists(file))
            {
                LoadEditor(file);
            }
        }

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            NewProjectDialog dlg = new NewProjectDialog();
            dlg.ShowDialog();
        }

        private void materialFlatButton4_Click(object sender, EventArgs e)
        {
            CrateIoFrm frm = new CrateIoFrm();
            frm.ShowDialog();
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button.Equals(MouseButtons.Right))
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string path = Path.Combine(Globals.CurrentProject, treeView1.SelectedNode.FullPath);
                var dlg = new AskDlg();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(path))
                    {


                        if (!dlg.Awnser.Contains("."))
                        {
                            Directory.CreateDirectory(Path.Combine(path, dlg.Awnser));
                            Globals.TreeViewLoaded = false;
                            return;
                        }




                        if (dlg.Awnser.Contains("."))
                        {
                            var p = Path.Combine(path, dlg.Awnser);
                            File.WriteAllText(p, " ");
                            Globals.TreeViewLoaded = false;
                            return;
                        }
                    }

                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string path = Path.Combine(Globals.CurrentProject, treeView1.SelectedNode.FullPath);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Globals.TreeViewLoaded = false;
                    return;
                }
                File.Delete(path);
                Globals.TreeViewLoaded = false;
                return;


            }

        }
    }
}

