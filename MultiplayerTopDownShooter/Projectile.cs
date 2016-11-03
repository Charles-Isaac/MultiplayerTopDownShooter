﻿using System;
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
    class Projectile
    {
        private Point m_Position;
        private PointF m_DirectionDeplacement;
        byte m_Velocite;
        int m_LastTickUpdate;
        public Projectile()
        {
            m_Position = new Point(0, 0);
            m_DirectionDeplacement = new Point(0, 0);
            m_Velocite = 20;
            m_LastTickUpdate = LastTickUpdate;
        }
        public Projectile(int LastTickCount)
        {
            m_Position = new Point(0, 0);
            m_DirectionDeplacement = new PointF(0, 0);
            m_Velocite = 20;
            m_LastTickUpdate = LastTickCount;
        }

        public Projectile(Point Position, PointF DirectionDeplacement, byte Velocite, int LastTickCount)
        {
            m_Position = Position;
            m_DirectionDeplacement = DirectionDeplacement;
            m_Velocite = Velocite;
            m_LastTickUpdate = LastTickCount;
        }

        public void UpdateStatus(long LastTickCount)
        {
            m_Position.X += (int)(m_DirectionDeplacement.X * m_Velocite * (LastTickCount - m_LastTickUpdate) / 20);
            m_Position.Y += (int)(m_DirectionDeplacement.Y * m_Velocite * (LastTickCount - m_LastTickUpdate) / 20);
            
            
        }

        #region Propriete

        public Point Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        public PointF Direction
        {
            get { return m_DirectionDeplacement; }
            set { m_DirectionDeplacement = value; }
        }
        public int LastTickUpdate
        {
            get { return m_LastTickUpdate; }
            set { m_LastTickUpdate = value; }
        }
        public byte Velocite
        {
            get { return m_Velocite; }
            set { m_Velocite = value; }
        }
        #endregion
    }
}
