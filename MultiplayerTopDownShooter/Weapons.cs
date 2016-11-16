using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ClonesEngine
{
    class Weapons
    {
        class Pistol
        {
            int NBullet;
            int LastTickShot;
            const float Precision = 0.9F;
            const byte BulletSpeed = 50;
            Timer Tim;
            

            public Pistol()
            {
                NBullet = -1;
                LastTickShot = Environment.TickCount;
                Tim = new Timer();

            }
            


            public List<Projectile> Shoot(PointF PlayerPosition, PointF Direction)
            {
                List<Projectile> BulletList = new List<Projectile>(1);
                BulletList.Add(new Projectile(PlayerPosition, Direction, BulletSpeed, Environment.TickCount));
                return BulletList;
            }
        }
        class MachineGun : WeaponTemplate
        {

            Timer Tim;
            public MachineGun()
            {
                Tim = new Timer();
                Tim.Interval = 250;
            }




            public override void MouseDown()
            {
                Tim.Start();
            }
            public override void MouseUp()
            {
                Tim.Stop();
            }
        }
        class Sniper
        {

        }
        class Shotgun
        {

        }
        class RocketLauncher
        {

        }

    }
    abstract class WeaponTemplate
    {
        public abstract void MouseDown();
        public abstract void MouseUp();
    }
}
