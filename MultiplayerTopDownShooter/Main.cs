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
        int FPSTimer;
        int FPSCounter = 0;
        int FPSLast = 0;
        public Main()
        {
            InitializeComponent();

            RNG = new Random();
            GP = new GestionnaireDePacket();
            this.DoubleBuffered = true;

            FPSTimer = Environment.TickCount;
       }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            Invoke(new Action(() =>
            {
                for (int i = 1; i <= GP.PlayerCount; i++)
                {
                    e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(GP.PlayerList[i].Couleur)),
                        GP.PlayerList[i].Position.X - GP.PlayerList[i].Size / 2, GP.PlayerList[i].Position.Y - GP.PlayerList[i].Size / 2, GP.PlayerList[i].Size, GP.PlayerList[i].Size);
                    lock (GP.PlayerList[i].BulletLock)
                    {
                            for (int j = GP.PlayerList[i].Bullet.Count - 1; j > 0; j--)
                            {
                                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(GP.PlayerList[i].Couleur)), GP.PlayerList[i].Bullet[j].Position.X - 2, GP.PlayerList[i].Bullet[j].Position.Y - 2, 4, 4);
                            }
                    }

                }
                e.Graphics.DrawString(GP.ID.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 10, 10);
                e.Graphics.DrawString(GP.PlayerList[GP.ID].Score.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 10, 50);
                if (Environment.TickCount - FPSTimer > 1000)
                {
                    FPSLast = FPSCounter;
                    FPSTimer = Environment.TickCount;
                    FPSCounter = 0;
                }
                FPSCounter++;
                e.Graphics.DrawString(FPSLast.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 10, 30);
                GP.UpdatePlayer(GP.ID);
                GP.Send(TramePreGen.InfoJoueur(GP.PlayerList[GP.ID], GP.ID, GP.PacketID));
                /*if (MouseButtons == MouseButtons.Left)
                {
                    Main_MouseDown(sender, new MouseEventArgs(MouseButtons.Left,1,10,10,1));
                }*/
                //Thread.Sleep(RNG.Next(100)); //random ping generator ;)
                this.Invalidate();
            }));

        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            int X, Y; 
            X = PointToClient(Cursor.Position).X;
            Y = PointToClient(Cursor.Position).Y;
            
            GP.PlayerList[GP.ID].AjouterProjectile(new PointF(((X - GP.PlayerList[GP.ID].Position.X) / (float)Math.Sqrt((X - GP.PlayerList[GP.ID].Position.X) * (X - GP.PlayerList[GP.ID].Position.X) + (Y - GP.PlayerList[GP.ID].Position.Y) * (Y - GP.PlayerList[GP.ID].Position.Y))),
                ((Y - GP.PlayerList[GP.ID].Position.Y) / (float)Math.Sqrt((X - GP.PlayerList[GP.ID].Position.X) * (X - GP.PlayerList[GP.ID].Position.X) + (Y - GP.PlayerList[GP.ID].Position.Y) * (Y - GP.PlayerList[GP.ID].Position.Y)))));
            
            //GP.PlayerList[GP.ID].PlayerBullet.RemoveAt(j);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                /*case Keys.I:
                    Players[0].IsAI = !Players[0].IsAI;
                    break;*/ //On/Off AI main player, used for debug purpose (and cheating)
                case Keys.S:
                    ChangeArrowsState(ArrowsPressed.Down, true);
                    break;
                case Keys.W:
                    ChangeArrowsState(ArrowsPressed.Up, true);
                    break;
                case Keys.A:
                    ChangeArrowsState(ArrowsPressed.Left, true);
                    break;
                case Keys.D:
                    ChangeArrowsState(ArrowsPressed.Right, true);
                    break;
                case Keys.Escape:
                    Environment.Exit(0);
                    break;
                case Keys.Space:
                    ChangeArrowsState(ArrowsPressed.Space, true);
                    Point Light = this.PointToClient(Cursor.Position);
                    GP.PlayerList[GP.ID].Position = Light;
                    break;
                case Keys.Return:

                    GP.Send(new byte[] {(byte)PacketUse.ResetAllID, GP.ID});
                    break;
                default:
                    return;
            }
            HandleArrows();
            e.Handled = true;
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            switch (e.KeyCode)
            {
                case Keys.S:
                    ChangeArrowsState(ArrowsPressed.Down, false);
                    break;
                case Keys.W:
                    ChangeArrowsState(ArrowsPressed.Up, false);
                    break;
                case Keys.A:
                    ChangeArrowsState(ArrowsPressed.Left, false);
                    break;
                case Keys.D:
                    ChangeArrowsState(ArrowsPressed.Right, false);
                    break;
                case Keys.Space:
                    ChangeArrowsState(ArrowsPressed.Space, false);
                    break;
                default:
                    return;
            }
            HandleArrows();
            e.Handled = true;
        }
        private void HandleArrows()
        {

            if ((arrowsPressed & ArrowsPressed.Up) == ArrowsPressed.None && (arrowsPressed & ArrowsPressed.Down) != ArrowsPressed.None)
            {
                GP.PlayerList[GP.ID].Velocite = new PointF(GP.PlayerList[GP.ID].Velocite.X, 1);
                //VY = 1;
            }

            if ((arrowsPressed & ArrowsPressed.Down) == ArrowsPressed.None && (arrowsPressed & ArrowsPressed.Up) != ArrowsPressed.None)
            {
                GP.PlayerList[GP.ID].Velocite = new PointF(GP.PlayerList[GP.ID].Velocite.X, -1);
                //VY = -1;
            }

            if ((arrowsPressed & ArrowsPressed.Right) == ArrowsPressed.None && (arrowsPressed & ArrowsPressed.Left) != ArrowsPressed.None)
            {
                GP.PlayerList[GP.ID].Velocite = new PointF(-1, GP.PlayerList[GP.ID].Velocite.Y);
                //VX = -1;
            }

            if ((arrowsPressed & ArrowsPressed.Left) == ArrowsPressed.None && (arrowsPressed & ArrowsPressed.Right) != ArrowsPressed.None)
            {
                GP.PlayerList[GP.ID].Velocite = new PointF(1, GP.PlayerList[GP.ID].Velocite.Y);
                //VX = 1;
            }

            if (((arrowsPressed & ArrowsPressed.Up) | (arrowsPressed & ArrowsPressed.Down)) == ArrowsPressed.None)
            {
                GP.PlayerList[GP.ID].Velocite = new PointF(GP.PlayerList[GP.ID].Velocite.X, 0);
                //VY = 0;
            }

            if (((arrowsPressed & ArrowsPressed.Right) | (arrowsPressed & ArrowsPressed.Left)) == ArrowsPressed.None)
            {
                GP.PlayerList[GP.ID].Velocite = new PointF(0, GP.PlayerList[GP.ID].Velocite.Y);
                //VX = 0;
            }

            //  Do whatever is needed using position
        }
        enum ArrowsPressed
        {
            None = 0x00,
            Left = 0x01,
            Right = 0x02,
            Up = 0x04,
            Down = 0x08,
            Space = 0x10,
            Escape = 0x20,
            All = 0x3F
        }
        ArrowsPressed arrowsPressed;
        void ChangeArrowsState(ArrowsPressed changed, bool isPressed)
        {
            if (isPressed)
            {
                arrowsPressed |= changed;
            }
            else
            {
                arrowsPressed &= ArrowsPressed.All ^ changed;
            }
        }
    }
}
