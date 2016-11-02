using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace ClonesEngine
{
    [XmlType]
    class PlayerData
    {
        [XmlElement(Order = 1)]
        byte m_ID;
        [XmlElement(Order = 2)]
        Point m_Position;
        [XmlElement(Order = 3)]
        Point m_DirectionRegard;
        [XmlElement(Order = 4)]
        PointF m_DirectionDeplacement;
        [XmlElement(Order = 5)]
        long m_LastTickUpdate;
        [XmlElement(Order = 6)]
        byte m_Velocite;
        [XmlElement(Order = 7)]
        int m_Couleur;

        /*[XmlElement(Order = 8)]
        List<Projectile> Bullet;
        */
        public PlayerData()
        {
            m_LastTickUpdate = 0;
            m_Position = new Point(0, 0);
            m_DirectionRegard = new Point(0, 0);
            m_DirectionDeplacement = new PointF(0, 0);
            m_ID = 0;
            m_Velocite = 10;
            m_Couleur = Color.Black.ToArgb();
        }
        public PlayerData(byte IDConstructeur, long TickCount)
        {
            m_LastTickUpdate = TickCount;
            m_Position = new Point(0, 0);
            m_DirectionRegard = new Point(0, 0);
            m_DirectionDeplacement = new PointF(0, 0);
            m_ID = IDConstructeur;
            m_Velocite = 10;
        }

        public int Couleur
        {
            get { return m_Couleur; }
            set { m_Couleur = value; }
        }

        public void AjouterProjectile(long TickCount)
        {
            //Bullet.Add(new Projectile(TickCount));
        }

        public void UpdateStats(long TickCount)
        {
            m_Position.X += (int)(m_DirectionDeplacement.X * m_Velocite * (TickCount - m_LastTickUpdate) / 20);
            m_Position.Y += (int)(m_DirectionDeplacement.Y * m_Velocite * (TickCount - m_LastTickUpdate) / 20);



            /* for (int i = 0; i < Bullet.Count; i++)
             {
                 Bullet[i].UpdateStatus(TickCount);
             }*/

            m_LastTickUpdate = TickCount;
        }

        public byte ID
        {
            get { return m_ID; }
        }

        public Point Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public PointF Velocite
        {
            get { return m_DirectionDeplacement; }
            set { m_DirectionDeplacement = value; }
        }
    }
}
