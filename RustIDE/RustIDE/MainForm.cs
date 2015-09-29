using FastColoredTextBoxNS;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            materialTabControl1.TabPages.Clear();
            AddEditorTab("new");
        }


        private void AddEditorTab(string name)
        {
            TabPage p = new TabPage(name);
            var textbox = new FastColoredTextBox() { Dock = DockStyle.Fill };
            p.Controls.Add(textbox);
            textbox.TextChanged += Textbox_TextChanged;
            materialTabControl1.TabPages.Add(p);
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
    }
}

