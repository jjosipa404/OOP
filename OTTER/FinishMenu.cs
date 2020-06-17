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
    public partial class FinishMenu : Form
    {
        public FinishMenu()
        {
            InitializeComponent();
        }
        private int score;

        public int Score { get => score; set => score = value; }

        private void labelSc_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveScore_Click(object sender, EventArgs e)
        {
            Scores sc = new Scores();
            sc.Sc = int.Parse(lblScore.Text);
          
            sc.ShowDialog();
            btnSaveScore.Enabled = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            
            BGL bgl = new BGL();
            this.Close();
            bgl.Show();
            
        }

        private void lblScore_Click(object sender, EventArgs e)
        {
            
        }

      
        private void FinishMenu_Load(object sender, EventArgs e)
        {
            lblScore.Text = Score.ToString();
            if (Score <= 0)
            {//onemoguciti spremanje negativnih bodova
                btnSaveScore.Enabled = false;
            }
        }
    }
}
