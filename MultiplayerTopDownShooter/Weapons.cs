using System;
using System.Drawing;
using System.Timers;

namespace ClonesEngine
{
    enum WeaponType : byte
    {
        NumberOfWeapons = 5,
        Pistol = 0,
        Shotgun = 1,
        MachineGun = 2,
        Sniper = 3,
        RocketLauncher = 4,

    }
    enum ProjectileType : byte
    {
        Bullet = 0,
        Rocket = 1,
        Grenade = 2,

    }
    abstract class Weapons
    {
        public abstract void MouseDown(PointF MouseDir);
        public abstract void MouseUp();
        public abstract void Reload();

        public abstract int NBulletLeft { get; set; }
        public abstract int NBulletInCharger { get; set; }
        public abstract PointF MouseDir { set; }
        public abstract string WeaponName { get; }
        public abstract PlayerData User { set; }
    }

    class Pistol : Weapons
    {

        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }

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
        PlayerData _Player;






        public override PlayerData User
        {
            set
            {
                _Player = value;
            }
        }

        public override void Reload()
        {
            if (!Reloading)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                Reloading = true;
                CanShoot = false;
                Tim.Interval = ReloadingTime;
                Tim.Start();
            }
        }
        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public Pistol(PlayerData Player)
        {
            _Player = Player;
            NBulletLeft = int.MaxValue - 100;
            NBulletInCharger = ClipSize;
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
                _Player.AjouterProjectile(new Projectile(_Player.Position, MouseDir, BulletSpeed, (byte)ProjectileType.Bullet));

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

        PlayerData _Player;


        public override PlayerData User
        {
            set
            {
                _Player = value;
            }
        }





        public override void Reload()
        {
            if (!Reloading)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                Reloading = true;
                CanShoot = false;
                Tim.Interval = ReloadingTime;
                Tim.Start();
            }
        }

        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public MachineGun(PlayerData Player)
        {
            _Player = Player;
            NBulletLeft = int.MaxValue - 100;
            NBulletInCharger = ClipSize;
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
                _Player.AjouterProjectile(new Projectile(_Player.Position, MouseDir, BulletSpeed, (byte)ProjectileType.Bullet));

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
                            _Player.AjouterProjectile(new Projectile(_Player.Position, MouseDir, BulletSpeed, (byte)ProjectileType.Bullet));
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


        PlayerData _Player;



        public override PlayerData User
        {
            set
            {
                _Player = value;
            }
        }




        public override void Reload()
        {
            if (!Reloading)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                Reloading = true;
                CanShoot = false;
                Tim.Interval = ReloadingTime;
                Tim.Start();
            }
        }

        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public Sniper(PlayerData Player)
        {
            _Player = Player;

            NBulletLeft = int.MaxValue - 100;
            NBulletInCharger = ClipSize;
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
                _Player.AjouterProjectile(new Projectile(_Player.Position, MouseDir, BulletSpeed, (byte)ProjectileType.Bullet));

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


        PlayerData _Player;


        public override PlayerData User
        {
            set
            {
                _Player = value;
            }
        }

        public Shotgun(PlayerData Player)
        {
            _Player = Player;


            RNG = new Random();
            NBulletLeft = 24;
            NBulletInCharger = ClipSize;
            Tim = new System.Timers.Timer(Firerate);
            Tim.AutoReset = false;
            Tim.Elapsed += new ElapsedEventHandler(_timer_Elapsed);

        }

        public override void Reload()
        {
            if (!Reloading)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                Reloading = true;
                CanShoot = false;
                Tim.Interval = ReloadingTime;
                Tim.Start();
            }
        }

        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

    /*    public Shotgun(ref byte ID, PlayerData[] PlayerList)
        {
           

        }*/
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
                    _Player.AjouterProjectile(new Projectile(_Player.Position, MouseDir, BulletSpeed, (byte)ProjectileType.Bullet));
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
    class RocketLauncher : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
        
        System.Media.SoundPlayer FireSound = new System.Media.SoundPlayer(MultiplayerTopDownShooter.Properties.Resources.RocketLauncherSound);
        System.Media.SoundPlayer ExplosionSound = new System.Media.SoundPlayer(MultiplayerTopDownShooter.Properties.Resources.Explosion);

        public override string WeaponName { get { return "Some huge ass rocket launcher"; } }
        const byte BulletSpeed = 10;
        const int ReloadingTime = 5000;
        const int ClipSize = 1;
        const int Firerate = 500;
        const int SpreadAngle = 5;
        Random RNG = new Random();
        bool CanShoot = true;
        bool Reloading = false;
        System.Timers.Timer Tim;
        bool IsAiming;
        bool HasExploded;

        PlayerData _Player;

        public override PlayerData User
        {
            set
            {
                _Player = value;
            }
        }



        public override void Reload()
        {
            //this one is somewhat useless
           /* if (!Reloading)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                Reloading = true;
                CanShoot = false;
                Tim.Interval = ReloadingTime;
                Tim.Start();
            }*/

        }
        PointF _MouseDir = new PointF();
        public override PointF MouseDir
        {
            set { _MouseDir = value; }
        }

        public RocketLauncher(PlayerData Player)
        {
            _Player = Player;
            NBulletLeft = int.MaxValue - 100;
            NBulletInCharger = ClipSize;
            Tim = new System.Timers.Timer(Firerate);
            Tim.AutoReset = false;
            Tim.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            IsAiming = false;
            HasExploded = true;

        }

        public void BringTheHavoc()//make it go boom
        {
            HasExploded = true;
            ExplosionSound.Play();
            double radians;
            lock (_Player.BulletLock)
            {
                for (int i = _Player.Bullet.Count - 1; i >= 0; i--)
                {
                    if (_Player.Bullet[i].TypeOfProjectile == (byte)ProjectileType.Rocket)
                    {
                        for (int j = 0; j < 30; j++)
                        {
                            radians = Math.Atan2(_Player.Bullet[i].Direction.Y, _Player.Bullet[i].Direction.X) + ((RNG.NextDouble() * 340) - 340 / 2.0) * (Math.PI / 180.0);
                            MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                            _Player.AjouterProjectile(new Projectile(_Player.Bullet[i].Position, _MouseDir, BulletSpeed, (byte)ProjectileType.Bullet));
                        }
                        _Player.Bullet.RemoveAt(i);
                    }
                }
            }
        }




        public override void MouseDown(PointF MouseDir)
        {
            
            if (CanShoot)
            {
                    CanShoot = false;
                    IsAiming = true;
                    Tim.Start();
                    _MouseDir = MouseDir;
                    //   _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, _MouseDir, BulletSpeed));
             }

            if (!HasExploded)
            {
                BringTheHavoc();
            }
        }





        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Reloading)
            {
                if (NBulletLeft > 0)
                {
                    NBulletInCharger = ClipSize;
                    NBulletLeft -= ClipSize;

                    Reloading = false;
                    CanShoot = true;
                }
            }
            else
            {
                if (IsAiming)
                {
                    NBulletInCharger--;
                    FireSound.Play();
                    double radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                    MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                    _Player.AjouterProjectile(new Projectile(_Player.Position, _MouseDir, BulletSpeed, (byte)ProjectileType.Rocket));
                    Reloading = true;
                    HasExploded = false;
              //      CanShoot = false;
                    Tim.Stop();
                    Tim.Interval = ReloadingTime;
                    Tim.Start();
                }
            }
        



            /*
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
                    if (IsAiming)
                    {
                        NBulletInCharger--;
                        FireSound.Play();
                        double radians = Math.Atan2(_MouseDir.Y, _MouseDir.X) + ((RNG.NextDouble() * SpreadAngle) - SpreadAngle / 2.0) * (Math.PI / 180.0);
                        MouseDir = new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
                        _PlayerList[_Wielder].AjouterProjectile(new Projectile(_PlayerList[_Wielder].Position, _MouseDir, BulletSpeed, (byte)ProjectileType.Bullet));

                        CanShoot = true;
                    }
                }
            }*/

        }
        public override void MouseUp()
        {
            IsAiming = false;
            if (NBulletInCharger > 0)
            {
                CanShoot = true;
            }
        }




    }

}
