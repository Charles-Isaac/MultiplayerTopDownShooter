using System;
using System.Drawing;

namespace ClonesEngine
{
    class Map
    {
        private readonly int m_NombreDeMurs = Settings.NumberOfWalls;
        private readonly Mur[] m_TMurs;

        public Map(int Seed)
        {
            m_TMurs = new Mur[m_NombreDeMurs + 4];
            Random RNG = new Random(Seed);
            for (int i = 0; i < m_NombreDeMurs + 4; i++)
            {
                m_TMurs[i] = new Mur(new Point(RNG.Next(5, Settings.GameSize.Width - 5), RNG.Next(5, Settings.GameSize.Height - 5)), new Point(RNG.Next(5, Settings.GameSize.Width - 5), RNG.Next(5, Settings.GameSize.Height - 5)));

                int j = 0;
                while (!Collision.IsIntersecting(m_TMurs[i].A, m_TMurs[i].B, m_TMurs[j].A, m_TMurs[j].B) && j < i)
                    j++;
                if (j != i)
                {
                    i--;
                }
            }
            for (int i = 0; i < m_NombreDeMurs; i++)
            {
                if (m_TMurs[i].A.X > m_TMurs[i].B.X)
                {
                    Point TempVar = m_TMurs[i].A;
                    m_TMurs[i].A = m_TMurs[i].B;
                    m_TMurs[i].B = TempVar;
                }
            }


            m_TMurs[m_NombreDeMurs].A = new Point(0, -0);
            m_TMurs[m_NombreDeMurs].B = new Point(0, Settings.GameSize.Height + 0);

            m_TMurs[m_NombreDeMurs + 1].A = new Point(Settings.GameSize.Width, Settings.GameSize.Height + 0);
            m_TMurs[m_NombreDeMurs + 1].B = new Point(Settings.GameSize.Width, -0);

            m_TMurs[m_NombreDeMurs + 2].A = new Point(Settings.GameSize.Width + 0, Settings.GameSize.Height);
            m_TMurs[m_NombreDeMurs + 2].B = new Point(-0, Settings.GameSize.Height);

            m_TMurs[m_NombreDeMurs + 3].A = new Point(Settings.GameSize.Width + 0, 0);
            m_TMurs[m_NombreDeMurs + 3].B = new Point(-0, 0);


            for (int i = m_NombreDeMurs; i < m_NombreDeMurs + 4; i++) //fool devs loop
            {
                if (m_TMurs[i].A.X > m_TMurs[i].B.X)
                {
                    Point TempVar = m_TMurs[i].A;
                    m_TMurs[i].A = m_TMurs[i].B;
                    m_TMurs[i].B = TempVar;
                }
            }


        }
        public Mur[] Murs
        {
            get { return m_TMurs; }
        }
    }
    class Mur
    {
        private Point m_A;
        private Point m_B;

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
