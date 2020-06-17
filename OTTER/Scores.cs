using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OTTER
{
    public partial class Scores : Form
    {
        public Scores()
        {
            InitializeComponent();
        }
        private int sc;

        public int Sc { get => sc; set => sc = value; }

        Dictionary<string, int> dict = new Dictionary<string, int>();

        private void Scores_Load(object sender, EventArgs e)
        {
           
            lblScore.Text = Sc.ToString();
            string dat = "Scores.txt";//ime datoteke u bin\Debug
           
            listBox1.Items.Clear();
            try
            {
                using (StreamReader sr = new StreamReader(dat))
                {
                    string linija;
                    while ((linija = sr.ReadLine()) != null)
                    {
                        string[] niz = linija.Split(';');
                        if (!dict.ContainsKey(niz[0]))//ako u dict nema spremljenih rezultata za tog playera
                            dict.Add(niz[0], int.Parse(niz[1]));
                        else
                        {//ako ima vise rezultata istog imena igraca u dictionary spremiti njegov najveci score
                            if (int.Parse(niz[1]) > dict[niz[0]])//ako je procitanii score veci od vec spremljenog scora tog igraca u dict
                            {
                                dict[niz[0]] = int.Parse(niz[1]);
                            }
                        }
                    }
                    sr.Close();
                }
            }
            
            catch (Exception)
            {
                FileStream fs = File.Create(dat);
                fs.Close();
                using (StreamReader sr = new StreamReader(dat))
                {
                    string linija;
                    while ((linija = sr.ReadLine()) != null)
                    {
                        string[] niz = linija.Split(';');
                        if (!dict.ContainsKey(niz[0]))//ako u dict nema spremljenih rezultata za tog playera
                            dict.Add(niz[0], int.Parse(niz[1]));
                        else
                        {//ako ima vise rezultata istog imena igraca u dictionary spremiti njegov najveci score
                            if (int.Parse(niz[1]) > dict[niz[0]])//ako je procitanii score veci od vec spremljenog scora tog igraca u dict
                            {
                                dict[niz[0]] = int.Parse(niz[1]);
                            }
                        }
                    }
                    sr.Close();
                }
                return;
            }

            
            DictSortPrint(dict);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dat = "Scores.txt";//ime datoteke u bin\Debug
            using (StreamWriter sw = new StreamWriter(dat, true))
            {
                Random g = new Random();
                if (textBox1.Text != "")
                {
                    string data = textBox1.Text + ";" + Sc;
                    sw.WriteLine(data);
                    if (!dict.ContainsKey(textBox1.Text))
                    {
                        dict.Add(textBox1.Text, Sc);

                    }
                    else
                    {
                        if (Sc > dict[textBox1.Text])//ako je procitanii score veci od vec spremljenog scora tog igraca u dict
                        {
                            dict[textBox1.Text] = Sc;
                        }
                    }

                }
                else
                {//ako player ne unese ime dodijeli Player+nasumicni broj
                    string name = "Player" + g.Next(0, 100);
                    string data = name + ";" + lblScore.Text;
                    sw.WriteLine(data);
                    if (!dict.ContainsKey(name))
                    {
                        dict.Add(name, Sc);

                    }
                    else
                    {
                        if (Sc > dict[name])//ako je procitanii score veci od vec spremljenog scora tog igraca u dict
                        {
                            dict[name] = Sc;
                        }
                    }
                }

            }

            DictSortPrint(dict);
            button1.Enabled = false;

        }
        

        private void DictSortPrint(Dictionary<string,int> d)
        {
            listBox1.Items.Clear();
            List<KeyValuePair<string, int>> sorted = (from kv in d orderby kv.Value select kv).ToList();
            sorted.Reverse();

            int br = 0;
            foreach (KeyValuePair<string, int> kv in sorted)
            {
                br++;
                listBox1.Items.Add(kv.Key + "\t\t" + kv.Value + "\n");
                if (br == 10)
                    break;
            }

            listBox1.SelectedItem = listBox1.Items[0];//oznaciti prvoga na ljestvici
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }




}
