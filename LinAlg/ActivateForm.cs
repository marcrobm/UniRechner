using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniRechner
{
    public partial class ActivateForm : Form
    {
        public string key = "";
        public bool activateclicked = false;
        public ActivateForm(String cpuid)
        {
            InitializeComponent();
            textBox1.Text = cpuid;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            key = textBoxKey.Text;
            activateclicked = true;
            this.Close();
        }
    }
}
