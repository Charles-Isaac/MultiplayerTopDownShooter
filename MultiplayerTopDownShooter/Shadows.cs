using System.Drawing;
using ClonesEngine;

namespace MultiplayerTopDownShooter
{
    static class Shadows
    {
        public static Point[,] ReturnMeAnArray(int NombreDeMurs, Mur[] WallX, PointF LightSource, int Width, int Height)
        {
            Point[,] tempPt = new Point[NombreDeMurs, 6];
            int[,,] PolyGone = new int[NombreDeMurs/* * 2 + 4*/, 4, 2];  //int[NO of the polygon, NO of the point of the angles of the polygon, X or Y]
            for (int i = 0; i < NombreDeMurs; i++)
            {
                double TDVAngle1X = WallX[i].A.X - LightSource.X;
                double TDVAngle1Y = WallX[i].A.Y - LightSource.Y;
                double TDVAngle0X = WallX[i].B.X - LightSource.X;
                double TDVAngle0Y = WallX[i].B.Y - LightSource.Y;

                PolyGone[i, 0, 0] = WallX[i].A.X;
                PolyGone[i, 0, 1] = WallX[i].A.Y;
                PolyGone[i, 1, 0] = WallX[i].B.X;
                PolyGone[i, 1, 1] = WallX[i].B.Y;

                if (TDVAngle0X > 0)
                {

                    PolyGone[i, 2, 0] = Width;
                    PolyGone[i, 2, 1] = (int)(LightSource.Y + TDVAngle0Y * (Width - LightSource.X) / TDVAngle0X);
                }
                else
                {
                    if (TDVAngle0X < 0)
                    {
                        PolyGone[i, 2, 0] = 0;
                        PolyGone[i, 2, 1] = (int)(LightSource.Y + TDVAngle0Y * (0 - LightSource.X) / TDVAngle0X);
                    }
                    else
                    {
                        PolyGone[i, 2, 0] = (int)LightSource.X;
                        if (TDVAngle0Y > 0)
                        {
                            PolyGone[i, 2, 1] = Height;
                        }
                        else
                        {
                            if (TDVAngle0Y < 0)
                            {
                                PolyGone[i, 2, 1] = 0;
                            }
                            else
                            {
                                PolyGone[i, 2, 1] = (int)(LightSource.Y);
                            }
                        }
                    }
                }






                if (TDVAngle1X > 0)
                {
                    PolyGone[i, 3, 0] = Width;
                    PolyGone[i, 3, 1] = (int)(LightSource.Y + TDVAngle1Y * (Width - LightSource.X) / TDVAngle1X);
                }
                else
                {
                    if (TDVAngle1X < 0)
                    {
                        PolyGone[i, 3, 0] = 0;
                        PolyGone[i, 3, 1] = (int)(LightSource.Y + TDVAngle1Y * (0 - LightSource.X) / TDVAngle1X);
                    }
                    else
                    {
                        PolyGone[i, 3, 0] = (int)LightSource.X;
                        if (TDVAngle1Y > 0)
                        {
                            PolyGone[i, 3, 1] = Height;
                        }
                        else
                        {
                            if (TDVAngle1Y < 0)
                            {
                                PolyGone[i, 3, 1] = 0;
                            }
                            else
                            {
                                PolyGone[i, 3, 1] = (int)(LightSource.Y);
                            }
                        }
                    }
                }

            }


            for (int i = 0; i < NombreDeMurs; i++)
            {
                tempPt[i, 0].X = PolyGone[i, 0, 0];
                tempPt[i, 0].Y = PolyGone[i, 0, 1];
                tempPt[i, 1].X = PolyGone[i, 1, 0];
                tempPt[i, 1].Y = PolyGone[i, 1, 1];
                tempPt[i, 2].X = PolyGone[i, 2, 0];
                tempPt[i, 2].Y = PolyGone[i, 2, 1];

                tempPt[i, 3].X = PolyGone[i, 2, 0];
                tempPt[i, 3].Y = PolyGone[i, 2, 1];
                tempPt[i, 4].X = PolyGone[i, 3, 0];
                tempPt[i, 4].Y = PolyGone[i, 3, 1];

                tempPt[i, 5].X = PolyGone[i, 3, 0];
                tempPt[i, 5].Y = PolyGone[i, 3, 1];

                if (tempPt[i, 2].X != tempPt[i, 5].X)
                {
                    if (((tempPt[i, 1].X - tempPt[i, 0].X) * (LightSource.Y - tempPt[i, 0].Y) - (tempPt[i, 1].Y - tempPt[i, 0].Y) * (LightSource.X - tempPt[i, 0].X)) < 0)
                    {
                        if (tempPt[i, 2].Y > 0)
                        {
                            tempPt[i, 3].Y = 0;
                        }
                        if (tempPt[i, 5].Y > 0)
                        {
                            tempPt[i, 4].Y = 0;
                        }
                    }
                    else
                    {
                        if (((tempPt[i, 1].X - tempPt[i, 0].X) * (LightSource.Y - tempPt[i, 0].Y) - (tempPt[i, 1].Y - tempPt[i, 0].Y) * (LightSource.X - tempPt[i, 0].X)) > 0)
                        {
                            if (tempPt[i, 2].Y < Height)
                            {
                                tempPt[i, 3].Y = Height;
                            }
                            if (tempPt[i, 5].Y < Height)
                            {
                                tempPt[i, 4].Y = Height;
                            }
                        }
                    }
                }
            }
            return tempPt;
        }
    }
}
