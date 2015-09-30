using MaterialSkin;
using MaterialSkin.Controls;
using RustIDE.Internal.Crates;
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


namespace RustIDE.Internal.Forms
{
    public partial class CrateIoFrm : MaterialForm
    {
        public Dictionary<string, string> InstalledPackages = new Dictionary<string, string>();

        public CrateIoFrm()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private async void CrateIoFrm_Load(object sender, EventArgs e)
        {
            var toml = File.ReadAllText(Path.Combine(Globals.CurrentProject, "Cargo.toml"));
            bool founddep = false;
            foreach (var i in toml.Replace("\r\n", "\n").Split('\n'))
            {

                if (founddep)
                {
                    if (!string.IsNullOrEmpty(i))
                    {
                        if (i.Contains("[") && i.Contains("]"))
                        {
                            founddep = false;
                            break;
                        }

                        InstalledPackages.Add(i.Split('=')[0].Trim(), i.Split('=')[1].Trim());
                    }
                }
                if (i.Trim() == "[dependencies]")
                {
                    founddep = true;
                }
            }

            await LoadMostDownloaded();
        }

        public async Task LoadMostDownloaded()
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(new MaterialLabel() { Text = "Loading ..." });
            var x = await CratesIO.Summery();
            int offset = 0;
            panel1.Controls.Clear();
            foreach (var i in x.most_downloaded)
            {
                Panel p = new Panel();
                p.Controls.Add(new MaterialLabel() { Text = "Name: " + i.name, AutoSize = true });
                p.Controls.Add(new MaterialLabel() { Text = "Version: " + i.max_version, Location = new Point(0, 20), AutoSize = true });
                var btn = new MaterialRaisedButton() { Text = "Install", Location = new Point(2, 40), Enabled = InstalledPackages.ContainsKey(i.name) == false };
                btn.Click += (ssender, ee) =>
                {

                };
                p.Controls.Add(btn);


                p.Controls.Add(new MaterialLabel() { Text = "Description: \n" + i.description, Location = new Point(150, 0), AutoSize = true });


                p.BorderStyle = BorderStyle.FixedSingle;
                p.Location = new Point(0, offset);
                p.Size = new Size(panel1.Size.Width - 20, p.Size.Height);
                offset += p.Height + 5;
                panel1.Controls.Add(p);
            }
        }


        public async Task Search(string term)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(new MaterialLabel() { Text = "Loading ..." });
            var x = await CratesIO.Search(term);
            int offset = 0;
            panel1.Controls.Clear();
            foreach (var i in x.crates)
            {
                Panel p = new Panel();
                p.Controls.Add(new MaterialLabel() { Text = "Name: " + i.name, AutoSize = true });
                p.Controls.Add(new MaterialLabel() { Text = "Version: " + i.max_version, Location = new Point(0, 20), AutoSize = true });
                var btn = new MaterialRaisedButton() { Name = i.name + "~" + i.max_version, AutoSize = true ,Text = InstalledPackages.ContainsKey(i.name) == false? "Install" : "Uninstall", Location = new Point(2, 40)};
                btn.Click += (ssender, ee) =>
                {
                    var b = ssender as MaterialRaisedButton;
                    if (b.Text == "Install")
                    {
                        var toml = File.ReadAllText(Path.Combine(Globals.CurrentProject, "Cargo.toml"));
                        string rep = "";
                        foreach (var xx in toml.Replace("\r\n", "\n").Split('\n'))
                        {
                           
                            if (xx.Trim() == "[dependencies]")
                            {
                                rep += xx + "\r\n" + b.Name.Split('~')[0] + " = " + b.Name.Split('~')[1] + "\r\n";
                            }
                            else
                            {
                                rep += xx + "\r\n";
                            }
                        }

                        File.WriteAllText(Path.Combine(Globals.CurrentProject, "Cargo.toml"), rep);
                        MessageBox.Show("Installed package " + b.Name.Split('~')[0]);
                        this.Close();
                    }
                    else
                    {
                        var toml = File.ReadAllText(Path.Combine(Globals.CurrentProject, "Cargo.toml"));
                        string rep = "";
                        bool founddep = false;
                        foreach (var xx in toml.Replace("\r\n", "\n").Split('\n'))
                        {
                            if (founddep)
                            {
                                if (xx.Split('=')[0].Trim() == b.Name.Split('~')[0])
                                {
                                    rep = xx;
                                    break;
                                }
                            }
                            if (xx.Trim() == "[dependencies]")
                            {
                                founddep = true;
                            }
                        }

                        File.WriteAllText(Path.Combine(Globals.CurrentProject, "Cargo.toml"), toml.Replace(rep, ""));
                        MessageBox.Show("Uninstalled package " + b.Name.Split('~')[0]);
                        this.Close();
                    }
                };
                p.Controls.Add(btn);


                p.Controls.Add(new MaterialLabel() { Text = "Description: \n" + i.description, Location = new Point(150, 0), AutoSize = true });


                p.BorderStyle = BorderStyle.FixedSingle;
                p.Location = new Point(0, offset);
                p.Size = new Size(panel1.Size.Width - 20, p.Size.Height);
                offset += p.Height + 5;
                panel1.Controls.Add(p);
            }
        }

        private async void materialSingleLineTextField1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await Search(materialSingleLineTextField1.Text);
            }
        }
    }
}
