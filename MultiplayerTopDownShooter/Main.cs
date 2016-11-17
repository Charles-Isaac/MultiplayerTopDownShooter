using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ClonesEngine;
using System.Drawing.Drawing2D;

namespace MultiplayerTopDownShooter
{
    
    public partial class Main : Form
    {
        Random RNG;
        GestionnaireDePacket GP;
        int FPSTimer;
        int FPSCounter = 0;
        int FPSLast = 0;
        int m_Lobby = 54545;

        SizeF Format = new SizeF(1, 1);

        SolidBrush Br;
        Point[] ShadowPolygon = new Point[6];
        Point[,] ShadowArray;

        Bitmap[] PlayersImage;
        Bitmap[] TerrainImage;
   //     System.Threading.Timer WeaponTimer;
        public Main()
        {
            InitializeComponent();
            //TransparencyKey = Color.Black;

            frmLobbyPrompt frmLobby = new frmLobbyPrompt();
            frmLobby.ShowDialog();
            if (frmLobby.DialogResult == DialogResult.OK)
            {
                m_Lobby = frmLobby.Lobby;
            }
            this.WindowState = FormWindowState.Maximized;
            RNG = new Random();
            GP = new GestionnaireDePacket(m_Lobby);
            this.DoubleBuffered = true;

            FPSTimer = Environment.TickCount;

            PlayersImage = new Bitmap[24];
            TerrainImage = new Bitmap[1];
            for (int i = 0; i < 24; i++)
            {
                using (Bitmap bitmap = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("Player" + (i*15).ToString().PadLeft(3, '0'), Properties.Resources.Culture)))
                    PlayersImage[i] = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppPArgb);
            }

