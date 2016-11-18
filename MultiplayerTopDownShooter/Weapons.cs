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
        NumberOfWeapons = 2,
        Pistol = 0,
        Shotgun = 1,

    }
    abstract class Weapons
    {
        public abstract void MouseDown(PointF MouseDir);
        public abstract void MouseUp();
        public abstract int NBulletLeft { get; set; }
        public abstract int NBulletInCharger { get; set; }


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
        const float Precision = 0.9F;
        const byte BulletSpeed = 25;
        const int ReloadingTime = 2000;
        const int ClipSize = 17;
        bool CanShoot = true;
        bool Reloading = false;
        System.Timers.Timer Tim;


        PointF _MouseDir = new PointF();
        public PointF MouseDir
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
            Tim = new System.Timers.Timer(100);
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
                _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, _MouseDir, BulletSpeed));
            }
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            if (Reloading)
            {
                if (NBulletLeft <= 0)
                {
                    Tim.Stop();
                    Tim.Interval = 2000;
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
                    Tim.Interval = 100;
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
                    Tim.Interval = 2000;
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
    /*class MachineGun : Weapons
    {
        /*
        int NBulletLeft;
        int NBulletInCharger;
        int LastTickShot;
        byte _Wielder;
        PlayerData[] _PlayerList;

        const string WeaponName = "Wannabe a Glock17";
        const float Precision = 0.9F;
        const byte BulletSpeed = 15;
        const int ReloadingTime = 2000;
        const int ClipSize = 17;
        bool CanShoot = true;
        bool Reloading = false;
        System.Timers.Timer Tim;


        PointF _MouseDir = new PointF();
        public PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public MachineGun(ref byte ID, PlayerData[] PlayerList)
        {
            NBulletLeft = int.MaxValue;
            NBulletInCharger = 17;
            _Wielder = ID;
            _PlayerList = PlayerList;
            LastTickShot = Environment.TickCount;
            Tim = new System.Timers.Timer(100);
            Tim.AutoReset = false;
            Tim.Elapsed += new ElapsedEventHandler(_timer_Elapsed);


        }
        public override void MouseDown(PointF MouseDir)
        {
            if (CanShoot)
            {
                Tim.Start();
                _MouseDir = MouseDir;

                _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, _MouseDir, BulletSpeed));
            }
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, _MouseDir, BulletSpeed));
            System.Windows.Forms.MessageBox.Show("Test");

        }
        public override void MouseUp()
        {
            Tim.Stop();
        }
      
    }*/
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
        const float Precision = 0.9F;
        const byte BulletSpeed = 15;
        const int ReloadingTime = 4000;
        const int SpreadAngle = 30;
        const int NumberOfBuckshot = 10;
        const int ClipSize = 8;
        bool CanShoot = true;
        bool Reloading = false;
        System.Timers.Timer Tim;


        PointF _MouseDir = new PointF();
        public PointF MouseDir
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
            Tim = new System.Timers.Timer(100);
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
                    radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + RNG.Next(-SpreadAngle / 2, SpreadAngle / 2) * (Math.PI / 180.0);
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
                    Tim.Interval = 2000;
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
                    Tim.Interval = 100;
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
                    Tim.Interval = 2000;
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
