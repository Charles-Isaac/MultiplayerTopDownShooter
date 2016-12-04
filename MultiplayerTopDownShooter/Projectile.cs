using System.Drawing;
using System.Xml.Serialization;

namespace ClonesEngine
{
    [XmlType]
    class Projectile
    {
        private PointF m_Position;
        private PointF m_DirectionDeplacement;
        private byte m_Velocite;
        private byte m_TypeOfProjectile;
        public Projectile()
        {
            //it may not looks like it but this methode is useful, do not erase
        }
        
        public Projectile(PointF Position, PointF DirectionDeplacement, byte Velocite, byte TypeOfProjectile)
        {
            m_TypeOfProjectile = TypeOfProjectile;
            m_Position = Position;
            m_DirectionDeplacement = DirectionDeplacement;
            m_Velocite = Velocite;
        }

        public void UpdateStatus(int OldTime, int NewTime)
        {
            m_Position.X += (m_DirectionDeplacement.X * m_Velocite * (NewTime - OldTime) / 20);
            m_Position.Y += (m_DirectionDeplacement.Y * m_Velocite * (NewTime - OldTime) / 20);
            //m_LastTickUpdate = LastTickCount;
            
        }

        #region Propriete
        public byte TypeOfProjectile
        {
            get { return m_TypeOfProjectile; }
            set { m_TypeOfProjectile = value; }
        }
        public PointF Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        public PointF Direction
        {
            get { return m_DirectionDeplacement; }
            set { m_DirectionDeplacement = value; }
        }
        public byte Velocite
        {
            get { return m_Velocite; }
            set { m_Velocite = value; }
        }
        #endregion
    }
}
