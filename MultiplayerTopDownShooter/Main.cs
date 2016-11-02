using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ClonesEngine;

namespace MultiplayerTopDownShooter
{
    public partial class Main : Form
    {
        Random RNG;
        GestionnaireDePacket GP;

        public Main()
        {
            InitializeComponent();

            RNG = new Random();
            GP = new GestionnaireDePacket();
            this.DoubleBuffered = true;
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            Invoke(new Action(() =>
            {
                for (int i = 1; i <= GP.PlayerCount; i++)
                {
                    int X, Y;
                    X = GP.PlayerList[i].Position.X;
                    Y = GP.PlayerList[i].Position.Y;

                    e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(GP.PlayerList[i].Couleur)), X - 10, Y - 10, 20, 20);
                }
            }));

        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            GP.PlayerList[GP.ID].AjouterProjectile(Environment.TickCount);
            //GP.PlayerList[GP.ID].PlayerBullet.RemoveAt(j);
        }
    }
}
