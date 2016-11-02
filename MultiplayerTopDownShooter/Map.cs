using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClonesEngine
{
    class Map
    {
        int NombreDeMurs = 20;
        Mur[] Murs;

        public Map()
        {
            Murs = new Mur[NombreDeMurs];
            Random RNG = new Random();
            for (int i = 0; i < NombreDeMurs; i++)
            {
               // Murs[i].A = new Point(RNG.Next(Settings.DefaultResolution.X);
                     
            }

        }

        
    }
    class Mur
    {
        Point m_A;
        Point m_B;
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
