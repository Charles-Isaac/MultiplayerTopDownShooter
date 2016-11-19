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
        NumberOfWeapons = 4,
        Pistol = 0,
        Shotgun = 1,
        MachineGun = 2,
        Sniper = 3,

    }
    abstract class Weapons
    {
        public abstract void MouseDown(PointF MouseDir);
        public abstract void MouseUp();
        public abstract int NBulletLeft { get; set; }
        public abstract int NBulletInCharger { get; set; }
        public abstract PointF MouseDir { set; }
        public abstract string WeaponName { get; }
    }
    class Pistol : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
        int LastTickShot;
        byte _Wielder;
        PlayerData[] _PlayerList;
        System.Media.SoundPlayer FireSound = new System.Media.SoundPlayer(MultiplayerTopDownShooter.Properties.Resources.Glock17Sound);

        public override string WeaponName { get { return "Wannabe a Glock17"; } } 
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

                FireSound.Play();

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
        System.Media.SoundPlayer FireSound = new System.Media.SoundPlayer(MultiplayerTopDownShooter.Properties.Resources.MachineGunSound);

        public override string WeaponName { get { return "Machine Gun"; } }
        const byte BulletSpeed = 25;
        const int ReloadingTime = 2000;
        const int ClipSize = 30;
        const int Firerate = 105;
        const int SpreadAngle = 6;
        Random RNG = new Random();
        object WeaponLock = new object();
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
            Tim.AutoReset = false;
            Tim.Elapsed += _timer_Elapsed;


        }
        public override void MouseDown(PointF MouseDir)
        {
            if (CanShoot)
            {
                CanShoot = false;
                NBulletInCharger--;
                lock (WeaponLock)
                {
                    Tim.Start();
                }
                _MouseDir = MouseDir;
                
                double radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, MouseDir, BulletSpeed));

                FireSound.Play();

            }
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (WeaponLock)
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
                        
                        if (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
                        {
                            NBulletInCharger--;
                            double radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                            PointF MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                            _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, MouseDir, BulletSpeed));
                            Tim.Start();
                            FireSound.Play();
                        }
                    }
                }
            }


            /*NBulletInCharger--;
                    CanShoot = true;
                    double radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                    PointF MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                    _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, MouseDir, BulletSpeed));
                    if (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
                    {
                        Tim.Start();
                    }
                    */

        }
        public override void MouseUp()
        {
            

        }
    }
    class Sniper : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
        int LastTickShot;
        byte _Wielder;
        PlayerData[] _PlayerList;
        System.Media.SoundPlayer FireSound = new System.Media.SoundPlayer(MultiplayerTopDownShooter.Properties.Resources.SniperSound);

        public override string WeaponName { get { return "Some HUGE Sniper Rifle"; } }
        const byte BulletSpeed = 200;
        const int ReloadingTime = 5000;
        const int ClipSize = 5;
        const int Firerate = 600;
        const int SpreadAngle = 0;
        Random RNG = new Random();
        bool CanShoot = true;
        bool Reloading = false;
        System.Timers.Timer Tim;


        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public Sniper(ref byte ID, PlayerData[] PlayerList)
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

                FireSound.Play();

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
    class Shotgun : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
        int LastTickShot;
        byte _Wielder;
        PlayerData[] _PlayerList;
        Random RNG;
        System.Media.SoundPlayer FireSound = new System.Media.SoundPlayer(MultiplayerTopDownShooter.Properties.Resources.ShotgunSound);

        public override string WeaponName { get { return "Shotgun"; } }
        const byte BulletSpeed = 15;
        const int ReloadingTime = 5000;
        const int SpreadAngle = 30;
        const int NumberOfBuckshot = 10;
        const int ClipSize = 8;
        const int Firerate = 600;
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
                FireSound.Play();
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
