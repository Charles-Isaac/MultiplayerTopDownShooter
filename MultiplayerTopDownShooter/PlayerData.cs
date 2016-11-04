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
        [XmlElement(Order = 5)]
        long m_LastTickUpdate;
        [XmlElement(Order = 6)]
        byte m_Velocite;
        [XmlElement(Order = 7)]
        int m_Couleur;

        [XmlElement(Order = 8)]
        List<Projectile> m_LBullet = new List<Projectile>();

        public PlayerData()
        {
            lock (m_BulletLock)
            {
                m_LastTickUpdate = 0;
                m_Position = new PointF(0, 0);
                m_DirectionRegard = new PointF(0, 0);
                m_DirectionDeplacement = new PointF(0, 0);
                m_ID = 0;
                m_Velocite = 10;
                m_Couleur = Color.Black.ToArgb();
            }
        }
        public PlayerData(byte IDConstructeur, long TickCount)
        {
            m_LastTickUpdate = TickCount;
            m_Position = new PointF(0, 0);
            m_DirectionRegard = new PointF(0, 0);
            m_DirectionDeplacement = new PointF(0, 0);
            m_ID = IDConstructeur;
            m_Velocite = 10;
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

        public void UpdateStats(int TickCount)
        {
            m_Position.X += (int)(m_DirectionDeplacement.X * m_Velocite * (Environment.TickCount - TickCount) / 20);
            m_Position.Y += (int)(m_DirectionDeplacement.Y * m_Velocite * (Environment.TickCount - TickCount) / 20);

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
                m_LBullet[i].UpdateStatus(TickCount);

                if (System.Windows.Forms.Form.ActiveForm == null || m_LBullet[i].Position.X > System.Windows.Forms.Form.ActiveForm.Width || m_LBullet[i].Position.X < 0 || m_LBullet[i].Position.Y > System.Windows.Forms.Form.ActiveForm.Height || m_LBullet[i].Position.Y < 0)
                {
                    lock (m_BulletLock)
                    {
                        m_LBullet.RemoveAt(i);
                    }
                }
            }
      //      Exit();

            m_LastTickUpdate = TickCount;
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
    }
}