            for (int i = 0; i < 1; i++)
            {
                using (Bitmap bitmap = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("GroundTexture2", Properties.Resources.Culture)))
                    TerrainImage[i] = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppPArgb);
            }
            Br = new SolidBrush(Color.Black);// new TextureBrush(Properties.Resources.ShadowTexture, new Rectangle(0, 0, 250, 250));
            /*

                        var autoEvent = new AutoResetEvent(false);
                        GP.StatusChecker(10);

                        // Create a timer that invokes CheckStatus after one second, 
                        // and every 1/4 second thereafter.

                        WeaponTimer = new System.Threading.Timer(GP.CheckStatus,
                                                   autoEvent, 1000, 250);
                        WeaponTimer.*/
            //   WeaponTimer = new System.Threading.Timer(autoEvent);






          //  this.BackgroundImage = TerrainImage[0];//Properties.Resources.GroundTexture2;
       }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }
        private void Main_Paint(object sender, PaintEventArgs e)
        {
            Invoke(new Action(() =>
            {

            e.Graphics.ScaleTransform((float)this.ClientSize.Height / Settings.GameSize.Width, (float)this.ClientSize.Height / Settings.GameSize.Width, System.Drawing.Drawing2D.MatrixOrder.Append);
                // e.Graphics.DrawImage(Properties.Resources.GroundTexture1,0,0,Settings.GameSize.Width,Settings.GameSize.Height);
                e.Graphics.DrawImage(TerrainImage[0], new Rectangle(0, 0, Settings.GameSize.Width, Settings.GameSize.Height), new Rectangle(0,0,250,250), GraphicsUnit.Pixel);
            for (int i = 1; i <= GP.PlayerCount; i++)
            {
                    //Murs
                    // e.Graphics.DrawImage((Image)Properties.Resources.ResourceManager.GetObject("Player" + ((int)((GP.PlayerList[i].DirectionRegard + 7.5) / 15) * 15).ToString().PadLeft(3,'0'), Properties.Resources.Culture), GP.PlayerList[i].Position.X - 45, GP.PlayerList[i].Position.Y - 45);
                    e.Graphics.DrawImage(PlayersImage[GP.PlayerList[i].DirectionRegard/15], GP.PlayerList[i].Position.X - 45, GP.PlayerList[i].Position.Y - 45);


                    // e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(GP.PlayerList[i].Couleur)),
                    //      GP.PlayerList[i].Position.X - GP.PlayerList[i].Size / 2, GP.PlayerList[i].Position.Y - GP.PlayerList[i].Size / 2, GP.PlayerList[i].Size, GP.PlayerList[i].Size);
                    lock (GP.PlayerList[i].BulletLock)
                    {
                        for (int j = GP.PlayerList[i].Bullet.Count - 1; j > 0; j--)
                        {
                            e.Graphics.FillEllipse(new SolidBrush(Color.Yellow/*FromArgb(GP.PlayerList[i].Couleur)*/), GP.PlayerList[i].Bullet[j].Position.X - Settings.DefaultBulletSize/2, GP.PlayerList[i].Bullet[j].Position.Y - Settings.DefaultBulletSize/2, Settings.DefaultBulletSize, Settings.DefaultBulletSize);
                            e.Graphics.DrawEllipse(new Pen(Color.Black/*FromArgb(GP.PlayerList[i].Couleur)*/, Settings.DefaultBulletSize/4), GP.PlayerList[i].Bullet[j].Position.X - Settings.DefaultBulletSize/2, GP.PlayerList[i].Bullet[j].Position.Y - Settings.DefaultBulletSize/2, Settings.DefaultBulletSize, Settings.DefaultBulletSize);
                        }
                    }

                }
                if (GP.Map != null)
                {
                    ShadowArray = Shadows.ReturnMeAnArray(GP.Map.Murs.Length, GP.Map.Murs, GP.PlayerList[GP.ID].Position, Settings.GameSize.Width , Settings.GameSize.Height);

                    for (int i = GP.Map.Murs.Length - 1; i > 0; i--)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            ShadowPolygon[j] = ShadowArray[i, j];
                        }
                        //try
                        {
                            e.Graphics.FillPolygon(Br, ShadowPolygon);
                        }
                        //catch { MessageBox.Show("Erreur Ombre"); }
                    }
                }

                if (GP.Map != null)
                {
                    for (int j = GP.Map.Murs.Length - 1; j > 0; j--)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Green, 5.0F), GP.Map.Murs[j].A, GP.Map.Murs[j].B);
                    }
                }

               

                
                e.Graphics.DrawString(GP.ID.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 10);
                e.Graphics.DrawString(GP.PlayerList[GP.ID].Score.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 60);
                e.Graphics.DrawString(((int)((GP.PlayerList[GP.ID].DirectionRegard + 7.5) / 15) * 15).ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 110);
                

                if (Environment.TickCount - FPSTimer > 1000)
                {
                    FPSLast = FPSCounter;
                    FPSTimer = Environment.TickCount;
                    FPSCounter = 0;
                }
                FPSCounter++;
                e.Graphics.DrawString(FPSLast.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black
                    ), Settings.GameSize.Width + 70, 160);
                Point MousePosition = this.PointToClient(Cursor.Position);
                MousePosition.X = (int)(MousePosition.X * Settings.GameSize.Width / (float)this.ClientSize.Height);
                MousePosition.Y = (int)(MousePosition.Y * Settings.GameSize.Height / (float)this.ClientSize.Height);
                e.Graphics.DrawLine(new Pen(Color.Red), GP.PlayerList[GP.ID].Position, MousePosition);
               // Thread.Sleep(250); //Lag gen
                GP.UpdatePlayer(GP.ID, MousePosition);
                GP.Send(TramePreGen.InfoJoueur(GP.PlayerList[GP.ID], GP.ID, GP.PacketID));
                /*if (MouseButtons == MouseButtons.Left)
                {
                    Main_MouseDown(sender, new MouseEventArgs(MouseButtons.Left,1,10,10,1));
                }*/
                //Thread.Sleep(RNG.Next(100)); //random ping generator ;)

   //             e.Graphics.DrawImage(Properties.Resources.Glock17, Settings.GameSize.Width+10, 0);
                //Invalidate();
                this.Invalidate(new Rectangle(new Point(0,0), new Size(this.ClientSize.Height+100, this.ClientSize.Height)));
            }));

        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            Point MousePosition = this.PointToClient(Cursor.Position);
            MousePosition.X = (int)(MousePosition.X * Settings.GameSize.Width / (float)this.ClientSize.Height);
            MousePosition.Y = (int)(MousePosition.Y * Settings.GameSize.Height / (float)this.ClientSize.Height);
            GP.WeaponList[1].MouseDown(new PointF(((MousePosition.X - GP.PlayerList[GP.ID].Position.X) / (float)Math.Sqrt((MousePosition.X - GP.PlayerList[GP.ID].Position.X) * (MousePosition.X - GP.PlayerList[GP.ID].Position.X) + (MousePosition.Y - GP.PlayerList[GP.ID].Position.Y) * (MousePosition.Y - GP.PlayerList[GP.ID].Position.Y))),
                ((MousePosition.Y - GP.PlayerList[GP.ID].Position.Y) / (float)Math.Sqrt((MousePosition.X - GP.PlayerList[GP.ID].Position.X) * (MousePosition.X - GP.PlayerList[GP.ID].Position.X) + (MousePosition.Y - GP.PlayerList[GP.ID].Position.Y) * (MousePosition.Y - GP.PlayerList[GP.ID].Position.Y)))));

       //     GP.PlayerList[GP.ID].AjouterProjectile(new PointF(((MousePosition.X - GP.PlayerList[GP.ID].Position.X) / (float)Math.Sqrt((MousePosition.X - GP.PlayerList[GP.ID].Position.X) * (MousePosition.X - GP.PlayerList[GP.ID].Position.X) + (MousePosition.Y - GP.PlayerList[GP.ID].Position.Y) * (MousePosition.Y - GP.PlayerList[GP.ID].Position.Y))),
             //   ((MousePosition.Y - GP.PlayerList[GP.ID].Position.Y) / (float)Math.Sqrt((MousePosition.X - GP.PlayerList[GP.ID].Position.X) * (MousePosition.X - GP.PlayerList[GP.ID].Position.X) + (MousePosition.Y - GP.PlayerList[GP.ID].Position.Y) * (MousePosition.Y - GP.PlayerList[GP.ID].Position.Y)))));
            
            //GP.PlayerList[GP.ID].PlayerBullet.RemoveAt(j);
        }
        private void Main_MouseUp(object sender, MouseEventArgs e)
        {




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
                    Light.X = (int)(Light.X * Settings.GameSize.Width / (float)this.ClientSize.Height);
                    Light.Y = (int)(Light.Y * Settings.GameSize.Height / (float)this.ClientSize.Height);
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

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.ClientSize.Width < this.ClientSize.Height + 10)
            {
                this.ClientSize = new Size(this.ClientSize.Height + 10,this.ClientSize.Height);
            }

            Format.Width = (float)(this.ClientSize.Width - (this.ClientSize.Width - this.ClientSize.Height)) / Settings.GameSize.Width;
            Format.Height = (float)this.ClientSize.Height / Settings.GameSize.Height;
          //  this.Invalidate();
        }

        
    }
}
