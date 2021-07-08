﻿using System;
using System.Windows.Forms;
using System.Linq;

namespace ChangerAPI
{
    public partial class OpenNewSkinForm : Form
    {

        public OpenNewSkinForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please, enter new skin name", "Naming error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (textBox1.Text == "default")
            {
                MessageBox.Show("Reserved name. Please enter another name", "Naming error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Text = "";
                textBox1.Invalidate();
                return;
            }

            foreach(var skinName in EnvironmentalVeriables.Skins.Select(x => x.Name))
            {
                if (skinName == textBox1.Text)
                {
                    MessageBox.Show("Name already used. Please, enter another skin name", "Naming error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Text = "";
                    textBox1.Invalidate();
                    return;
                }
            }

            EnvironmentalVeriables.TempSkinName = textBox1.Text;
            this.Close();
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(null, null);
        }
    }
}
