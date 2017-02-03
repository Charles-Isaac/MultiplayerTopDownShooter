using System.Drawing;
using ClonesEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerTopDownShooter
{
    static class Shadows
    {
        public static PointF[] ReturnMeAnArray(Mur[] WallX, PointF LightSource) //retourn un array 2D de point correspondant au polygones des ombres
        {
            List<PointF> ListeDePointIntersectionOmbre = new List<PointF>();
            List<double> ListeAngle = new List<double>();
            
            for (int j = 0; j < WallX.Length; j++)
            {
                double Angle = Math.Atan2(WallX[j].A.Y - LightSource.Y, WallX[j].A.X - LightSource.X);
                ListeAngle.Add(Angle - 0.0001);
             //   ListeAngle.Add(Angle);
                ListeAngle.Add(Angle + 0.0001);

                Angle = Math.Atan2(WallX[j].B.Y - LightSource.Y, WallX[j].B.X - LightSource.X);
                ListeAngle.Add(Angle - 0.0001);
             //   ListeAngle.Add(Angle);
                ListeAngle.Add(Angle + 0.0001);
            }
            for (int j = ListeAngle.Count - 1; j >= 0; j--)
            {
                double dx = Math.Cos(ListeAngle[j]);
                double dy = Math.Sin(ListeAngle[j]);

                PointF RayA = new PointF(LightSource.X, LightSource.Y);
                DoublePoint RayB = new DoublePoint(LightSource.X + dx, LightSource.Y + dy);

                PointWVect ClosestIntersect = null;
                for (int i = WallX.Length - 1; i >= 0; i--)
                {
                    PointWVect Intersect = GetIntersection(RayA, RayB, WallX[i]);
                    if (Intersect == null) continue;
                    if (ClosestIntersect == null || Intersect.Longueur < ClosestIntersect.Longueur)
                    {
                        ClosestIntersect = Intersect;
                    }
                }

                if (ClosestIntersect != null) // Add to list of intersects
                {
                    ListeDePointIntersectionOmbre.Add((PointF) ClosestIntersect);
                }
            }
            
            
            ListeDePointIntersectionOmbre = ListeDePointIntersectionOmbre.OrderBy(x => Math.Atan2(x.X - LightSource.X, x.Y - LightSource.Y)).ToList();

            for (int i = ListeDePointIntersectionOmbre.Count - 1; i >= 1; i--)
            {
                
                    if ((Math.Abs(ListeDePointIntersectionOmbre[i].Y - ListeDePointIntersectionOmbre[i-1].Y) <= 0.4) && (Math.Abs(ListeDePointIntersectionOmbre[i].X - ListeDePointIntersectionOmbre[i-1].X) <= 0.4))
                    {
                        ListeDePointIntersectionOmbre.RemoveAt(i-1);
                        i--;
                    }
                
            }

            for (int i = ListeDePointIntersectionOmbre.Count - 1; i >= 2; i--)
            {
                if (Math.Abs(ListeDePointIntersectionOmbre[i].X *
                    (ListeDePointIntersectionOmbre[i-1].Y - ListeDePointIntersectionOmbre[i-2].Y) + 
                    ListeDePointIntersectionOmbre[i-1].X *
                    (ListeDePointIntersectionOmbre[i-2].Y - ListeDePointIntersectionOmbre[i].Y) +
                    ListeDePointIntersectionOmbre[i-2].X *
                    (ListeDePointIntersectionOmbre[i].Y - ListeDePointIntersectionOmbre[i-1].Y)) <= 8.4)
                {
                    ListeDePointIntersectionOmbre.RemoveAt(i-1);
                }
            }
           
  //          ListeDePointIntersectionOmbre = ListeDePointIntersectionOmbre.OrderBy(x => Math.Atan2(x.X - LightSource.X, x.Y - LightSource.Y)).ToList();
            ListeDePointIntersectionOmbre.Add(ListeDePointIntersectionOmbre[0]);
            ListeDePointIntersectionOmbre.Add(new PointF( LightSource.X, -0));
            ListeDePointIntersectionOmbre.Add(new PointF(Settings.GameSize.Width + 0, -0));
            ListeDePointIntersectionOmbre.Add(new PointF(Settings.GameSize.Width + 0, Settings.GameSize.Height + 0));
            ListeDePointIntersectionOmbre.Add(new PointF(-0, Settings.GameSize.Height + 0));


            ListeDePointIntersectionOmbre.Add(new PointF(-0, -0));
            
            ListeDePointIntersectionOmbre.Add(new PointF(LightSource.X, -0));

         //   ListeDePointIntersectionOmbre.Add(ListeDePointIntersectionOmbre[ListeDePointIntersectionOmbre.Count - 7/*Copie le derier de la liste avant les ajouts*/]);
            //double test = Math.Atan2(ListeDePointIntersectionOmbre[0].X - LightSource.X, ListeDePointIntersectionOmbre[0].Y - LightSource.Y);
            return ListeDePointIntersectionOmbre.ToArray();

        }
        private static PointWVect GetIntersection(PointF RayA, DoublePoint RayB, Mur Segment)
        {

            // RAY in parametric: Point + Delta*T1
            double r_px = RayA.X;
            double r_py = RayA.Y;
            double r_dx = RayB.X - RayA.X;
            double r_dy = RayB.Y - RayA.Y;

            // SEGMENT in parametric: Point + Delta*T2
            int s_px = Segment.A.X;
            int s_py = Segment.A.Y;
            int s_dx = Segment.B.X - Segment.A.X;
            int s_dy = Segment.B.Y - Segment.A.Y;

            // Are they parallel? If so, no intersect
            double r_mag = Math.Sqrt(r_dx * r_dx + r_dy * r_dy);
            double s_mag = Math.Sqrt(s_dx * s_dx + s_dy * s_dy);
            if (r_dx / r_mag == s_dx / s_mag && r_dy / r_mag == s_dy / s_mag)
            {
                // Unit vectors are the same.
                return null;
            }

            // SOLVE FOR T1 & T2
            // r_px+r_dx*T1 = s_px+s_dx*T2 && r_py+r_dy*T1 = s_py+s_dy*T2
            // ==> T1 = (s_px+s_dx*T2-r_px)/r_dx = (s_py+s_dy*T2-r_py)/r_dy
            // ==> s_px*r_dy + s_dx*T2*r_dy - r_px*r_dy = s_py*r_dx + s_dy*T2*r_dx - r_py*r_dx
            // ==> T2 = (r_dx*(s_py-r_py) + r_dy*(r_px-s_px))/(s_dx*r_dy - s_dy*r_dx)
            double T2 = (r_dx * (s_py - r_py) + r_dy * (r_px - s_px)) / (s_dx * r_dy - s_dy * r_dx);
            double T1 = (s_px + s_dx * T2 - r_px) / r_dx;

            // Must be within parametic whatevers for RAY/SEGMENT
            if (T1 < 0) return null;
            if (T2 < 0 || T2 > 1) return null;

            // Return the POINT OF INTERSECTION
            return new PointWVect((r_px + r_dx * T1), (r_py + r_dy * T1), T1);

        }

        private class DoublePoint
        {
            private double m_X;
            private double m_Y;

            public DoublePoint(double X, double Y)
            {
                m_X = X;
                m_Y = Y;
            }

            public double X
            {
                get
                {
                    return m_X;
                }

                set
                {
                    m_X = value;
                }
            }

            public double Y
            {
                get
                {
                    return m_Y;
                }

                set
                {
                    m_Y = value;
                }
            }
        }



        private class PointWVect
        {
            private double m_X;
            private double m_Y;
            private double m_Longueur;

            public PointWVect(double X, double Y, double Longueur)
            {
                m_X = X;
                m_Y = Y;
                m_Longueur = Longueur;
            }

            public static explicit operator PointF(PointWVect PWV)
            {
                return new PointF((float)PWV.m_X,(float)PWV.m_Y);
            }

            public double X
            {
                get
                {
                    return m_X;
                }

                set
                {
                    m_X = value;
                }
            }

            public double Y
            {
                get
                {
                    return m_Y;
                }

                set
                {
                    m_Y = value;
                }
            }

            public double Longueur
            {
                get
                {
                    return m_Longueur;
                }

                set
                {
                    m_Longueur = value;
                }
            }
        }
    }
    
}
