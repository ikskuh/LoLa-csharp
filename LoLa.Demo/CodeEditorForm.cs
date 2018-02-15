using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoLa.Demo
{
    public partial class CodeEditorForm : Form
    {
        public event EventHandler Run;

        public CodeEditorForm()
        {
            InitializeComponent();
            this.scintilla.Margins[0].Width = 16; // Show line numbers
            foreach (var style in this.scintilla.Styles)
                style.Font = "Consolas";
        }

        public string Code
        {
            get { return this.scintilla.Text; }
            set { this.scintilla.Text = value; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Run?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.R)
            {
                this.Run?.Invoke(this, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
                e.SuppressKeyPress = true;
            }
        }
    }
}
