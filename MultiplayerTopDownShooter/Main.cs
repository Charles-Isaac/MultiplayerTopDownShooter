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
        private readonly GestionnaireDePacket m_GP;
        private int m_ChronoFPS;
        private int m_CompteurFPS;
        private int m_FPS;
        private readonly int m_Lobby = 54545;

        private SizeF m_Format = new SizeF(1, 1);


        private readonly Bitmap[] m_PlayersImage;
        private readonly Bitmap[] m_TerrainImage;
        private readonly Bitmap[] m_WeaponsImage;

        TextureBrush m_TBrush;

        public Main()
        {
            InitializeComponent();

            //creation de la brush utilisé pour dessiner les ombres
            using (Bitmap BitmapShadow = new Bitmap(Properties.Resources.ShadowTexture))
            {
                m_TBrush =
                    new TextureBrush(BitmapShadow.Clone(new Rectangle(0, 0, BitmapShadow.Width, BitmapShadow.Height),
                        PixelFormat.Format32bppPArgb));

            }
            frmLobbyPrompt frmLobby = new frmLobbyPrompt(); //formulaire utilisé pour choisir la salle
            frmLobby.ShowDialog();
            if (frmLobby.DialogResult == DialogResult.OK)
            {
                m_Lobby = frmLobby.Lobby;
            }
            else
            {
                Load += (s, e) => Close(); //Ferme l'application
            }
            WindowState = FormWindowState.Maximized;

            m_GP = new GestionnaireDePacket(m_Lobby);

            m_ChronoFPS = Environment.TickCount;

            m_PlayersImage = new Bitmap[24];
            m_TerrainImage = new Bitmap[1];
            m_WeaponsImage = new Bitmap[(byte) WeaponType.NumberOfWeapons];
            for (int i = 0; i < 24; i++)//Creation de la texture des joueurs
            {
                using (
                    Bitmap PlayerBitmap =
                        new Bitmap(
                            Properties.Resources.ResourceManager.GetObject(
                                "Player" + (i*15).ToString().PadLeft(3, '0'), Properties.Resources.Culture) as Image))
                    m_PlayersImage[i] = PlayerBitmap.Clone(
                        new Rectangle(0, 0, PlayerBitmap.Width, PlayerBitmap.Height), PixelFormat.Format32bppPArgb);
            }

            for (int i = 0; i < 1; i++)//Creation de la texture du sol
            {
                using (
                    Bitmap GroundBitmap =
                        new Bitmap(
                            Properties.Resources.ResourceManager.GetObject("GroundTexture",
                                Properties.Resources.Culture) as Image))
                    m_TerrainImage[i] = GroundBitmap.Clone(
                        new Rectangle(0, 0, GroundBitmap.Width, GroundBitmap.Height), PixelFormat.Format32bppPArgb);
            }


            #region TextureArmes
            using (Bitmap WeaponBitmapPistol = new Bitmap(Properties.Resources.Pistol))
                m_WeaponsImage[(byte) WeaponType.Pistol] =
                    WeaponBitmapPistol.Clone(new Rectangle(0, 0, WeaponBitmapPistol.Width, WeaponBitmapPistol.Height),
                        PixelFormat.Format32bppPArgb);

            using (Bitmap WeaponBitmapMachineGun = new Bitmap(Properties.Resources.MachineGun))
                m_WeaponsImage[(byte) WeaponType.MachineGun] =
                    WeaponBitmapMachineGun.Clone(
                        new Rectangle(0, 0, WeaponBitmapMachineGun.Width, WeaponBitmapMachineGun.Height),
                        PixelFormat.Format32bppPArgb);

            using (Bitmap WeaponBitmapShotgun = new Bitmap(Properties.Resources.Shotgun))
                m_WeaponsImage[(byte) WeaponType.Shotgun] =
                    WeaponBitmapShotgun.Clone(
                        new Rectangle(0, 0, WeaponBitmapShotgun.Width, WeaponBitmapShotgun.Height),
                        PixelFormat.Format32bppPArgb);

            using (Bitmap WeaponBitmapSniper = new Bitmap(Properties.Resources.Sniper))
                m_WeaponsImage[(byte) WeaponType.Sniper] =
                    WeaponBitmapSniper.Clone(new Rectangle(0, 0, WeaponBitmapSniper.Width, WeaponBitmapSniper.Height),
                        PixelFormat.Format32bppPArgb);

            using (Bitmap WeaponBitmapRocketLauncher = new Bitmap(Properties.Resources.RocketLauncher))
                m_WeaponsImage[(byte) WeaponType.RocketLauncher] =
                    WeaponBitmapRocketLauncher.Clone(
                        new Rectangle(0, 0, WeaponBitmapRocketLauncher.Width, WeaponBitmapRocketLauncher.Height),
                        PixelFormat.Format32bppPArgb);


            #endregion TextureArmes
            
        }

        protected override void OnMouseWheel(MouseEventArgs e) //Change l'arme selectionnee
        {
            m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.m_SelectedWeapon].MouseUp();
            m_GP.m_SelectedWeapon += (byte) (e.Delta/120);
            if (m_GP.m_SelectedWeapon == 255)
            {
                m_GP.m_SelectedWeapon = (byte) WeaponType.NumberOfWeapons - 1;
            }
            else
            {
                if (m_GP.m_SelectedWeapon > (byte) WeaponType.NumberOfWeapons - 1)
                {
                    m_GP.m_SelectedWeapon = 0;
                }
            }
            base.OnMouseWheel(e);
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            //Adapte le jeu a la taille de l'ecran
            e.Graphics.ScaleTransform((float) ClientSize.Height/Settings.GameSize.Width*Settings.GameStretchX,
                (float) ClientSize.Height/Settings.GameSize.Height*Settings.GameStretchY, MatrixOrder.Append); 
            //dessine le sol
            e.Graphics.DrawImage(m_TerrainImage[0],
                new Rectangle(0, 0, Settings.GameSize.Width, Settings.GameSize.Height), new Rectangle(0, 0, 250, 250),
                GraphicsUnit.Pixel);


            //parcourt la totalité des joueurs
            for (int i = 1; i <= m_GP.PlayerCount; i++)
            {
                //Dessine le joueur
                e.Graphics.DrawImage(m_PlayersImage[m_GP.PlayerList[i].DirectionRegard / 15],
                        m_GP.PlayerList[i].Position.X - 45, m_GP.PlayerList[i].Position.Y - 45);
                
                lock (m_GP.PlayerList[i].BulletLock) //Entre dans la section critique
                {
                    for (int j = m_GP.PlayerList[i].Bullet.Count - 1; j >= 0; j--) //dessine toutes les balles
                    {
                        if (m_GP.PlayerList[i].Bullet[j].TypeOfProjectile == (byte) ProjectileType.Rocket)
                        {
                            e.Graphics.FillEllipse(new SolidBrush(Color.Red /*FromArgb(m_GP.PlayerList[i].Couleur)*/),
                                m_GP.PlayerList[i].Bullet[j].Position.X - Settings.DefaultBulletSize/2,
                                m_GP.PlayerList[i].Bullet[j].Position.Y - Settings.DefaultBulletSize/2,
                                Settings.DefaultBulletSize, Settings.DefaultBulletSize);
                        }
                        else
                        {
                            e.Graphics.FillEllipse(
                                new SolidBrush(Color.Yellow /*FromArgb(m_GP.PlayerList[i].Couleur)*/),
                                m_GP.PlayerList[i].Bullet[j].Position.X - Settings.DefaultBulletSize/2,
                                m_GP.PlayerList[i].Bullet[j].Position.Y - Settings.DefaultBulletSize/2,
                                Settings.DefaultBulletSize, Settings.DefaultBulletSize);
                        }



                        e.Graphics.DrawEllipse(
                            new Pen(Color.Black /*FromArgb(m_GP.PlayerList[i].Couleur)*/, Settings.DefaultBulletSize/4),
                            m_GP.PlayerList[i].Bullet[j].Position.X - Settings.DefaultBulletSize/2,
                            m_GP.PlayerList[i].Bullet[j].Position.Y - Settings.DefaultBulletSize/2,
                            Settings.DefaultBulletSize, Settings.DefaultBulletSize);
                    }
                }

            }

            if (m_GP.Map != null)
            {

                for (int j = m_GP.Map.Murs.Length - 1; j >= 0; j--)
                {
                    e.Graphics.DrawLine(new Pen(Color.White, 25.0F), m_GP.Map.Murs[j].A, m_GP.Map.Murs[j].B);
                }
                //calcule toutes les ombres
                PointF[] ShadowArray = Shadows.ReturnMeAnArray(m_GP.Map.Murs, m_GP.PlayerList[m_GP.ID].Position);

                //Dessine le polygone calcule avec la brosse d'ombre
        
                e.Graphics.FillPolygon(m_TBrush, ShadowArray);
                
            }

            //dessine tous les murs si la map a ete genere
           
            
            //affiche les FPS
            if (Environment.TickCount - m_ChronoFPS > 1000)
            {
                m_FPS = m_CompteurFPS;
                m_ChronoFPS = Environment.TickCount;
                m_CompteurFPS = 0;
            }
            m_CompteurFPS++;

            e.Graphics.DrawString("FPS: " + m_FPS.ToString(), new Font("Arial", 30), new SolidBrush(Color.Black),
                Settings.GameSize.Width + 70, 160);
            e.Graphics.DrawString("Your game ID: " + m_GP.ID.ToString(), new Font("Arial", 30),
                new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 10);
            e.Graphics.DrawString("Your score: " + m_GP.PlayerList[m_GP.ID].Score.ToString(), new Font("Arial", 30),
                new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 60);
           
            for (int i = 1; i < m_GP.PlayerCount + 1; i++)
            {
                e.Graphics.DrawString("Player " + i.ToString() + " score: " + m_GP.PlayerList[i].Score.ToString(),
                    new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 700 + 50*i);
            }


            Point PositionSourie = PointToClient(Cursor.Position);
            PositionSourie.X = (int) (PositionSourie.X*Settings.GameSize.Width/(float) ClientSize.Height / Settings.GameStretchX);
            PositionSourie.Y = (int) (PositionSourie.Y*Settings.GameSize.Height/(float) ClientSize.Height / Settings.GameStretchY);


            if (m_GP.PlayerList[m_GP.ID].WeaponList != null)
            {
                e.Graphics.DrawString(m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.m_SelectedWeapon].WeaponName,
                    new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 310);
                e.Graphics.DrawString("Balles dans le chargeur: " +
                    m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.m_SelectedWeapon].NBulletInCharger.ToString(),
                    new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 210);

                if (m_GP.PlayerList[m_GP.ID].WeaponList?[m_GP.m_SelectedWeapon].NBulletLeft < 1000)
                {
                    e.Graphics.DrawString("Balles restante: " + 
                        m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.m_SelectedWeapon].NBulletLeft.ToString(),
                        new Font("Arial", 30), new SolidBrush(Color.Black), Settings.GameSize.Width + 70, 260);
                }
                else
                {
                    e.Graphics.DrawString("Balles restante: ∞", new Font("Arial", 30), new SolidBrush(Color.Black),
                        Settings.GameSize.Width + 70, 260);
                }



                m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.m_SelectedWeapon].MouseDirection =
                    new PointF(
                        ((PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X)/
                         (float)
                         Math.Sqrt((PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X)*
                                   (PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X) +
                                   (PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y)*
                                   (PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y))),
                        ((PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y)/
                         (float)
                         Math.Sqrt((PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X)*
                                   (PositionSourie.X - m_GP.PlayerList[m_GP.ID].Position.X) +
                                   (PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y)*
                                   (PositionSourie.Y - m_GP.PlayerList[m_GP.ID].Position.Y))));

                e.Graphics.DrawLine(new Pen(Color.Red), m_GP.PlayerList[m_GP.ID].Position, PositionSourie);

                e.Graphics.DrawImage(m_WeaponsImage[m_GP.m_SelectedWeapon], Settings.GameSize.Width + 70, 400);

            }

            m_GP.UpdatePlayer(m_GP.ID, PositionSourie);
            m_GP.Send(TramePreGen.InfoJoueur(m_GP.PlayerList[m_GP.ID], m_GP.ID, m_GP.PacketID));


            Invalidate(/*new Rectangle(new Point(0, 0), new Size(ClientSize.Width, ClientSize.Height))*/);
        }



        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            Point MousePositionByForm = PointToClient(Cursor.Position);
            MousePositionByForm.X = (int)(MousePositionByForm.X * Settings.GameSize.Width / (float)ClientSize.Height / Settings.GameStretchX);
            MousePositionByForm.Y = (int)(MousePositionByForm.Y * Settings.GameSize.Height / (float)ClientSize.Height / Settings.GameStretchY);
            m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.m_SelectedWeapon]?.MouseDown(new PointF(((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) / (float)Math.Sqrt((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) * (MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) + (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y))),
                ((MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) / (float)Math.Sqrt((MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) * (MousePositionByForm.X - m_GP.PlayerList[m_GP.ID].Position.X) + (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y) * (MousePositionByForm.Y - m_GP.PlayerList[m_GP.ID].Position.Y)))));
        }


        
        private void Main_MouseUp(object sender, MouseEventArgs e)
        {


            m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.m_SelectedWeapon].MouseUp();
            
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.End:
                    m_GP.NewMap();
                    break;
                case Keys.Next:
                    m_GP.PlayerList[m_GP.ID].Vitesse++;
                    break;
                case Keys.D1:
                    m_GP.m_SelectedWeapon = 0;
                    break;
                case Keys.D2:
                    m_GP.m_SelectedWeapon = 1;
                    break;
                case Keys.D3:
                    m_GP.m_SelectedWeapon = 2;
                    break;
                case Keys.D4:
                    m_GP.m_SelectedWeapon = 3;
                    break;
                case Keys.D5:
                    m_GP.m_SelectedWeapon = 4;
                    break;
                
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
                    m_GP.PlayerList[m_GP.ID].WeaponList[m_GP.m_SelectedWeapon].Reload();
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Space:
                    ChangeArrowsState(ArrowsPressed.Space, true);
                    Point Light = PointToClient(Cursor.Position);
                    Light.X = (int)(Light.X * Settings.GameSize.Width / (float)ClientSize.Height / Settings.GameStretchX);
                    Light.Y = (int)(Light.Y * Settings.GameSize.Height / (float)ClientSize.Height / Settings.GameStretchY);
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
            using (Bitmap WeaponBitmapPistol = new Bitmap(Properties.Resources.ShadowTexture))
            {
                m_TBrush = new TextureBrush(WeaponBitmapPistol.Clone(new Rectangle(0, 0, WeaponBitmapPistol.Width, WeaponBitmapPistol.Height), PixelFormat.Format32bppPArgb));

            }
            m_TBrush.ScaleTransform((float)Settings.GameSize.Width / tempBitmap.Width, (float)Settings.GameSize.Height / tempBitmap.Height, MatrixOrder.Append);
            
        }

        
    }
}
