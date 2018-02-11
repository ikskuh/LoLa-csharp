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
    public partial class Form1 : Form
    {
        private List<BasicComputer> computers = new List<BasicComputer>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var com = new BasicComputer();
            com.Display.WriteLine("Hello, World!");
            this.computers.Add(com);
        }
    }
}
