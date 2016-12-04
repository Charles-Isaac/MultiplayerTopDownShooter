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
                    A = new Point { X = 0, Y = 0 },
                    B = new Point { X = 0, Y = 100 }
                };
                MapCouloir[0] = new Mur
                {
                    A = new Point { X = 50, Y = 0 },
                    B = new Point { X = 50, Y = 50 }
                };
                MapCouloir[0] = new Mur
                {
                    A = new Point { X = 50, Y = 50 },
                    B = new Point { X = 100, Y = 50 }
                };
                MapCouloir[0] = new Mur
                {
                    A = new Point { X = 0, Y = 100 },
                    B = new Point { X = 100, Y = 100 }
                };
            }


            MapCouloir = MapCouloir.Translate(Position);
            MapCouloir = MapCouloir.Rotate(RotationDegre);
            return new Mur[0];
        }

        public static Mur[] Yolo(Point Position, double RotationDegre)
        {
            Mur[] MapYolo = new Mur[80];

            //Couloir en yolo  

            MapYolo[0] = new Mur { A = new Point { X = 267, Y = 139 }, B = new Point { X = 269, Y = 140 } };
            MapYolo[1] = new Mur { A = new Point { X = 265, Y = 139 }, B = new Point { X = 267, Y = 139 } };
            MapYolo[2] = new Mur { A = new Point { X = 261, Y = 140 }, B = new Point { X = 265, Y = 139 } };
            MapYolo[3] = new Mur { A = new Point { X = 251, Y = 143 }, B = new Point { X = 261, Y = 140 } };
            MapYolo[4] = new Mur { A = new Point { X = 246, Y = 144 }, B = new Point { X = 251, Y = 143 } };
            MapYolo[5] = new Mur { A = new Point { X = 240, Y = 146 }, B = new Point { X = 246, Y = 144 } };
            MapYolo[6] = new Mur { A = new Point { X = 225, Y = 148 }, B = new Point { X = 240, Y = 146 } };
            MapYolo[7] = new Mur { A = new Point { X = 219, Y = 148 }, B = new Point { X = 225, Y = 148 } };
            MapYolo[8] = new Mur { A = new Point { X = 208, Y = 148 }, B = new Point { X = 219, Y = 148 } };
            MapYolo[9] = new Mur { A = new Point { X = 196, Y = 146 }, B = new Point { X = 208, Y = 148 } };
            MapYolo[10] = new Mur { A = new Point { X = 188, Y = 143 }, B = new Point { X = 196, Y = 146 } };
            MapYolo[11] = new Mur { A = new Point { X = 181, Y = 141 }, B = new Point { X = 188, Y = 143 } };
            MapYolo[12] = new Mur { A = new Point { X = 177, Y = 139 }, B = new Point { X = 181, Y = 141 } };
            MapYolo[13] = new Mur { A = new Point { X = 171, Y = 137 }, B = new Point { X = 177, Y = 139 } };
            MapYolo[14] = new Mur { A = new Point { X = 167, Y = 134 }, B = new Point { X = 171, Y = 137 } };
            MapYolo[15] = new Mur { A = new Point { X = 266, Y = 61 }, B = new Point { X = 266, Y = 61 } };
            MapYolo[16] = new Mur { A = new Point { X = 269, Y = 62 }, B = new Point { X = 266, Y = 61 } };
            MapYolo[17] = new Mur { A = new Point { X = 271, Y = 64 }, B = new Point { X = 269, Y = 62 } };
            MapYolo[18] = new Mur { A = new Point { X = 272, Y = 66 }, B = new Point { X = 271, Y = 64 } };
            MapYolo[19] = new Mur { A = new Point { X = 274, Y = 70 }, B = new Point { X = 272, Y = 66 } };
            MapYolo[20] = new Mur { A = new Point { X = 275, Y = 75 }, B = new Point { X = 274, Y = 70 } };
            MapYolo[21] = new Mur { A = new Point { X = 276, Y = 81 }, B = new Point { X = 275, Y = 75 } };
            MapYolo[22] = new Mur { A = new Point { X = 274, Y = 87 }, B = new Point { X = 276, Y = 81 } };
            MapYolo[23] = new Mur { A = new Point { X = 273, Y = 91 }, B = new Point { X = 274, Y = 87 } };
            MapYolo[24] = new Mur { A = new Point { X = 269, Y = 97 }, B = new Point { X = 273, Y = 91 } };
            MapYolo[25] = new Mur { A = new Point { X = 265, Y = 102 }, B = new Point { X = 269, Y = 97 } };
            MapYolo[26] = new Mur { A = new Point { X = 261, Y = 105 }, B = new Point { X = 265, Y = 102 } };
            MapYolo[27] = new Mur { A = new Point { X = 258, Y = 107 }, B = new Point { X = 261, Y = 105 } };
            MapYolo[28] = new Mur { A = new Point { X = 253, Y = 107 }, B = new Point { X = 258, Y = 107 } };
            MapYolo[29] = new Mur { A = new Point { X = 249, Y = 105 }, B = new Point { X = 253, Y = 107 } };
            MapYolo[30] = new Mur { A = new Point { X = 247, Y = 103 }, B = new Point { X = 249, Y = 105 } };
            MapYolo[31] = new Mur { A = new Point { X = 245, Y = 99 }, B = new Point { X = 247, Y = 103 } };
            MapYolo[32] = new Mur { A = new Point { X = 244, Y = 95 }, B = new Point { X = 245, Y = 99 } };
            MapYolo[33] = new Mur { A = new Point { X = 243, Y = 91 }, B = new Point { X = 244, Y = 95 } };
            MapYolo[34] = new Mur { A = new Point { X = 243, Y = 88 }, B = new Point { X = 243, Y = 91 } };
            MapYolo[35] = new Mur { A = new Point { X = 245, Y = 81 }, B = new Point { X = 243, Y = 88 } };
            MapYolo[36] = new Mur { A = new Point { X = 246, Y = 78 }, B = new Point { X = 245, Y = 81 } };
            MapYolo[37] = new Mur { A = new Point { X = 251, Y = 72 }, B = new Point { X = 246, Y = 78 } };
            MapYolo[38] = new Mur { A = new Point { X = 253, Y = 70 }, B = new Point { X = 251, Y = 72 } };
            MapYolo[39] = new Mur { A = new Point { X = 256, Y = 68 }, B = new Point { X = 253, Y = 70 } };
            MapYolo[40] = new Mur { A = new Point { X = 262, Y = 63 }, B = new Point { X = 256, Y = 68 } };
            MapYolo[41] = new Mur { A = new Point { X = 207, Y = 102 }, B = new Point { X = 207, Y = 102 } };
            MapYolo[42] = new Mur { A = new Point { X = 208, Y = 102 }, B = new Point { X = 207, Y = 102 } };
            MapYolo[43] = new Mur { A = new Point { X = 208, Y = 101 }, B = new Point { X = 208, Y = 102 } };
            MapYolo[44] = new Mur { A = new Point { X = 208, Y = 99 }, B = new Point { X = 208, Y = 101 } };
            MapYolo[45] = new Mur { A = new Point { X = 208, Y = 97 }, B = new Point { X = 208, Y = 99 } };
            MapYolo[46] = new Mur { A = new Point { X = 208, Y = 94 }, B = new Point { X = 208, Y = 97 } };
            MapYolo[47] = new Mur { A = new Point { X = 208, Y = 90 }, B = new Point { X = 208, Y = 94 } };
            MapYolo[48] = new Mur { A = new Point { X = 208, Y = 86 }, B = new Point { X = 208, Y = 90 } };
            MapYolo[49] = new Mur { A = new Point { X = 208, Y = 81 }, B = new Point { X = 208, Y = 86 } };
            MapYolo[50] = new Mur { A = new Point { X = 208, Y = 75 }, B = new Point { X = 208, Y = 81 } };
            MapYolo[51] = new Mur { A = new Point { X = 207, Y = 69 }, B = new Point { X = 208, Y = 75 } };
            MapYolo[52] = new Mur { A = new Point { X = 207, Y = 64 }, B = new Point { X = 207, Y = 69 } };
            MapYolo[53] = new Mur { A = new Point { X = 207, Y = 61 }, B = new Point { X = 207, Y = 64 } };
            MapYolo[54] = new Mur { A = new Point { X = 207, Y = 56 }, B = new Point { X = 207, Y = 61 } };
            MapYolo[55] = new Mur { A = new Point { X = 207, Y = 51 }, B = new Point { X = 207, Y = 56 } };
            MapYolo[56] = new Mur { A = new Point { X = 207, Y = 48 }, B = new Point { X = 207, Y = 51 } };
            MapYolo[57] = new Mur { A = new Point { X = 156, Y = 70 }, B = new Point { X = 156, Y = 70 } };
            MapYolo[58] = new Mur { A = new Point { X = 157, Y = 71 }, B = new Point { X = 156, Y = 70 } };
            MapYolo[59] = new Mur { A = new Point { X = 158, Y = 72 }, B = new Point { X = 157, Y = 71 } };
            MapYolo[60] = new Mur { A = new Point { X = 160, Y = 74 }, B = new Point { X = 158, Y = 72 } };
            MapYolo[61] = new Mur { A = new Point { X = 162, Y = 75 }, B = new Point { X = 160, Y = 74 } };
            MapYolo[62] = new Mur { A = new Point { X = 164, Y = 77 }, B = new Point { X = 162, Y = 75 } };
            MapYolo[63] = new Mur { A = new Point { X = 165, Y = 81 }, B = new Point { X = 164, Y = 77 } };
            MapYolo[64] = new Mur { A = new Point { X = 166, Y = 86 }, B = new Point { X = 165, Y = 81 } };
            MapYolo[65] = new Mur { A = new Point { X = 165, Y = 90 }, B = new Point { X = 166, Y = 86 } };
            MapYolo[66] = new Mur { A = new Point { X = 162, Y = 98 }, B = new Point { X = 165, Y = 90 } };
            MapYolo[67] = new Mur { A = new Point { X = 157, Y = 107 }, B = new Point { X = 162, Y = 98 } };
            MapYolo[68] = new Mur { A = new Point { X = 154, Y = 110 }, B = new Point { X = 157, Y = 107 } };
            MapYolo[69] = new Mur { A = new Point { X = 150, Y = 113 }, B = new Point { X = 154, Y = 110 } };
            MapYolo[70] = new Mur { A = new Point { X = 148, Y = 113 }, B = new Point { X = 150, Y = 113 } };
            MapYolo[71] = new Mur { A = new Point { X = 144, Y = 113 }, B = new Point { X = 148, Y = 113 } };
            MapYolo[72] = new Mur { A = new Point { X = 140, Y = 111 }, B = new Point { X = 144, Y = 113 } };
            MapYolo[73] = new Mur { A = new Point { X = 139, Y = 110 }, B = new Point { X = 140, Y = 111 } };
            MapYolo[74] = new Mur { A = new Point { X = 138, Y = 107 }, B = new Point { X = 139, Y = 110 } };
            MapYolo[75] = new Mur { A = new Point { X = 139, Y = 97 }, B = new Point { X = 138, Y = 107 } };
            MapYolo[76] = new Mur { A = new Point { X = 140, Y = 91 }, B = new Point { X = 139, Y = 97 } };
            MapYolo[77] = new Mur { A = new Point { X = 142, Y = 83 }, B = new Point { X = 140, Y = 91 } };
            MapYolo[78] = new Mur { A = new Point { X = 144, Y = 76 }, B = new Point { X = 142, Y = 83 } };
            MapYolo[79] = new Mur { A = new Point { X = 146, Y = 72 }, B = new Point { X = 144, Y = 76 } };

            MapYolo = MapYolo.Translate(Position);
            MapYolo = MapYolo.Rotate(RotationDegre);
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
