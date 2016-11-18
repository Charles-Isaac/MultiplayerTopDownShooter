using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace ClonesEngine
{
    enum WeaponType : byte
    {
        NumberOfWeapons = 3,
        Pistol = 0,
        Shotgun = 1,
        MachineGun = 2,

    }
    abstract class Weapons
    {
        public abstract void MouseDown(PointF MouseDir);
        public abstract void MouseUp();
        public abstract int NBulletLeft { get; set; }
        public abstract int NBulletInCharger { get; set; }
        public abstract PointF MouseDir { set; }


    }
    abstract class WeaponTemplate
    {
        public abstract void MouseDown();
        public abstract void MouseUp();
    }
    class Pistol : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
        int LastTickShot;
        byte _Wielder;
        PlayerData[] _PlayerList;

        const string WeaponName = "Wannabe a Glock17";
        const byte BulletSpeed = 25;
        const int ReloadingTime = 2000;
        const int ClipSize = 17;
        const int Firerate = 100;
        const int SpreadAngle = 2;
        Random RNG = new Random();
        bool CanShoot = true;
        bool Reloading = false;
        System.Timers.Timer Tim;


        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public Pistol(ref byte ID, PlayerData[] PlayerList)
        {
            NBulletLeft = int.MaxValue;
            NBulletInCharger = ClipSize;
            _Wielder = ID;
            _PlayerList = PlayerList;
            LastTickShot = Environment.TickCount;
            Tim = new System.Timers.Timer(Firerate);
            Tim.AutoReset = false;
            Tim.Elapsed += new ElapsedEventHandler(_timer_Elapsed);


        }
        public override void MouseDown(PointF MouseDir)
        {
            if (CanShoot)
            {
                CanShoot = false;
                Tim.Start();
                _MouseDir = MouseDir;
                 NBulletInCharger--;

                double radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, MouseDir, BulletSpeed));
            

           //   _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, _MouseDir, BulletSpeed));
            }
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            if (Reloading)
            {
                if (NBulletLeft <= 0)
                {
                    Tim.Stop();
                    Tim.Interval = ReloadingTime;
                    Tim.Start();
                }
                else
                {
                    if (NBulletLeft <= ClipSize)
                    {
                        NBulletInCharger = NBulletLeft;
                        NBulletLeft = 0;
                    }
                    else
                    {
                        NBulletLeft -= ClipSize;
                        NBulletInCharger = ClipSize;
                    }
                    Tim.Stop();
                    Tim.Interval = Firerate;
                    Reloading = false;
                    CanShoot = true;
                }
                
            }
            else
            {
               
                if (NBulletInCharger <= 0)
                {
                    CanShoot = false;
                    Reloading = true;
                    Tim.Stop();
                    Tim.Interval = ReloadingTime;
                    Tim.Start();
                    
                }
                else
                {
                    CanShoot = true;
                }
            }

        }
        public override void MouseUp()
        {
            //Do nothing in this case
        }
      



    }
    class MachineGun : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
        int LastTickShot;
        byte _Wielder;
        PlayerData[] _PlayerList;

        const string WeaponName = "Wannabe a Glock17";
        const byte BulletSpeed = 25;
        const int ReloadingTime = 2000;
        const int ClipSize = 30;
        const int Firerate = 100;
        const int SpreadAngle = 4;
        Random RNG = new Random();

        bool CanShoot = true;
        bool Reloading = false;
        System.Timers.Timer Tim;


        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public MachineGun(ref byte ID, PlayerData[] PlayerList)
        {
            NBulletLeft = int.MaxValue;
            NBulletInCharger = ClipSize;
            _Wielder = ID;
            _PlayerList = PlayerList;
            LastTickShot = Environment.TickCount;
            Tim = new System.Timers.Timer(Firerate);
            Tim.AutoReset = true;
            Tim.Elapsed += new ElapsedEventHandler(_timer_Elapsed);


        }
        public override void MouseDown(PointF MouseDir)
        {
            if (CanShoot)
            {
                CanShoot = false;
                NBulletInCharger--;

                Tim.Start();
                _MouseDir = MouseDir;
                
                double radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, MouseDir, BulletSpeed));
            }
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (Reloading)
            {

                if (NBulletLeft <= 0)
                {
                    Tim.Stop();
                    Tim.AutoReset = false;
                    Tim.Interval = ReloadingTime;
                    Tim.Start();
                }
                else
                {
                    if (NBulletLeft <= ClipSize)
                    {
                        NBulletInCharger = NBulletLeft;
                        NBulletLeft = 0;
                    }
                    else
                    {
                        NBulletLeft -= ClipSize;
                        NBulletInCharger = ClipSize;
                    }
                    Tim.Stop();
                    Tim.AutoReset = true;
                    Tim.Interval = Firerate;
                    Reloading = false;
                    CanShoot = true;
                }

            }
            else
            {

                if (NBulletInCharger <= 0)
                {
                    CanShoot = false;
                    Reloading = true;
                    Tim.Stop();
                    Tim.AutoReset = false;
                    Tim.Interval = ReloadingTime;
                    Tim.Start();

                }
                else
                {
                    NBulletInCharger--;
                    CanShoot = true;
                    double radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                    PointF MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                    _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, MouseDir, BulletSpeed));
                }
            }

        }
        public override void MouseUp()
        {
            Tim.Stop();
        }
    }
    class Sniper
    {

    }
    class Shotgun : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
        int LastTickShot;
        byte _Wielder;
        PlayerData[] _PlayerList;
        Random RNG;

        const string WeaponName = "Wannabe a Glock17";
        const byte BulletSpeed = 15;
        const int ReloadingTime = 4000;
        const int SpreadAngle = 30;
        const int NumberOfBuckshot = 10;
        const int ClipSize = 8;
        const int Firerate = 500;
        bool CanShoot = true;
        bool Reloading = false;
        System.Timers.Timer Tim;


        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public Shotgun(ref byte ID, PlayerData[] PlayerList)
        {
            RNG = new Random();
            NBulletLeft = 24;
            NBulletInCharger = ClipSize;
            _Wielder = ID;
            _PlayerList = PlayerList;
            LastTickShot = Environment.TickCount;
            Tim = new System.Timers.Timer(Firerate);
            Tim.AutoReset = false;
            Tim.Elapsed += new ElapsedEventHandler(_timer_Elapsed);


        }
        public override void MouseDown(PointF MouseDir)
        {
            if (CanShoot)
            {
                CanShoot = false;
                Tim.Start();
                _MouseDir = MouseDir;
                NBulletInCharger--;
                double radians;

                for (int i = 0; i < NumberOfBuckshot; i++)
                {
                    radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                    MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                    _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, MouseDir, BulletSpeed));
                }
                
            }
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (Reloading)
            {
                if (NBulletLeft <= 0)
                {
                    Tim.Stop();
                    Tim.Interval = ReloadingTime;
                    Tim.Start();
                }
                else
                {
                    if (NBulletLeft <= ClipSize)
                    {
                        NBulletInCharger = NBulletLeft;
                        NBulletLeft = 0;
                    }
                    else
                    {
                        NBulletLeft -= ClipSize;
                        NBulletInCharger = ClipSize;
                    }
                    Tim.Stop();
                    Tim.Interval = Firerate;
                    Reloading = false;
                    CanShoot = true;
                }

            }
            else
            {

                if (NBulletInCharger <= 0)
                {
                    CanShoot = false;
                    Reloading = true;
                    Tim.Stop();
                    Tim.Interval = ReloadingTime;
                    Tim.Start();

                }
                else
                {
                    CanShoot = true;
                }
            }

        }
        public override void MouseUp()
        {
            //Do nothing in this case
        }

    }
    class RocketLauncher
    {

    }

}
