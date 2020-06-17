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
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }
        private int score;
        private bool pause;
        private string img;

        public int Score { get => score; set => score = value; }
        public bool Pause { get => pause; set => pause = value; }
        public string Img { get => img; set => img = value; }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            
            this.Pause = false;
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
        }

        private void Menu_Load(object sender, EventArgs e)
        {
           
        }

    

        private void btnInstructions_Click(object sender, EventArgs e)
        {
            HowTo how = new HowTo();

            how.ShowDialog();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            
        }
    }

  

}
