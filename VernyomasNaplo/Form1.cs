﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace VernyomasNaplo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Watcher();
        }

        public enum Idopont
        {
            Reggeli = 10,
            Delutani = 15,
            Esti = 21
        }


        public void Watcher()
        {
            DateTime idopont = DateTime.Now;
            int counter = 0;

            //utolsó sorok beolvasása
            foreach (var line in File.ReadLines(@"F:\Dokumentumok\Vérnyomás-Eredmények\eredmenyek.txt").Reverse())
            {
                string tag = line.Split('\t').First();
                DateTime elozoNapDatum;
                int elozoNap;
                if (DateTime.TryParse(tag, out elozoNapDatum))
                {
                    elozoNap = elozoNapDatum.Day;
                } else
                {
                    elozoNap = 0;
                }
                
                

                if (idopont.Day == elozoNap)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }

            if (idopont.Hour > (int)Idopont.Reggeli && idopont.Hour < (int)Idopont.Delutani && counter == 0)
            {
                FajlbaIr(idopont.ToString(@"MM\/dd\/yyyy HH:mm") + "\t" + "Elmaradt a mérés");
            }
            else if (idopont.Hour > (int)Idopont.Delutani && idopont.Hour < (int)Idopont.Esti && counter < 2)
            {
                for (int i = counter; i < 2; i++)
                {
                    FajlbaIr(idopont.ToString(@"MM\/dd\/yyyy HH:mm") + "\t" + "Elmaradt a mérés");
                }
            }
            else if (idopont.Hour > (int)Idopont.Esti && counter < 3)
            {
                for (int i = counter; i < 3; i++)
                {
                    FajlbaIr(idopont.ToString(@"MM\/dd\/yyyy HH:mm") + "\t" + "Elmaradt a mérés");
                }
            }

            while (true)
            {

                TimeSpan dt = DateTime.Now.TimeOfDay;
                if (dt.Hours == (int)Idopont.Reggeli && dt.Minutes == 0 && dt.Seconds == 0)
                {
                    Write();
                    break;
                }
                else if (dt.Hours == (int)Idopont.Delutani && dt.Minutes == 0 && dt.Seconds == 0)
                {
                    Write();
                    break;

                }
                else if (dt.Hours == (int)Idopont.Esti && dt.Minutes == 0 && dt.Seconds == 0)
                {
                    Write();
                    break;
                }
                Thread.Sleep(900);
            }


        }

        private void Write()
        {
            MessageBox.Show("Mérd meg a vérnyomásodat!","Emlékeztető",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            this.Visible = true;
            this.Activate();
            
        }

        private bool isValid(string mezo)
        {
            int ertek;
            bool sikerulte = int.TryParse(mezo, out ertek);
            if (mezo == "" || !sikerulte || ertek < 0 || ertek > 200)
            {
                return false;
            }
            return true;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void gomb_Click(object sender, EventArgs e)
        {
            bool AllRight = true;
            DateTime idopont = DateTime.Now;


            //pipát vizsgáljuk
            if (batteryDead.Checked)
            {
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"F:\Dokumentumok\Vérnyomás-Eredmények\eredmenyek.txt", true))
                {
                    file.WriteLine(idopont.ToString(@"MM\/dd\/yyyy HH:mm") + "\tLemerült az elem.");
                }
                this.Visible = false;
                Watcher();
            }

            if (!isValid(this.szisztoles_box.Text))
            {
                MessageBox.Show("Kitöltetlen, vagy hibás mező!\nAdd meg a szisztolés értéket!");
                AllRight = false;
            }
            else if (!isValid(this.diasztoles_box.Text))
            {
                MessageBox.Show("Kitöltetlen, vagy hibás mező!\nAdd meg a diasztolés értéket!");
                AllRight = false;
            }
            else if (!isValid(this.pulzus_box.Text))
            {
                MessageBox.Show("Kitöltetlen, vagy hibás mező!\nAdd meg a pulzus értéket!");
                AllRight = false;
            }


            if (AllRight)
            {

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"F:\Dokumentumok\Vérnyomás-Eredmények\eredmenyek.txt", true))
                {
                    file.WriteLine(idopont.ToString(@"MM\/dd\/yyyy HH:mm") + "\t" + szisztoles_box.Text + "\t\t" + diasztoles_box.Text + "\t\t" + pulzus_box.Text);
                }
                this.Visible = false;
                Watcher();
            }

        }

        private void FajlbaIr(string s)
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"F:\Dokumentumok\Vérnyomás-Eredmények\eredmenyek.txt", true))
            {
                file.WriteLine(s);
            }
        }

    }
}