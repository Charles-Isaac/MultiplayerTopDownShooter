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
        public static bool IsIntersecting(PointF a, PointF b, PointF c, PointF d)
        {
            float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
            float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }

        public static bool PlayerBulletCollision(PlayerData Player, PointF OldProjectile, PointF NewProjectile)
        {//a + 5, b - 5;
            //c == new, d == old
            float denominatorX = (((Player.Position.X - 5) - (Player.Position.X + 5)) * ((OldProjectile.Y) - (NewProjectile.Y))) - (((Player.Position.Y) - (Player.Position.Y)) * ((OldProjectile.X) - (NewProjectile.X)));
            float numerator1X = (((Player.Position.Y) - (NewProjectile.Y)) * ((OldProjectile.X) - (NewProjectile.X))) - (((Player.Position.X + 5) - (NewProjectile.X)) * ((OldProjectile.Y) - (NewProjectile.Y)));
            float numerator2X = (((Player.Position.Y) - (NewProjectile.Y)) * ((Player.Position.X - 5) - (Player.Position.X + 5))) - (((Player.Position.X + 5) - (NewProjectile.X)) * ((Player.Position.Y) - (Player.Position.Y)));

            

            

            float denominatorY = ((Player.Position.X - Player.Position.X) * ((OldProjectile.Y) - (NewProjectile.Y))) - (((Player.Position.Y - 5) - (Player.Position.Y + 5)) * ((OldProjectile.X) - (NewProjectile.X)));
            float numerator1Y = (((Player.Position.Y + 5) - (NewProjectile.Y)) * ((OldProjectile.X) - (NewProjectile.X))) - (((Player.Position.X) - (NewProjectile.X)) * ((OldProjectile.Y) - (NewProjectile.Y)));
            float numerator2Y = (((Player.Position.Y + 5) - (NewProjectile.Y)) * ((Player.Position.X) - (Player.Position.X))) - (((Player.Position.X) - (NewProjectile.X)) * ((Player.Position.Y - 5) - (Player.Position.Y + 5)));

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
