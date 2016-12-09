using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace ClonesEngine
{
    [XmlType]
    class PlayerData
    {

        private readonly object m_BulletLock = new object();
        public object BulletLock
        {
            get { return m_BulletLock; }
        }

        [XmlElement(Order = 1)]
        private byte m_ID;
        [XmlElement(Order = 2)]
        private PointF m_Position;
        [XmlElement(Order = 3)]
        private int m_DirectionRegard;
        [XmlElement(Order = 4)]
        private PointF m_DirectionDeplacement;
        [XmlElement(Order = 5)]
        private byte m_Velocite;
        [XmlElement(Order = 6)]
        private int m_Couleur;
        [XmlElement(Order = 7)]
        private byte m_DeathStatus;
        [XmlElement(Order = 8)]
        private byte m_Size;
        [XmlElement(Order = 9)]
        private int m_Score;


        public Weapons[] WeaponList;


        [XmlElement(Order = 10)]
        private List<Projectile> m_LBullet = new List<Projectile>();

        public PlayerData()
        {
            lock (m_BulletLock)
            {
                CallWeaponConstructor();
                m_Position = new PointF(1, 1);
                m_DirectionRegard = 0;
                m_DirectionDeplacement = new PointF(0, 0);
                m_ID = 0;
                m_DeathStatus = 1;
                m_Velocite = Settings.DefaultPlayerSpeed;               
                Random RNG = new Random();
                m_Couleur = Color.FromArgb(RNG.Next(256), RNG.Next(256), RNG.Next(256)).ToArgb();
                m_Size = Settings.DefaultPlayerSize;
                m_Score = 0;
            }
        }

        public void CallWeaponConstructor()
        {

            WeaponList = new Weapons[(byte)WeaponType.NumberOfWeapons];
            WeaponList[(byte)WeaponType.Pistol] = new Pistol(this);
            WeaponList[(byte)WeaponType.Shotgun] = new Shotgun(this);
            WeaponList[(byte)WeaponType.MachineGun] = new MachineGun(this);
            WeaponList[(byte)WeaponType.Sniper] = new Sniper(this);
            WeaponList[(byte)WeaponType.RocketLauncher] = new RocketLauncher(this);
        }

        public void UpdateWeaponWielder()
        {
            
            WeaponList[(byte)WeaponType.Pistol].User = this;
            WeaponList[(byte)WeaponType.Shotgun].User = this;
            WeaponList[(byte)WeaponType.MachineGun].User = this;
            WeaponList[(byte)WeaponType.Sniper].User = this;
            WeaponList[(byte)WeaponType.RocketLauncher].User = this;
        }

        public byte DeathStatus
        {
            get { return m_DeathStatus; }
            set { m_DeathStatus = value; }
        }

        public PlayerData(byte IDConstructeur)
        {
            m_Position = new PointF(0, 0);
            m_DirectionRegard = 0;
            m_DirectionDeplacement = new PointF(0, 0);
            m_ID = IDConstructeur;
            m_Velocite = Settings.DefaultPlayerSpeed;
            m_Size = Settings.DefaultPlayerSize;
            m_Score = 0;
            Random RNG = new Random();
            m_Couleur = Color.FromArgb(RNG.Next(256), RNG.Next(256), RNG.Next(256)).ToArgb();
        }

        public int Couleur
        {
            get { return m_Couleur; }
            set { m_Couleur = value; }
        }

        public void AddProjectile(Projectile bullet)
        {
            lock (m_BulletLock)
            {
                m_LBullet.Add(bullet);
            }
        }

        public List<PlayerDamage> UpdateStats(int[] OldTime, int NewTime, PlayerData[] Player, int PlayerCount, int ID, Map Murs, Point MousePosition)
        {

            List<PlayerDamage> BulletDamage = new List<PlayerDamage>();

            for (int i = 1; i <= PlayerCount; i++)
            {

                
                if (Player[i].Velocite.X != 0 || Player[i].Velocite.Y != 0)
                {
                    if (Murs != null)
                    {
                        PointF TempPosi = new PointF(Player[i].Position.X + (Player[i].Velocite.X * Player[i].Vitesse * (NewTime - OldTime[i]) / 20), Player[i].Position.Y + (Player[i].Velocite.Y * Player[i].Vitesse * (NewTime - OldTime[i]) / 20));
                        int Timeout = 10;
                        int j = Murs.Murs.Length - 1;
                        for (; j >= 0; j--)
                        {
                            if (Collision.IsIntersecting(TempPosi, Player[i].Position, Murs.Murs[j].A, Murs.Murs[j].B))
                            {

                                PointF VectMur = new PointF(Murs.Murs[j].A.X - Murs.Murs[j].B.X, Murs.Murs[j].A.Y - Murs.Murs[j].B.Y);
                                PointF VectPlayer = new PointF(TempPosi.X - Player[i].Position.X, TempPosi.Y - Player[i].Position.Y);
                                float Dp = VectPlayer.X * VectMur.X + VectPlayer.Y * VectMur.Y;
                                
                                PointF Proj = new PointF((Dp/(VectMur.X*VectMur.X + VectMur.Y*VectMur.Y))*VectMur.X,
                                    (Dp/(VectMur.X*VectMur.X + VectMur.Y*VectMur.Y))*VectMur.Y
                                );
                                if (float.IsNaN(Proj.X))
                                {
                                    Proj.X = 0;
                                }
                                if (float.IsNaN(Proj.Y))
                                {
                                    Proj.Y = 0;
                                    if (Proj.X == 0)
                                    {
                                        continue;
                                    }
                                }
                                if (Player[i].Position.Y <= 0)
                                {
                                    Player[i].Position = new PointF(Player[i].Position.X, 1);
                                }
                                if (Player[i].Position.Y >= Settings.GameSize.Height)
                                {
                                    Player[i].Position = new PointF(Player[i].Position.X, Settings.GameSize.Height - 1);
                                }
                                if (Player[i].Position.X <= 0)
                                {
                                    Player[i].Position = new PointF(1, Player[i].Position.Y);
                                }
                                if (Player[i].Position.X >= Settings.GameSize.Width)
                                {
                                    Player[i].Position = new PointF(Settings.GameSize.Width - 1, Player[i].Position.Y);
                                }

                                TempPosi = new PointF(Player[i].Position.X + Proj.X, Player[i].Position.Y + Proj.Y);
                                Timeout--;
                                if (Timeout >= 0)
                                {
                                    j = Murs.Murs.Length;

                                }


                            }

                        }

                        Player[i].Position = TempPosi;
                    }   
                }
            }
            Player[ID].m_DirectionRegard = (int)(-((Math.Atan2(MousePosition.X - Player[ID].Position.X, MousePosition.Y - Player[ID].Position.Y) * 180 / Math.PI) - 180)+7.5)%360;


          
            lock (m_BulletLock)
            {
                for (int i = m_LBullet.Count - 1; i >= 0; i--)
                {
                    PointF OldPosition = m_LBullet[i].Position;
                    bool Exist = true;

                    m_LBullet[i].UpdateStatus(OldTime[ID], NewTime);
                    

                    for (byte j = 1; j <= PlayerCount; j++)
                    {
                        if (Exist && j != ID && Collision.PlayerBulletCollision(Player[j], OldPosition, m_LBullet[i].Position))
                        {
                            
                            m_Score++;
                            if (m_LBullet[i].TypeOfProjectile == (byte)ProjectileType.Rocket)
                            {
                                ((RocketLauncher)WeaponList[(byte)WeaponType.RocketLauncher]).BringTheHavoc();
                            }
                            else
                            {
                               m_LBullet.RemoveAt(i);
                                Player[j].Position = new PointF(0,0);
                            }

                         
                            Exist = false;
                            BulletDamage.Add(new PlayerDamage(j, 1));
                            
                        }
                    }

                    if (Exist && Murs != null)
                    {
                        for (int j = Murs.Murs.Length - 1; j >= 0 && Exist; j--)
                        {
                            if (Collision.IsIntersecting(OldPosition, m_LBullet[i].Position, Murs.Murs[j].A, Murs.Murs[j].B))
                            {
                                if (m_LBullet[i].TypeOfProjectile == (byte)ProjectileType.Rocket)
                                {
                                    m_LBullet[i].Position = OldPosition;
                                    ((RocketLauncher)WeaponList[(byte)WeaponType.RocketLauncher]).BringTheHavoc();
                                }
                                else
                                {
                                    m_LBullet.RemoveAt(i);
                                }
                                Exist = false;
                                
                            }
                        }
                        if (Exist && (m_LBullet[i].Position.X > Settings.GameSize.Width + 10 || m_LBullet[i].Position.Y > Settings.GameSize.Height + 10 || m_LBullet[i].Position.X < -10 || m_LBullet[i].Position.X < -10))
                        {
                            m_LBullet.RemoveAt(i);
                        }
                    }
                }
            }

            

            return BulletDamage;
        }
        public int Score
        {
        get { return m_Score; }
            set { m_Score = value; }
        }
        public byte Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }
        public PointF Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        public int DirectionRegard
        {
            get { return m_DirectionRegard; }
            set { m_DirectionRegard = value; }
        }
        public PointF Velocite
        {
            get { return m_DirectionDeplacement; }
            set { m_DirectionDeplacement = value; }
        }
        public List<Projectile> Bullet
        {
            get { return m_LBullet; }
            set { m_LBullet = value; }
        }
        public byte Vitesse
        {
            get { return m_Velocite; }
            set { m_Velocite = value; }
        }
    }
}
