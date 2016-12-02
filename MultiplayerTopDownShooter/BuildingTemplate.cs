using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClonesEngine
{
    static class BuildingTemplate
    {
        public static Mur[] Couloir(Point Position, double RotationDegre)
        {
            Mur[] MapCouloir = new Mur[4];

            //Couloir en L
            {
                MapCouloir[0] = new Mur
                {
                    A = new Point {X = 0, Y = 0},
                    B = new Point {X = 0, Y = 100}
                };
                MapCouloir[0] = new Mur
                {
                    A = new Point {X = 50, Y = 0},
                    B = new Point {X = 50, Y = 50}
                };
                MapCouloir[0] = new Mur
                {
                    A = new Point {X = 50, Y = 50},
                    B = new Point {X = 100, Y = 50}
                };
                MapCouloir[0] = new Mur
                {
                    A = new Point {X = 0, Y = 100},
                    B = new Point {X = 100, Y = 100}
                };
            }


            MapCouloir = MapCouloir.Translate(Position);
            MapCouloir = MapCouloir.Rotate(RotationDegre);
            return new Mur[0];
        }

        private static Mur[] Rotate(this Mur[] MurToTrans, double Angle)
        {
            double angleInRadians = Angle * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);

            for (int i = 0; i < MurToTrans.Length; i++)
            {
                MurToTrans[i].A = new Point
                {
                    X =
                    (int)
                    (cosTheta * (MurToTrans[i].A.X) -
                    sinTheta * (MurToTrans[i].A.Y)),
                    Y =
                    (int)
                    (sinTheta * (MurToTrans[i].A.X) +
                    cosTheta * (MurToTrans[i].A.Y))
                };
            }
            return MurToTrans;
        }

        private static Mur[] Translate(this Mur[] MurToTrans, Point TargetedPosition)
        {
            return MurToTrans;
        }

    }
}
