using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    public partial class OptionsMenu : Form
    {
        public OptionsMenu()
        {
            InitializeComponent();
        }
        private string _img;

        public string Img { get => _img; set => _img = value; }

        private void btnExit_Click(object sender, EventArgs e)
        {//button start
            BGL bgl = new BGL();
            bgl.IMG = Img;
            this.Close();
            GC.Collect();
            bgl.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            
        }

        private void OptionsMenu_Load(object sender, EventArgs e)
        {
            Img = "";
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
           
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
           
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
           
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
           
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            this.Img = "BLUE";
            btnExit.Enabled = true;//btn za start game
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Img = "RED";
            btnExit.Enabled = true;//btn za start game
        }
    }
}
