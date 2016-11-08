using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;
using System.Threading;

namespace ClonesEngine
{
    [XmlType]
    class PlayerData
    {

        private static object m_BulletLock = new object();
        public object BulletLock
        {
            get { return m_BulletLock; }
        }

        [XmlElement(Order = 1)]
        byte m_ID;
        [XmlElement(Order = 2)]
        PointF m_Position;
        [XmlElement(Order = 3)]
        PointF m_DirectionRegard;
        [XmlElement(Order = 4)]
        PointF m_DirectionDeplacement;
       // [XmlElement(Order = 5)]
       // long m_LastTickUpdate;
        [XmlElement(Order = 6)]
        byte m_Velocite;
        [XmlElement(Order = 7)]
        int m_Couleur;
        [XmlElement(Order = 8)]
        byte m_DeathStatus;
        [XmlElement(Order = 9)]
        byte m_Size;


        [XmlElement(Order = 10)]
        List<Projectile> m_LBullet = new List<Projectile>();

        public PlayerData()
        {
            lock (m_BulletLock)
            {
                //m_LastTickUpdate = 0;
                m_Position = new PointF(0, 0);
                m_DirectionRegard = new PointF(0, 0);
                m_DirectionDeplacement = new PointF(0, 0);
                m_ID = 0;
                m_DeathStatus = 1;
                m_Velocite = 10;
                m_Couleur = Color.Black.ToArgb();
                m_Size = 10;
            }
        }
        public byte DeathStatus
        {
            get { return m_DeathStatus; }
            set { m_DeathStatus = value; }
        }

        public PlayerData(byte IDConstructeur, long TickCount)
        {
            //m_LastTickUpdate = TickCount;
            m_Position = new PointF(0, 0);
            m_DirectionRegard = new PointF(0, 0);
            m_DirectionDeplacement = new PointF(0, 0);
            m_ID = IDConstructeur;
            m_Velocite = 10;
            m_Size = 10;
        }

        public int Couleur
        {
            get { return m_Couleur; }
            set { m_Couleur = value; }
        }

        public void AjouterProjectile(PointF Direction)
        {
            m_LBullet.Add(new Projectile(m_Position, Direction, 1, Environment.TickCount));
        }

        public List<PlayerDamage> UpdateStats(int[] OldTime, int NewTime, PlayerData[] Player, int PlayerCount, int ID)
        {
            List<PlayerDamage> BulletDamage = new List<PlayerDamage>();

            for (int i = 1; i <= PlayerCount; i++)
            {
                Player[i].Position = new PointF(Player[i].Position.X + (Player[i].Velocite.X * Player[i].Vitesse * (NewTime - OldTime[i]) / 20),
                    Player[i].Position.Y + (Player[i].Velocite.Y * Player[i].Vitesse * (NewTime - OldTime[i]) / 20));
                Collision.PlayerBorder(Player[i]);
            }

            
            /*
            if (System.Windows.Forms.Form.ActiveForm == null || m_Position.X > System.Windows.Forms.Form.ActiveForm.Width || m_Position.X < 0 || m_LBullet[i].Position.Y > System.Windows.Forms.Form.ActiveForm.Height || m_LBullet[i].Position.Y < 0)
            {
                m_LBullet.RemoveAt(i);
            }
            /*  for (int i = m_LBullet.Count - 1; i > 0; i--)
              {
                  m_LBullet[i].Position = new Point((m_LBullet[i].Position.X * m_LBullet[i].Velocite * (Environment.TickCount - TickCount) / 20 + m_LBullet[i].Position.X), (m_LBullet[i].Position.Y * m_LBullet[i].Velocite * (Environment.TickCount - TickCount) / 20 + m_LBullet[i].Position.Y));
              }
              */
            //      Enter();
            for (int i = m_LBullet.Count - 1; i > 0; i--)
            {
                PointF OldPosition = m_LBullet[i].Position;

                m_LBullet[i].UpdateStatus(OldTime[ID], NewTime);
                if (float.IsNaN(m_LBullet[i].Position.X) || (m_LBullet[i].Position.X > System.Windows.Forms.Form.ActiveForm?.Width || m_LBullet[i].Position.X < 0 || m_LBullet[i].Position.Y > System.Windows.Forms.Form.ActiveForm?.Height || m_LBullet[i].Position.Y < 0)) //Crash without the '?'
                {
                    lock (m_BulletLock)
                    {
                        m_LBullet.RemoveAt(i);
                    }
                }
                else
                {
                    for (byte j = 1; j <= PlayerCount; j++)
                    {
                        if (j != ID && Collision.PlayerBulletCollision(Player[j], OldPosition, m_LBullet[i].Position))
                        {
                            lock (m_BulletLock)
                            {
                                m_LBullet.RemoveAt(i);
                                BulletDamage.Add(new PlayerDamage(j, 1));
                            }
                        }
                    }
                }

                
            }

            return BulletDamage;
      //      Exit();

            //m_LastTickUpdate = OldTime;
        }
  /*      private bool Locked = false;
        public void Enter()
        {
            // Yield until acquired is false
            while (Locked) { Thread.Sleep(1); }
            Locked = true;
        }
        public void Exit()
        {
            Locked = false;
        }
        public byte ID
        {
            get { return m_ID; }
        }
  */
        public  byte Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }
        public PointF Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public PointF Velocite
        {
            get { return m_DirectionDeplacement; }
            set { m_DirectionDeplacement = value; }
        }
        public List<Projectile> Bullet
        {
            get { return m_LBullet; }
        }
        public byte Vitesse
        {
            get { return m_Velocite; }
            set { m_Velocite = value; }
        }
    }
}
