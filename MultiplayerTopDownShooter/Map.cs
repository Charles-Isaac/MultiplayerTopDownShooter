using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClonesEngine
{
    class Map
    {
        private readonly Mur[] m_TMurs;

        public Map(int Seed)
        {
            List<Mur> FutureMap = new List<Mur>();
            Random RNG = new Random(Seed);

            
            FutureMap.Add(new Mur(new Point(0, -0), new Point(0, Settings.GameSize.Height + 0)));

            FutureMap.Add(new Mur(new Point(Settings.GameSize.Width, Settings.GameSize.Height + 0),
                new Point(Settings.GameSize.Width, -0)));

            FutureMap.Add(new Mur(new Point(Settings.GameSize.Width + 0, Settings.GameSize.Height),
                new Point(-0, Settings.GameSize.Height)));

            FutureMap.Add(new Mur(new Point(Settings.GameSize.Width + 0, 0), new Point(-0, 0)));
       

            int TryCount = 0;
            for (int i = 0; i < Settings.NumberOfBuilding && TryCount < 1000; i++)
            {

                int NumberOfWallAdded;
                int PreviousWallCount = FutureMap.Count;
                switch (RNG.Next(3))
                {
                    case 0:
                        FutureMap.AddRange(BuildingTemplate.Couloir(new Point(RNG.Next(1, Settings.GameSize.Width), RNG.Next(1, Settings.GameSize.Height)), RNG.NextDouble() * 360, out NumberOfWallAdded));

                        break;
                    case 1:
                        FutureMap.AddRange(BuildingTemplate.Fence(new Point(RNG.Next(1, Settings.GameSize.Width), RNG.Next(1, Settings.GameSize.Height)), RNG.NextDouble() * 360, out NumberOfWallAdded));

                        break;
                    default:
                        FutureMap.AddRange(BuildingTemplate.Couloir(new Point(RNG.Next(1, Settings.GameSize.Width), RNG.Next(1, Settings.GameSize.Height)), RNG.NextDouble() * 360, out NumberOfWallAdded));

                        break;
                }
               
                int j = 0;

                while (
                    !CheckForIntersection(FutureMap[j/NumberOfWallAdded].A, FutureMap[j/NumberOfWallAdded].B,
                        FutureMap[PreviousWallCount + j%NumberOfWallAdded].A,
                        FutureMap[PreviousWallCount + j%NumberOfWallAdded].B) &&
                    j < (NumberOfWallAdded*PreviousWallCount))
                {
                    j++;
                    
                }

                if (j != NumberOfWallAdded * PreviousWallCount)
                {
                    FutureMap.RemoveRange(PreviousWallCount, NumberOfWallAdded);
                    i--;
                }
               


            }
            if (TryCount >= 100)
            {
                MessageBox.Show("Somehow the map generation didn't work");
                throw new Exception("Map Generation Redundancy Error");
            }

         
            m_TMurs = FutureMap.ToArray();
        }
        public Mur[] Murs
        {
            get { return m_TMurs; }
        }


        private static bool CheckForIntersection(PointF VecteurA1, PointF VecteurA2, PointF VecteurB1, PointF VecteurB2) //Retourne un booléen indiquant si la ligne tracé entre VecteurA1 et VecteurA2 intersectionne la ligne tracé entre VecteurB1 et VecteurB2 
        {
            if (VecteurB1.X < 0 || VecteurB1.X > Settings.GameSize.Width || VecteurB1.Y < 0 || VecteurB1.Y > Settings.GameSize.Height)
            {
                return true;
            }

            float Denominator = ((VecteurA2.X - VecteurA1.X) * (VecteurB2.Y - VecteurB1.Y)) - ((VecteurA2.Y - VecteurA1.Y) * (VecteurB2.X - VecteurB1.X));
            float Numerator1 = ((VecteurA1.Y - VecteurB1.Y) * (VecteurB2.X - VecteurB1.X)) - ((VecteurA1.X - VecteurB1.X) * (VecteurB2.Y - VecteurB1.Y));
            float Numerator2 = ((VecteurA1.Y - VecteurB1.Y) * (VecteurA2.X - VecteurA1.X)) - ((VecteurA1.X - VecteurB1.X) * (VecteurA2.Y - VecteurA1.Y));

            if (Denominator == 0) return false;

            float R = Numerator1 / Denominator;
            float S = Numerator2 / Denominator;

            return (R >= 0 && R <= 1) && (S >= 0 && S <= 1);
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
