using System;
using System.Drawing;

namespace ClonesEngine
{
    static class BuildingTemplate
    {
        public static Mur[] Couloir(Point Position, double RotationDegre, out int NumberOfWallAdded)
        {
            Mur[] MapCouloir = new Mur[4];

            //Couloir en L  
            {
                MapCouloir[0] = new Mur
                {
                    A = new Point { X = 0, Y = 0 },
                    B = new Point { X = 0, Y = 300 }
                };
                MapCouloir[1] = new Mur
                {
                    A = new Point { X = 100, Y = 0 },
                    B = new Point { X = 100, Y = 200 }
                };
                MapCouloir[2] = new Mur
                {
                    A = new Point { X = 100, Y = 200 },
                    B = new Point { X = 300, Y = 200 }
                };
                MapCouloir[3] = new Mur
                {
                    A = new Point { X = 0, Y = 300 },
                    B = new Point { X = 300, Y = 300 }
                };
            }


            MapCouloir = MapCouloir.Translate(Position);
            MapCouloir = MapCouloir.Rotate(RotationDegre);
            NumberOfWallAdded = 4;
            return MapCouloir;
        }

        public static Mur[] Fence(Point Position, double RotationDegre, out int NumberOfWallAdded)
        {
            Mur[] MapCouloir = new Mur[4];

            //Couloir en --  
            {
                MapCouloir[0] = new Mur
                {
                    A = new Point { X = 0, Y = 0 },
                    B = new Point { X = 100, Y = 0 }
                };
                MapCouloir[1] = new Mur
                {
                    A = new Point { X = 150, Y = 0 },
                    B = new Point { X = 250, Y = 0 }
                };
                MapCouloir[2] = new Mur
                {
                    A = new Point { X = 300, Y = 50 },
                    B = new Point { X = 400, Y = 0 }
                };
                MapCouloir[3] = new Mur
                {
                    A = new Point { X = 450, Y = 100 },
                    B = new Point { X = 550, Y = 0 }
                };
            }


            MapCouloir = MapCouloir.Translate(Position);
            MapCouloir = MapCouloir.Rotate(RotationDegre);
            NumberOfWallAdded = 4;
            return MapCouloir;
        }

      
      
        private static Mur[] Rotate(this Mur[] MurToTrans, double Angle)
        {
            double AngleInRadians = Angle * (Math.PI / 180);
            double CosTheta = Math.Cos(AngleInRadians);
            double SinTheta = Math.Sin(AngleInRadians);

            for (int i = 0; i < MurToTrans.Length; i++)
            {
                MurToTrans[i].A = new Point
                {
                    X =
                    (int)
                    (CosTheta * (MurToTrans[i].A.X - MurToTrans[0].A.X) -
            SinTheta * (MurToTrans[i].A.Y - MurToTrans[0].A.Y) + MurToTrans[0].A.X),
                    Y =
                    (int)
                    (SinTheta * (MurToTrans[i].A.X - MurToTrans[0].A.X) +
            CosTheta * (MurToTrans[i].A.Y - MurToTrans[0].A.Y) + MurToTrans[0].A.Y)
                };
                MurToTrans[i].B = new Point
                {
                    X =
                    (int)
                    (CosTheta * (MurToTrans[i].B.X - MurToTrans[0].A.X) -
            SinTheta * (MurToTrans[i].B.Y - MurToTrans[0].A.Y) + MurToTrans[0].A.X),
                    Y =
                    (int)
                    (SinTheta * (MurToTrans[i].B.X - MurToTrans[0].A.X) +
            CosTheta * (MurToTrans[i].B.Y - MurToTrans[0].A.Y) + MurToTrans[0].A.Y)
                };
            }
            return MurToTrans;
        }

        private static Mur[] Translate(this Mur[] MurToTrans, Point Glissement)
        {
            for (int i = MurToTrans.Length - 1; i >= 0; i--)
            {
                MurToTrans[i] = new Mur
                {
                    A = new Point {X = MurToTrans[i].A.X + Glissement.X, Y = MurToTrans[i].A.Y + Glissement.Y},
                    B = new Point {X = MurToTrans[i].B.X + Glissement.X, Y = MurToTrans[i].B.Y + Glissement.Y}
                };
            }

            return MurToTrans;
        }

    }
}
