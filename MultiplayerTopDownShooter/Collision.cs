using System.Drawing;

namespace ClonesEngine
{
    static class Collision
    {
        public static bool IsIntersecting(PointF VecteurA1, PointF VecteurA2, PointF VecteurB1, PointF VecteurB2) //Retourne un booléen indiquant si la ligne tracé entre VecteurA1 et VecteurA2 intersectionne la ligne tracé entre VecteurB1 et VecteurB2 
        {
            float Denominator = ((VecteurA2.X - VecteurA1.X) * (VecteurB2.Y - VecteurB1.Y)) - ((VecteurA2.Y - VecteurA1.Y) * (VecteurB2.X - VecteurB1.X));
            float Numerator1 = ((VecteurA1.Y - VecteurB1.Y) * (VecteurB2.X - VecteurB1.X)) - ((VecteurA1.X - VecteurB1.X) * (VecteurB2.Y - VecteurB1.Y));
            float Numerator2 = ((VecteurA1.Y - VecteurB1.Y) * (VecteurA2.X - VecteurA1.X)) - ((VecteurA1.X - VecteurB1.X) * (VecteurA2.Y - VecteurA1.Y));

            if (Denominator == 0) return Numerator1 == 0 && Numerator2 == 0;

            float R = Numerator1 / Denominator;
            float S = Numerator2 / Denominator;

            return (R >= 0 && R <= 1) && (S >= 0 && S <= 1);
        }

        public static bool PlayerBulletCollision(PlayerData Player, PointF OldProjectile, PointF NewProjectile) //Retourne un booléen indiquant si le projectile est entré en collision
        {
            float DenominatorX = (((Player.Position.X - Player.Size / 2) - (Player.Position.X + Player.Size / 2)) * ((OldProjectile.Y) - (NewProjectile.Y))) - (((Player.Position.Y) - (Player.Position.Y)) * ((OldProjectile.X) - (NewProjectile.X)));
            float Numerator1X = (((Player.Position.Y) - (NewProjectile.Y)) * ((OldProjectile.X) - (NewProjectile.X))) - (((Player.Position.X + Player.Size / 2) - (NewProjectile.X)) * ((OldProjectile.Y) - (NewProjectile.Y)));
            float Numerator2X = (((Player.Position.Y) - (NewProjectile.Y)) * ((Player.Position.X - Player.Size / 2) - (Player.Position.X + Player.Size / 2))) - (((Player.Position.X + Player.Size / 2) - (NewProjectile.X)) * ((Player.Position.Y) - (Player.Position.Y)));


            float DenominatorY = ((Player.Position.X - Player.Position.X) * ((OldProjectile.Y) - (NewProjectile.Y))) - (((Player.Position.Y - Player.Size / 2) - (Player.Position.Y + Player.Size / 2)) * ((OldProjectile.X) - (NewProjectile.X)));
            float Numerator1Y = (((Player.Position.Y + Player.Size / 2) - (NewProjectile.Y)) * ((OldProjectile.X) - (NewProjectile.X))) - (((Player.Position.X) - (NewProjectile.X)) * ((OldProjectile.Y) - (NewProjectile.Y)));
            float Numerator2Y = (((Player.Position.Y + Player.Size / 2) - (NewProjectile.Y)) * ((Player.Position.X) - (Player.Position.X))) - (((Player.Position.X) - (NewProjectile.X)) * ((Player.Position.Y - Player.Size / 2) - (Player.Position.Y + Player.Size / 2)));

            if (DenominatorX == 0 || DenominatorY == 0) return (Numerator1X == 0 && Numerator2X == 0) || (Numerator1Y == 0 && Numerator2Y == 0);

            float Rx = Numerator1X / DenominatorX;
            float Sx = Numerator2X / DenominatorX;

            float Ry = Numerator1Y / DenominatorY;
            float Sy = Numerator2Y / DenominatorY;

            return (Rx >= 0 && Rx <= 1) && (Sx >= 0 && Sx <= 1) || (Ry >= 0 && Ry <= 1) && (Sy >= 0 && Sy <= 1);
            
        }

      
    }
}
