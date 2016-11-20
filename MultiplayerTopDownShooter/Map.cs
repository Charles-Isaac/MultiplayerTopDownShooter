using System;
using System.Drawing;

namespace ClonesEngine
{
    // [XmlType]
    class Map
    {
        int NombreDeMurs = Settings.NumberOfWalls;
        Mur[] m_TMurs;

        public Map(int Seed)
        {
            m_TMurs = new Mur[NombreDeMurs + 4];
            Random RNG = new Random(Seed);
            int j;
            for (int i = 0; i < NombreDeMurs + 4; i++)
            {
                // Murs[i].A = new PointF(RNG.Next(Settings.DefaultResolution.X);
                m_TMurs[i] = new Mur(new Point(RNG.Next(5, Settings.GameSize.Width - 5), RNG.Next(5, Settings.GameSize.Height - 5)), new Point(RNG.Next(5, Settings.GameSize.Width - 5), RNG.Next(5, Settings.GameSize.Height - 5)));

                j = 0;
                while (!Collision.IsIntersecting(m_TMurs[i].A, m_TMurs[i].B, m_TMurs[j].A, m_TMurs[j].B) && j < i)
                    j++;
                if (j != i)
                {
                    i--;
                }
            }
            for (int i = 0; i < NombreDeMurs; i++)
            {
                if (m_TMurs[i].A.X < m_TMurs[i].B.X)
                {
                    Point TempVar = m_TMurs[i].A;
                    m_TMurs[i].A = m_TMurs[i].B;
                    m_TMurs[i].B = TempVar;
                }
            }
            m_TMurs[NombreDeMurs/* + 0*/].A = new Point(0, -5);
            m_TMurs[NombreDeMurs/* + 0*/].B = new Point(0, Settings.GameSize.Height + 5);

            m_TMurs[NombreDeMurs + 1].A = new Point(Settings.GameSize.Width, Settings.GameSize.Height + 5);
            m_TMurs[NombreDeMurs + 1].B = new Point(Settings.GameSize.Width, -5);

            m_TMurs[NombreDeMurs + 2].A = new Point(Settings.GameSize.Width + 5, Settings.GameSize.Height);
            m_TMurs[NombreDeMurs + 2].B = new Point(-5, Settings.GameSize.Height);

            m_TMurs[NombreDeMurs + 3].A = new Point(Settings.GameSize.Width + 5, 0);
            m_TMurs[NombreDeMurs + 3].B = new Point(-5, 0);
        }
        public Mur[] Murs
        {
            get { return m_TMurs; }
        }
    }
   // [XmlType]
    class Mur
    {
        Point m_A;
        Point m_B;

        public Mur()
        {
            m_A = new Point();
            m_B = new Point();
        }
        public Mur(Point A, Point B)
        {
            m_A = A;
            m_B = B;
        }

        public Point A
        {
            get { return m_A; }
            set { m_A = value; }
        }
        public Point B
        {
            get { return m_B; }
            set { m_B = value; }
        }
    }
}
