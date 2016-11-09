using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClonesEngine
{
    static class Collision
    {
        public static bool IsIntersecting(PointF vA1, PointF vA2, PointF vB1, PointF vB2)
        {
            float denominator = ((vA2.X - vA1.X) * (vB2.Y - vB1.Y)) - ((vA2.Y - vA1.Y) * (vB2.X - vB1.X));
            float numerator1 = ((vA1.Y - vB1.Y) * (vB2.X - vB1.X)) - ((vA1.X - vB1.X) * (vB2.Y - vB1.Y));
            float numerator2 = ((vA1.Y - vB1.Y) * (vA2.X - vA1.X)) - ((vA1.X - vB1.X) * (vA2.Y - vA1.Y));

            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }

        public static bool PlayerBulletCollision(PlayerData Player, PointF OldProjectile, PointF NewProjectile)
        {//a + 5, b - 5;
            //c == new, d == old
            float denominatorX = (((Player.Position.X - Player.Size / 2) - (Player.Position.X + Player.Size / 2)) * ((OldProjectile.Y) - (NewProjectile.Y))) - (((Player.Position.Y) - (Player.Position.Y)) * ((OldProjectile.X) - (NewProjectile.X)));
            float numerator1X = (((Player.Position.Y) - (NewProjectile.Y)) * ((OldProjectile.X) - (NewProjectile.X))) - (((Player.Position.X + Player.Size / 2) - (NewProjectile.X)) * ((OldProjectile.Y) - (NewProjectile.Y)));
            float numerator2X = (((Player.Position.Y) - (NewProjectile.Y)) * ((Player.Position.X - Player.Size / 2) - (Player.Position.X + Player.Size / 2))) - (((Player.Position.X + Player.Size / 2) - (NewProjectile.X)) * ((Player.Position.Y) - (Player.Position.Y)));

            

            

            float denominatorY = ((Player.Position.X - Player.Position.X) * ((OldProjectile.Y) - (NewProjectile.Y))) - (((Player.Position.Y - Player.Size / 2) - (Player.Position.Y + Player.Size / 2)) * ((OldProjectile.X) - (NewProjectile.X)));
            float numerator1Y = (((Player.Position.Y + Player.Size / 2) - (NewProjectile.Y)) * ((OldProjectile.X) - (NewProjectile.X))) - (((Player.Position.X) - (NewProjectile.X)) * ((OldProjectile.Y) - (NewProjectile.Y)));
            float numerator2Y = (((Player.Position.Y + Player.Size / 2) - (NewProjectile.Y)) * ((Player.Position.X) - (Player.Position.X))) - (((Player.Position.X) - (NewProjectile.X)) * ((Player.Position.Y - Player.Size / 2) - (Player.Position.Y + Player.Size / 2)));

            if (denominatorX == 0 || denominatorY == 0) return (numerator1X == 0 && numerator2X == 0) || (numerator1Y == 0 && numerator2Y == 0);

            float rX = numerator1X / denominatorX;
            float sX = numerator2X / denominatorX;

            float rY = numerator1Y / denominatorY;
            float sY = numerator2Y / denominatorY;

            return (rX >= 0 && rX <= 1) && (sX >= 0 && sX <= 1) || (rY >= 0 && rY <= 1) && (sY >= 0 && sY <= 1);

            
        }

        public static void PlayerBorder(PlayerData Player)
        {
            if (Player.Position.X < 0)
            {
                Player.Position = new PointF(0, Player.Position.Y);
            }
            else
            {
                if (Player.Position.X > System.Windows.Forms.Form.ActiveForm?.Width)
                {
                    Player.Position = new PointF((float)System.Windows.Forms.Form.ActiveForm?.Width, Player.Position.Y);
                }
            }
            if (Player.Position.Y < 0)
            {
                Player.Position = new PointF(Player.Position.X, 0);
            }
            else
            {
                if (Player.Position.Y > System.Windows.Forms.Form.ActiveForm?.Height)
                {
                    Player.Position = new PointF(Player.Position.X, (float)System.Windows.Forms.Form.ActiveForm?.Height);
                }
            }
        }
    }
}
