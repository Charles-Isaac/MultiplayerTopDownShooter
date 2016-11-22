using ClonesEngine;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MultiplayerTopDownShooter
{

    public partial class Main : Form
    {
     //  private readonly Random m_RNG;
        private readonly GestionnaireDePacket m_GP;
        private int m_ChronoFPS;
        private int m_CompteurFPS;
        private int m_FPS;
        private readonly int m_Lobby = 54545;

        private SizeF m_Format = new SizeF(1, 1);

        private readonly SolidBrush m_Br;
        private readonly Point[] m_ShadowPolygon = new Point[6];
        private Point[,] m_ShadowArray;

        private readonly Bitmap[] m_PlayersImage;
        private readonly Bitmap[] m_TerrainImage;
        private readonly Bitmap[] m_WeaponsImage;

       // Image image = new Bitmap(Properties.Resources.ShadowTexture);
        TextureBrush tBrush;

        //     System.Threading.Timer WeaponTimer;
        public Main()
        {
            InitializeComponent();

            //TransparencyKey = Color.Black;
            using (Bitmap WeaponBitmapPistol = new Bitmap(Properties.Resources.ShadowTexture))
            {
                tBrush = new TextureBrush(WeaponBitmapPistol.Clone(new Rectangle(0, 0, WeaponBitmapPistol.Width, WeaponBitmapPistol.Height), PixelFormat.Format32bppPArgb));

                //tBrush.ScaleTransform((float)Settings.GameSize.Width / WeaponBitmapPistol.Width, (float)Settings.GameSize.Height / WeaponBitmapPistol.Height, System.Drawing.Drawing2D.MatrixOrder.Append);
            }
            frmLobbyPrompt frmLobby = new frmLobbyPrompt();
            frmLobby.ShowDialog();
            if (frmLobby.DialogResult == DialogResult.OK)
            {
                m_Lobby = frmLobby.Lobby;
            }
            WindowState = FormWindowState.Maximized;
       //     m_RNG = new Random();
            m_GP = new GestionnaireDePacket(m_Lobby);
       //     DoubleBuffered = true;

            m_ChronoFPS = Environment.TickCount;

            m_PlayersImage = new Bitmap[24];
            m_TerrainImage = new Bitmap[1];
            m_WeaponsImage = new Bitmap[(byte)WeaponType.NumberOfWeapons];
            for (int i = 0; i < 24; i++)
            {
                using (Bitmap PlayerBitmap = new Bitmap(Properties.Resources.ResourceManager.GetObject("Player" + (i*15).ToString().PadLeft(3, '0'), Properties.Resources.Culture) as Image))
                    m_PlayersImage[i] = PlayerBitmap.Clone(new Rectangle(0, 0, PlayerBitmap.Width, PlayerBitmap.Height), PixelFormat.Format32bppPArgb);
            }

            for (int i = 0; i < 1; i++)
            {
                using (Bitmap GroundBitmap = new Bitmap(Properties.Resources.ResourceManager.GetObject("GroundTexture2", Properties.Resources.Culture) as Image))
                    m_TerrainImage[i] = GroundBitmap.Clone(new Rectangle(0, 0, GroundBitmap.Width, GroundBitmap.Height), PixelFormat.Format32bppPArgb);
            }



            using (Bitmap WeaponBitmapPistol = new Bitmap(Properties.Resources.Pistol))
                m_WeaponsImage[(byte)WeaponType.Pistol] = WeaponBitmapPistol.Clone(new Rectangle(0, 0, WeaponBitmapPistol.Width, WeaponBitmapPistol.Height), PixelFormat.Format32bppPArgb);

            using (Bitmap WeaponBitmapMachineGun = new Bitmap(Properties.Resources.MachineGun))
                m_WeaponsImage[(byte)WeaponType.MachineGun] = WeaponBitmapMachineGun.Clone(new Rectangle(0, 0, WeaponBitmapMachineGun.Width, WeaponBitmapMachineGun.Height), PixelFormat.Format32bppPArgb);

            using (Bitmap WeaponBitmapShotgun = new Bitmap(Properties.Resources.Shotgun))
                m_WeaponsImage[(byte)WeaponType.Shotgun] = WeaponBitmapShotgun.Clone(new Rectangle(0, 0, WeaponBitmapShotgun.Width, WeaponBitmapShotgun.Height), PixelFormat.Format32bppPArgb);

            using (Bitmap WeaponBitmapSniper = new Bitmap(Properties.Resources.Sniper))
                m_WeaponsImage[(byte)WeaponType.Sniper] = WeaponBitmapSniper.Clone(new Rectangle(0, 0, WeaponBitmapSniper.Width, WeaponBitmapSniper.Height), PixelFormat.Format32bppPArgb);

            using (Bitmap WeaponBitmapRocketLauncher = new Bitmap(Properties.Resources.RocketLauncher))
                m_WeaponsImage[(byte)WeaponType.RocketLauncher] = WeaponBitmapRocketLauncher.Clone(new Rectangle(0, 0, WeaponBitmapRocketLauncher.Width, WeaponBitmapRocketLauncher.Height), PixelFormat.Format32bppPArgb);



            m_Br = new SolidBrush(Color.Black);// new TextureBrush(Properties.Resources.ShadowTexture, new Rectangle(0, 0, 250, 250));
            /*

                        var autoEvent = new AutoResetEvent(false);
                        m_GP.StatusChecker(10);

                        // Create a timer that invokes CheckStatus after one second, 
                        // and every 1/4 second thereafter.

                        WeaponTimer = new System.Threading.Timer(m_GP.CheckStatus,
                                                   autoEvent, 1000, 250);
                        WeaponTimer.*/
            //   WeaponTimer = new System.Threading.Timer(autoEvent);






          //  this.BackgroundImage = TerrainImage[0];//Properties.Resources.GroundTexture2;
       }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].MouseUp();
            m_GP.SelectedWeapon += (byte)(e.Delta / 120);
            if (m_GP.SelectedWeapon == 255)
            {
                m_GP.SelectedWeapon = (byte)WeaponType.NumberOfWeapons - 1;
            }
            else
            {
                if (m_GP.SelectedWeapon > (byte)WeaponType.NumberOfWeapons - 1)
                {
                    m_GP.SelectedWeapon = 0;
                }
            }
            base.OnMouseWheel(e);
        }
        private void Main_Paint(object sender, PaintEventArgs e)
        {
            Invoke(new Action(() =>
            {

                e.Graphics.ScaleTransform((float)ClientSize.Height / Settings.GameSize.Width, (float)ClientSize.Height / Settings.GameSize.Height, System.Drawing.Drawing2D.MatrixOrder.Append);
                // e.Graphics.DrawImage(Properties.Resources.GroundTexture1,0,0,Settings.GameSize.Width,Settings.GameSize.Height);
                e.Graphics.DrawImage(m_TerrainImage[0], new Rectangle(0, 0, Settings.GameSize.Width, Settings.GameSize.Height), new Rectangle(0, 0, 250, 250), GraphicsUnit.Pixel);



                for (int i = 1; i <= m_GP.PlayerCount; i++)
                {
                    //Murs
                    // e.Graphics.DrawImage((Image)Properties.Resources.ResourceManager.GetObject("Player" + ((int)((m_GP.PlayerList[i].DirectionRegard + 7.5) / 15) * 15).ToString().PadLeft(3,'0'), Properties.Resources.Culture), m_GP.PlayerList[i].Position.X - 45, m_GP.PlayerList[i].Position.Y - 45);
                    e.Graphics.DrawImage(m_PlayersImage[m_GP.PlayerList[i].DirectionRegard / 15], m_GP.PlayerList[i].Position.X - 45, m_GP.PlayerList[i].Position.Y - 45);



                    // e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(m_GP.PlayerList[i].Couleur)),
                    //      m_GP.PlayerList[i].Position.X - m_GP.PlayerList[i].Size / 2, m_GP.PlayerList[i].Position.Y - m_GP.PlayerList[i].Size / 2, m_GP.PlayerList[i].Size, m_GP.PlayerList[i].Size);
                    lock (m_GP.PlayerList[i].BulletLock)
                    {
                        /*  Point MousePosition1 = this.PointToClient(Cursor.Position);
                          MousePosition1.X = (int)(MousePosition1.X * Settings.GameSize.Width / (float)this.ClientSize.Height);
                          MousePosition1.Y = (int)(MousePosition1.Y * Settings.GameSize.Height / (float)this.ClientSize.Height);

                          m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].MouseDown(MousePosition1);*/
                        for (int j = m_GP.PlayerList[i].Bullet.Count - 1; j >= 0; j--)
                        {
                            if (m_GP.PlayerList[i].Bullet[j].TypeOfProjectile == (byte)ProjectileType.Rocket)
                            {
                                e.Graphics.FillEllipse(new SolidBrush(Color.Red/*FromArgb(m_GP.PlayerList[i].Couleur)*/), m_GP.PlayerList[i].Bullet[j].Position.X - Settings.DefaultBulletSize / 2, m_GP.PlayerList[i].Bullet[j].Position.Y - Settings.DefaultBulletSize / 2, Settings.DefaultBulletSize, Settings.DefaultBulletSize);
                            }
                            else
                            {
                                e.Graphics.FillEllipse(new SolidBrush(Color.Yellow/*FromArgb(m_GP.PlayerList[i].Couleur)*/), m_GP.PlayerList[i].Bullet[j].Position.X - Settings.DefaultBulletSize / 2, m_GP.PlayerList[i].Bullet[j].Position.Y - Settings.DefaultBulletSize / 2, Settings.DefaultBulletSize, Settings.DefaultBulletSize);
                            }



                            e.Graphics.DrawEllipse(new Pen(Color.Black/*FromArgb(m_GP.PlayerList[i].Couleur)*/, Settings.DefaultBulletSize / 4), m_GP.PlayerList[i].Bullet[j].Position.X - Settings.DefaultBulletSize / 2, m_GP.PlayerList[i].Bullet[j].Position.Y - Settings.DefaultBulletSize / 2, Settings.DefaultBulletSize, Settings.DefaultBulletSize);
                        }
                    }

                }
                if (m_GP.Map != null)
                {
                    m_ShadowArray = Shadows.ReturnMeAnArray(m_GP.Map.Murs.Length, m_GP.Map.Murs, m_GP.PlayerList[m_GP.ID].Position, Settings.GameSize.Width, Settings.GameSize.Height);

                    for (int i = m_GP.Map.Murs.Length - 1; i >= 0; i--)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            m_ShadowPolygon[j] = m_ShadowArray[i, j];
                        }
                        //try
                        {
                            
                            e.Graphics.FillPolygon(tBrush, m_ShadowPolygon);
                        }
                        //catch { MessageBox.Show("Erreur Ombre"); }
                    }
                }




                /*

                Image image = new Bitmap(Properties.Resources.ShadowTexture);
                TextureBrush tBrush = new TextureBrush(image);
                tBrush.ScaleTransform((float)Settings.GameSize.Width / image.Width, (float)Settings.GameSize.Height / image.Height, System.Drawing.Drawing2D.MatrixOrder.Append);
                /* tBrush.Transform = new Matrix(
                    1.0f / 640.0f,
                    0.0f,
                    0.0f,
                    0.1f / 480.0f,
                    0.0f,
                    0.0f);
                e.Graphics.FillRectangle(tBrush, new Rectangle(PointToClient(Cursor.Position).X, 0, Settings.GameSize.Width/2, Settings.GameSize.Height));

                e.Graphics.DrawString(((float)PointToClient(Cursor.Position).Y / 100).ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 250, 160);
                */






                if (m_GP.Map != null)
                {
                    for (int j = m_GP.Map.Murs.Length - 1; j >= 0; j--)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Green, 5.0F), m_GP.Map.Murs[j].A, m_GP.Map.Murs[j].B);
                    }
                }






                if (Environment.TickCount - m_ChronoFPS > 1000)
                {
                    m_FPS = m_CompteurFPS;
                    m_ChronoFPS = Environment.TickCount;
                    m_CompteurFPS = 0;
                }
                m_CompteurFPS++;
                e.Graphics.DrawString("FPS: " + m_FPS.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 160);
                e.Graphics.DrawString("Your game ID: " + m_GP.ID.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 10);
                e.Graphics.DrawString("Your score: " + m_GP.PlayerList[m_GP.ID].Score.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 60);
                e.Graphics.DrawString("Where you are looking: " + ((int)((m_GP.PlayerList[m_GP.ID].DirectionRegard + 7.5) / 15) * 15).ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 110);
                for (int i = 1; i < m_GP.PlayerCount + 1; i++)
                {
                    e.Graphics.DrawString("Player " + i.ToString() + " score: " + m_GP.PlayerList[i].Score.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 700 + 50 * i);
                }


                Point PositionSourie = PointToClient(Cursor.Position);
                PositionSourie.X = (int)(PositionSourie.X * Settings.GameSize.Width / (float)ClientSize.Height);
                PositionSourie.Y = (int)(PositionSourie.Y * Settings.GameSize.Height / (float)ClientSize.Height);


                if (m_GP.PlayerList[m_GP.ID].WeaponList != null)
                {
                    e.Graphics.DrawString(m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].WeaponName, new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 310);
                    e.Graphics.DrawString(m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].NBulletInCharger.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 210);
                    if (m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].NBulletLeft < 1000)
                    {
                        e.Graphics.DrawString(m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].NBulletLeft.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 260);
                    }
                    else
                    {
                        e.Graphics.DrawString("∞", new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 260);
                    }



                    // Thread.Sleep(250); //Lag gen
                    m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].MouseDirection = new PointF(((PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X) / (float)Math.Sqrt((PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X) * (PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X) + (PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y))),
                   ((PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y) / (float)Math.Sqrt((PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X) * (PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X) + (PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y))));
                    e.Graphics.DrawLine(new Pen(Color.Red), m_GP.PlayerList[m_GP.ID].Position, PositionSourie);

                    e.Graphics.DrawImage(m_WeaponsImage[m_GP.SelectedWeapon], Settings.GameSize.Width + 70, 400);

                }

                m_GP.UpdatePlayer(m_GP.ID, PositionSourie);
                m_GP.Send(TramePreGen.InfoJoueur(m_GP.PlayerList[m_GP.ID], m_GP.ID, m_GP.PacketID));

                /*if (MouseButtons == MouseButtons.Left)
                {
                    Main_MouseDown(sender, new MouseEventArgs(MouseButtons.Left,1,10,10,1));
                }*/
                //Thread.Sleep(RNG.Next(100)); //random ping generator ;)

                //             e.Graphics.DrawImage(Properties.Resources.Glock17, Settings.GameSize.Width+10, 0);
                //Invalidate();
                Invalidate(new Rectangle(new Point(0, 0), new Size(ClientSize.Width, ClientSize.Height)));
            }));

        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            Point MousePositionByForm = PointToClient(Cursor.Position);
            MousePositionByForm.X = (int)(MousePositionByForm.X * Settings.GameSize.Width / (float)ClientSize.Height);
            MousePositionByForm.Y = (int)(MousePositionByForm.Y * Settings.GameSize.Height / (float)ClientSize.Height);
            m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].MouseDown(new PointF(((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) / (float)Math.Sqrt((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) * (MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) + (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y))),
                ((MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) / (float)Math.Sqrt((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) * (MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) + (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y)))));

       //     m_GP.PlayerList[m_GP.ID].AjouterProjectile(new PointF(((MousePosition.X - m_GP.PlayerList[m_GP.ID].Position.X) / (float)Math.Sqrt((MousePosition.X - m_GP.PlayerList[m_GP.ID].Position.X) * (MousePosition.X - m_GP.PlayerList[m_GP.ID].Position.X) + (MousePosition.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (MousePosition.Y - m_GP.PlayerList[m_GP.ID].Position.Y))),
             //   ((MousePosition.Y - m_GP.PlayerList[m_GP.ID].Position.Y) / (float)Math.Sqrt((MousePosition.X - m_GP.PlayerList[m_GP.ID].Position.X) * (MousePosition.X - m_GP.PlayerList[m_GP.ID].Position.X) + (MousePosition.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (MousePosition.Y - m_GP.PlayerList[m_GP.ID].Position.Y)))));
            
            //m_GP.PlayerList[m_GP.ID].PlayerBullet.RemoveAt(j);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point MousePositionByForm = PointToClient(Cursor.Position);
            MousePositionByForm.X = (int)(MousePositionByForm.X * Settings.GameSize.Width / (float)ClientSize.Height);
            MousePositionByForm.Y = (int)(MousePositionByForm.Y * Settings.GameSize.Height / (float)ClientSize.Height);
            m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].MouseDown(new PointF(((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) / (float)Math.Sqrt((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) * (MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) + (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y))),
                ((MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) / (float)Math.Sqrt((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) * (MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) + (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y)))));

        }

        
        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].MouseUp();



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
                case Keys.R:
                    m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.SelectedWeapon].Reload();
                    break;
                case Keys.Escape:
                    Environment.Exit(0);
                    break;
                case Keys.Space:
                    ChangeArrowsState(ArrowsPressed.Space, true);
                    Point Light = PointToClient(Cursor.Position);
                    Light.X = (int)(Light.X * Settings.GameSize.Width / (float)ClientSize.Height);
                    Light.Y = (int)(Light.Y * Settings.GameSize.Height / (float)ClientSize.Height);
                    m_GP.PlayerList[m_GP.ID].Position = Light;
                    break;
                case Keys.Return:

                    m_GP.Send(new[] {(byte)PacketUse.ResetAllID, m_GP.ID});
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

            if ((m_ArrowsPressed & ArrowsPressed.Up) == ArrowsPressed.None && (m_ArrowsPressed & ArrowsPressed.Down) != ArrowsPressed.None)
            {
                m_GP.PlayerList[m_GP.ID].Velocite = new PointF(m_GP.PlayerList[m_GP.ID].Velocite.X, 1);
                //VY = 1;
            }

            if ((m_ArrowsPressed & ArrowsPressed.Down) == ArrowsPressed.None && (m_ArrowsPressed & ArrowsPressed.Up) != ArrowsPressed.None)
            {
                m_GP.PlayerList[m_GP.ID].Velocite = new PointF(m_GP.PlayerList[m_GP.ID].Velocite.X, -1);
                //VY = -1;
            }

            if ((m_ArrowsPressed & ArrowsPressed.Right) == ArrowsPressed.None && (m_ArrowsPressed & ArrowsPressed.Left) != ArrowsPressed.None)
            {
                m_GP.PlayerList[m_GP.ID].Velocite = new PointF(-1, m_GP.PlayerList[m_GP.ID].Velocite.Y);
                //VX = -1;
            }

            if ((m_ArrowsPressed & ArrowsPressed.Left) == ArrowsPressed.None && (m_ArrowsPressed & ArrowsPressed.Right) != ArrowsPressed.None)
            {
                m_GP.PlayerList[m_GP.ID].Velocite = new PointF(1, m_GP.PlayerList[m_GP.ID].Velocite.Y);
                //VX = 1;
            }

            if (((m_ArrowsPressed & ArrowsPressed.Up) | (m_ArrowsPressed & ArrowsPressed.Down)) == ArrowsPressed.None)
            {
                m_GP.PlayerList[m_GP.ID].Velocite = new PointF(m_GP.PlayerList[m_GP.ID].Velocite.X, 0);
                //VY = 0;
            }

            if (((m_ArrowsPressed & ArrowsPressed.Right) | (m_ArrowsPressed & ArrowsPressed.Left)) == ArrowsPressed.None)
            {
                m_GP.PlayerList[m_GP.ID].Velocite = new PointF(0, m_GP.PlayerList[m_GP.ID].Velocite.Y);
                //VX = 0;
            }

            //  Do whatever is needed using position
        }
        [Flags]
        private enum ArrowsPressed
        {
            None = 0x00,
            Left = 0x01,
            Right = 0x02,
            Up = 0x04,
            Down = 0x08,
            Space = 0x10,
          //  Escape = 0x20,
            All = 0x3F
        }
        private ArrowsPressed m_ArrowsPressed;
        private void ChangeArrowsState(ArrowsPressed changed, bool IsPressed)
        {
            if (IsPressed)
            {
                m_ArrowsPressed |= changed;
            }
            else
            {
                m_ArrowsPressed &= ArrowsPressed.All ^ changed;
            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {

            if (ClientSize.Width < ClientSize.Height + 10)
            {
                ClientSize = new Size(ClientSize.Height + 10,ClientSize.Height);
            }

            m_Format.Width = (float)(ClientSize.Width - (ClientSize.Width - ClientSize.Height)) / Settings.GameSize.Width;
            m_Format.Height = (float)ClientSize.Height / Settings.GameSize.Height;
            Bitmap tempBitmap = new Bitmap(Properties.Resources.ShadowTexture);
            tBrush.ScaleTransform((float)Settings.GameSize.Width / tempBitmap.Width, (float)Settings.GameSize.Height / tempBitmap.Height, System.Drawing.Drawing2D.MatrixOrder.Append);
            //  this.Invalidate();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        
    }
}
