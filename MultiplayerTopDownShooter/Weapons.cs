using System;
using System.Drawing;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace ClonesEngine
{
    enum WeaponType : byte //Enum contenant les IDs des armes dispo
    {
        NumberOfWeapons = 5,
        Pistol = 0,
        Shotgun = 1,
        MachineGun = 2,
        Sniper = 3,
        RocketLauncher = 4,

    }
    enum WeaponSound : byte //Enum contenant les IDs des sons
    {
        Pistol = 0,
        Shotgun = 1,
        MachineGun = 2,
        Sniper = 3,
        RocketLauncher = 4,
        Explosion = 5,

    }
    enum ProjectileType : byte //Enum contenant les IDs des types de projectiles dispo
    {
        Bullet = 0,
        Rocket = 1,
        Grenade = 2,

    }
   /* public class PlaySoundEventArgs : EventArgs //classe des arguments pour l'event faisant jouer du son
    {
        private byte m_SoundID;

        public byte SoundID
        {
            get { return m_SoundID; }
            set { m_SoundID = value; }
        }
    }*/
    abstract class Weapons //Classe abstraite utilisé comme template pour toutes les armes
    {
        public abstract void MouseDown(PointF MouseDir); //methode utilisé quand un bouton de la sourie est appuyé
        public abstract void MouseUp();//methode utilisé quand un bouton de la sourie est relaché (utilisé pour les armes automatiques)
        public abstract void Reload();//methode utilisé quand le joueur recharge une arme
      //  public abstract void Reloaded();
        public abstract int NBulletLeft { get; set; } //propriété utilisé pour obtenir ou modifier pour le nombre de balles restantes en inventaire
        public abstract int NBulletInCharger { get; set; }//propriété utilisé pour obtenir ou modifier pour le nombre de balles restantes dans le chargeur
        public abstract PointF MouseDirection { set; } //propriété utilisé pour mettre a jours la direction pinté par la sourie
        public abstract string WeaponName { get; } //le nom de l'arme... duuuuh
        public abstract PlayerData User { set; } //Donnés de l'utilisateur de l'arme
        public abstract void PlayShootingSound(); //methode pour jouer le son de l'arme


       // public delegate void OnShotEventHandler(Weapons w, byte e);
        public EventHandler<byte> Shot;
       // public event OnShotEventHandler Shot;

        protected virtual void OnShot(Weapons w, byte e)
        {
            Shot?.Invoke(w, e);
        }
        
    }

    sealed class Pistol : Weapons
    {

        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }

        public override string WeaponName { get { return "Pistolet"; } } 
        private const byte m_BulletSpeed = 25;
        private const int m_ReloadingTime = 2000;
        private const int m_ClipSize = 17;
        private const int m_Firerate = 70;
        private const int m_SpreadAngle = 2;
        private readonly Random m_RNG = new Random();
        private bool m_CanShoot = true;
        private bool m_Reloading;
        private readonly System.Timers.Timer m_WeaponTimer;
        private PlayerData m_Player;



        private volatile bool m_ShootingSound;
        public override void PlayShootingSound()
        {
            m_ShootingSound = false;
            new Task(Sound).Start();
        }
        private void Sound()
        {
            
            using (WaveStream BlockAlignedStream = new WaveFileReader(MultiplayerTopDownShooter.Properties.Resources.PistolSound))
            {
                using (WaveOut WaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    WaveOut.Init(BlockAlignedStream);
                    WaveOut.Play();
                    m_ShootingSound = true;
                    while (WaveOut.PlaybackState == PlaybackState.Playing && m_ShootingSound)
                        Thread.Sleep(15);
                }
            }
        }



        public override PlayerData User
        {
            set
            {
                m_Player = value;
            }
        }


        public override void Reload()
        {
            if (!m_Reloading && NBulletInCharger < m_ClipSize)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                m_Reloading = true;
                m_CanShoot = false;
                m_WeaponTimer.Interval = m_ReloadingTime;
                m_WeaponTimer.Start();
            }
        }
        private PointF m_MouseDir;
        public override PointF MouseDirection
        {
            set { m_MouseDir = value; }
        }

        public Pistol(PlayerData Player)
        {
            m_Player = Player;
            NBulletLeft = int.MaxValue - 100;
            NBulletInCharger = m_ClipSize;
            m_WeaponTimer = new System.Timers.Timer(m_Firerate) {AutoReset = false};
            m_WeaponTimer.Elapsed += _timer_Elapsed;
           

        }
        public override void MouseDown(PointF MouseDir)
        {
            if (m_CanShoot)
            {
                m_CanShoot = false;
                m_WeaponTimer.Start();
                m_MouseDir = MouseDir;
                 NBulletInCharger--;

                double Radians = Math.Atan2(m_MouseDir.Y, m_MouseDir.X) + ((m_RNG.NextDouble() * m_SpreadAngle) - m_SpreadAngle / 2.0) * (Math.PI / 180.0);
                MouseDir = new PointF((float)Math.Cos(Radians), (float)Math.Sin(Radians));
                m_Player.AddProjectile(new Projectile(m_Player.Position, MouseDir, m_BulletSpeed, (byte)ProjectileType.Bullet));
                OnShot(this, (byte)WeaponSound.Pistol);
                PlayShootingSound();

             }
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {


            if (m_Reloading)
            {
                if (NBulletLeft <= 0)
                {
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_ReloadingTime;
                    m_WeaponTimer.Start();
                }
                else
                {
                    if (NBulletLeft <= m_ClipSize)
                    {
                        NBulletInCharger = NBulletLeft;
                        NBulletLeft = 0;
                    }
                    else
                    {
                        NBulletLeft -= m_ClipSize;
                        NBulletInCharger = m_ClipSize;
                    }
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_Firerate;
                    m_Reloading = false;
                    m_CanShoot = true;
                }
            }
            else
            {

                if (NBulletInCharger <= 0)
                {
                    m_CanShoot = false;
                    m_Reloading = true;
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_ReloadingTime;
                    m_WeaponTimer.Start();

                }
                else
                {
                    m_CanShoot = true;
                }
            }

        }
        public override void MouseUp()
        {
            //Do nothing in this case (somewhat useless)
        }
      



    }
    sealed class MachineGun : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }

        public override string WeaponName { get { return "Mitrailleuse antique"; } }
        private const byte m_BulletSpeed = 25;
        private const int m_ReloadingTime = 2000;
        private const int m_ClipSize = 30;//30;
        private const int m_Firerate = 150;//105;
        private const int m_SpreadAngle = 6;
        private readonly Random m_RNG = new Random();
        private readonly object m_WeaponLock = new object();
        private bool m_CanShoot = true;
        private bool m_Reloading;
        private readonly System.Timers.Timer m_WeaponTimer;

        private PlayerData m_Player;

        public override PlayerData User
        {
            set
            {
                m_Player = value;
            }
        }


        private volatile bool m_ShootingSound;
        public override void PlayShootingSound()
        {
            m_ShootingSound = false;
            new Task(Sound).Start();
        }

        private void Sound()
        {
           
                using (WaveOut WaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    WaveOut.Init(new WaveFileReader(MultiplayerTopDownShooter.Properties.Resources.MachineGunSound));
                    WaveOut.Play();
                    m_ShootingSound = true;
                    while (WaveOut.PlaybackState == PlaybackState.Playing && m_ShootingSound)
                        Thread.Sleep(15);
                }
            

        }

        

        public override void Reload()
        {
            if (!m_Reloading && NBulletInCharger < m_ClipSize)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                m_Reloading = true;
                m_CanShoot = false;
                m_WeaponTimer.Interval = m_ReloadingTime;
                m_WeaponTimer.Start();
            }
        }

        private PointF m_MouseDir;
        public override PointF MouseDirection
        {
            set { m_MouseDir = value; }
        }

        public MachineGun(PlayerData Player)
        {
            m_Player = Player;
            NBulletLeft = 50;//int.MaxValue - 100;
            NBulletInCharger = m_ClipSize;
            m_WeaponTimer = new System.Timers.Timer(m_Firerate) {AutoReset = false};
            m_WeaponTimer.Elapsed += _timer_Elapsed;
           
        
        }
        public override void MouseDown(PointF MouseDir)
        {
            if (m_CanShoot)
            {
                m_CanShoot = false;
                NBulletInCharger--;
                lock (m_WeaponLock)
                {
                    m_WeaponTimer.Start();
                }
                m_MouseDir = MouseDir;
                
                double Radians = Math.Atan2(m_MouseDir.Y, m_MouseDir.X) + ((m_RNG.NextDouble() * m_SpreadAngle) - m_SpreadAngle / 2.0) * (Math.PI / 180.0);
                MouseDir = new PointF((float)Math.Cos(Radians), (float)Math.Sin(Radians));
                m_Player.AddProjectile(new Projectile(m_Player.Position, MouseDir, m_BulletSpeed, (byte)ProjectileType.Bullet));


                OnShot(this, (byte)WeaponSound.MachineGun);


                PlayShootingSound();

              
            }
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (m_WeaponLock)
            {



                if (m_Reloading)
                {
                    if (NBulletLeft <= 0)
                    {
                        m_WeaponTimer.Stop();
                        m_WeaponTimer.Interval = m_ReloadingTime;
                        m_WeaponTimer.Start();
                    }
                    else
                    {
                        if (NBulletLeft <= m_ClipSize)
                        {
                            NBulletInCharger = NBulletLeft;
                            NBulletLeft = 0;
                        }
                        else
                        {
                            NBulletLeft -= m_ClipSize;
                            NBulletInCharger = m_ClipSize;
                        }
                        m_WeaponTimer.Stop();
                        m_WeaponTimer.Interval = m_Firerate;
                        m_Reloading = false;
                        m_CanShoot = true;
                    }

                }
                else
                {

                    if (NBulletInCharger <= 0)
                    {
                        m_CanShoot = false;
                        m_Reloading = true;
                        m_WeaponTimer.Stop();
                        m_WeaponTimer.Interval = m_ReloadingTime;
                        m_WeaponTimer.Start();

                    }
                    else
                    {
                        
                        m_CanShoot = true;

                        if (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
                        {
                            NBulletInCharger--;
                            double Radians = Math.Atan2(m_MouseDir.Y, m_MouseDir.X) +
                                             ((m_RNG.NextDouble()*m_SpreadAngle) - m_SpreadAngle/2.0)*(Math.PI/180.0);

                            m_Player.AddProjectile(new Projectile(m_Player.Position,
                                new PointF((float) Math.Cos(Radians), (float) Math.Sin(Radians)), m_BulletSpeed,
                                (byte) ProjectileType.Bullet));
                            m_WeaponTimer.Start();
                            OnShot(this, (byte) WeaponSound.MachineGun); 
                            PlayShootingSound(); //bug de la cadence de tir

                        }
                    }
                }
            }


        

    }
    public override void MouseUp()
        {
            

        }
    }
    sealed class Sniper : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
        
        public override string WeaponName { get { return "Fusil de précision"; } }
        private const byte m_BulletSpeed = 200;
        private const int m_ReloadingTime = 5000;
        private const int m_ClipSize = 5;
        private const int m_Firerate = 600;
        private const int m_SpreadAngle = 0;
        private readonly Random m_RNG = new Random();
        private bool m_CanShoot = true;
        private bool m_Reloading;
        private readonly System.Timers.Timer m_WeaponTimer;


        private PlayerData m_Player;

        private volatile bool m_ShootingSound;
        public override void PlayShootingSound()
        {
            m_ShootingSound = false;
            new Task(Sound).Start();
        }
        private void Sound()
        {

            using (WaveStream BlockAlignedStream = new WaveFileReader(MultiplayerTopDownShooter.Properties.Resources.SniperSound))
            {
                using (WaveOut WaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    WaveOut.Init(BlockAlignedStream);
                    WaveOut.Play();
                    m_ShootingSound = true;
                    while (WaveOut.PlaybackState == PlaybackState.Playing && m_ShootingSound)
                        Thread.Sleep(15);
                }
            }
        }


        public override PlayerData User
        {
            set
            {
                m_Player = value;
            }
        }




        public override void Reload()
        {
            if (!m_Reloading && NBulletInCharger < m_ClipSize)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                m_Reloading = true;
                m_CanShoot = false;
                m_WeaponTimer.Interval = m_ReloadingTime;
                m_WeaponTimer.Start();
            }
        }

        private PointF m_MouseDir;
        public override PointF MouseDirection
        {
            set { m_MouseDir = value; }
        }

        public Sniper(PlayerData Player)
        {
            m_Player = Player;

            NBulletLeft = 15;//int.MaxValue - 100;
            NBulletInCharger = m_ClipSize;
            m_WeaponTimer = new System.Timers.Timer(m_Firerate) {AutoReset = false};
            m_WeaponTimer.Elapsed += _timer_Elapsed;


        }

        public override void MouseDown(PointF MouseDir)
        {
            if (m_CanShoot)
            {
                m_CanShoot = false;
                m_WeaponTimer.Start();
                m_MouseDir = MouseDir;
                NBulletInCharger--;

                double Radians = Math.Atan2(m_MouseDir.Y, m_MouseDir.X) +
                                 ((m_RNG.NextDouble()*m_SpreadAngle) - m_SpreadAngle/2.0)*(Math.PI/180.0);
                MouseDir = new PointF((float) Math.Cos(Radians), (float) Math.Sin(Radians));
                m_Player.AddProjectile(new Projectile(m_Player.Position, MouseDir, m_BulletSpeed,
                    (byte) ProjectileType.Bullet));
                OnShot(this, (byte) WeaponSound.Sniper);
                PlayShootingSound();

            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (m_Reloading)
            {
                if (NBulletLeft <= 0)
                {
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_ReloadingTime;
                    m_WeaponTimer.Start();
                }
                else
                {
                    if (NBulletLeft <= m_ClipSize)
                    {
                        NBulletInCharger = NBulletLeft;
                        NBulletLeft = 0;
                    }
                    else
                    {
                        NBulletLeft -= m_ClipSize;
                        NBulletInCharger = m_ClipSize;
                    }
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_Firerate;
                    m_Reloading = false;
                    m_CanShoot = true;
                }

            }
            else
            {

                if (NBulletInCharger <= 0)
                {
                    m_CanShoot = false;
                    m_Reloading = true;
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_ReloadingTime;
                    m_WeaponTimer.Start();

                }
                else
                {
                    m_CanShoot = true;
                }
            }

        }
        public override void MouseUp()
        {
            //Do nothing in this case (somewhat useless)
        }




    }
    sealed class Shotgun : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
       
        private readonly Random m_RNG;
   
        public override string WeaponName { get { return "Shotgun"; } }
        private const byte m_BulletSpeed = 15;
        private const int m_ReloadingTime = 700;
        private const int m_SpreadAngle = 30;
        private const int m_NumberOfBuckshot = 10;
        private const int m_ClipSize = 8;
        private const int m_Firerate = 600;
        private bool m_CanShoot = true;
        private bool m_Reloading;
        private readonly System.Timers.Timer m_WeaponTimer;


        private PlayerData m_Player;

        private volatile bool m_ShootingSound;
        public override void PlayShootingSound()
        {
            m_ShootingSound = false;
            new Task(Sound).Start();
        }
        private void Sound()
        {

            using (WaveStream BlockAlignedStream = new WaveFileReader(MultiplayerTopDownShooter.Properties.Resources.ShotgunSound))
            {
                using (WaveOut WaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    WaveOut.Init(BlockAlignedStream);
                    WaveOut.Play();
                    m_ShootingSound = true;
                    while (WaveOut.PlaybackState == PlaybackState.Playing && m_ShootingSound)
                        Thread.Sleep(15);
                }
            }
        }

        public override PlayerData User
        {
            set
            {
                m_Player = value;
            }
        }

        public Shotgun(PlayerData Player)
        {
            m_Player = Player;


            m_RNG = new Random();
            NBulletLeft = 16;//int.MaxValue - 100;
            NBulletInCharger = m_ClipSize;
            m_WeaponTimer = new System.Timers.Timer(m_Firerate) {AutoReset = false};
            m_WeaponTimer.Elapsed += _timer_Elapsed;

        }

        public override void Reload()
        {
            if (!m_Reloading && NBulletInCharger < m_ClipSize)
            {
      //          NBulletLeft += NBulletInCharger;
     //           NBulletInCharger = 0;
                m_Reloading = true;
                m_CanShoot = false;
                m_WeaponTimer.Interval = m_ReloadingTime;
                m_WeaponTimer.Start();
            }
        }

        private PointF m_MouseDir;
        public override PointF MouseDirection
        {
            set { m_MouseDir = value; }
        }

 
        public override void MouseDown(PointF MouseDir)
        {
            if (m_CanShoot)
            {
                m_CanShoot = false;
                m_WeaponTimer.Start();
                m_MouseDir = MouseDir;
                NBulletInCharger--;

                for (int i = 0; i < m_NumberOfBuckshot; i++)
                {
                    double Radians = Math.Atan2(m_MouseDir.Y, m_MouseDir.X) + ((m_RNG.NextDouble() * m_SpreadAngle) - m_SpreadAngle / 2.0) * (Math.PI / 180.0);
                    MouseDir = new PointF((float)Math.Cos(Radians), (float)Math.Sin(Radians));
                    m_Player.AddProjectile(new Projectile(m_Player.Position, MouseDir, m_BulletSpeed, (byte)ProjectileType.Bullet));
                }
                OnShot(this, (byte)WeaponSound.Shotgun);
                PlayShootingSound();
            }
            else
            {
                m_Reloading = false;
                if (NBulletInCharger > 0)
                {
                    m_CanShoot = true;
                }
                m_WeaponTimer.Interval = m_Firerate;

                m_WeaponTimer.Start();
            }
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (m_Reloading)
            {
                if (NBulletLeft <= 0)
                {
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_ReloadingTime;
                    m_WeaponTimer.Start();
                }
                else
                {
                    if (NBulletLeft > 0)
                    {
                        NBulletLeft--;
                        NBulletInCharger++;
                        if (NBulletInCharger < m_ClipSize && NBulletLeft > 0)
                        {
                            m_WeaponTimer.Start();
                        }
                        else
                        {
                            m_WeaponTimer.Stop();
                            m_WeaponTimer.Interval = m_Firerate;
                            m_Reloading = false;
                            m_CanShoot = true;
                        }
                    }
                    
                    
                }

            }
            else
            {

                if (NBulletInCharger <= 0)
                {
                    m_CanShoot = false;
                    m_Reloading = true;
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_ReloadingTime;
                    m_WeaponTimer.Start();

                }
                else
                {
                    m_CanShoot = true;
                }
            }

        }
        public override void MouseUp()
        {
            //Do nothing in this case (somewhat useless)
        }

    }
    sealed class RocketLauncher : Weapons
    {
        public override int NBulletLeft { get; set; }
        public override int NBulletInCharger { get; set; }
     
        public override string WeaponName { get { return "Lance roquette"; } }
        private const byte m_BulletSpeed = 10;
        private const int m_ReloadingTime = 5000;
        private const int m_ClipSize = 1;
        private const int m_Firerate = 500;
        private const int m_SpreadAngle = 5;
        private readonly Random m_RNG = new Random();
        private bool m_CanShoot = true;
        private bool m_Reloading;
        private readonly System.Timers.Timer m_WeaponTimer;
        private bool m_IsAiming;
        private bool m_HasExploded;

        private PlayerData m_Player;
        private volatile bool m_ShootingSound;
        public override void PlayShootingSound()
        {
            m_ShootingSound = false;
            new Task(Sound).Start();
        }
        private void Sound()
        {

            using (WaveStream BlockAlignedStream = new WaveFileReader(MultiplayerTopDownShooter.Properties.Resources.RocketLauncherSound))
            {
                using (WaveOut WaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    WaveOut.Init(BlockAlignedStream);
                    WaveOut.Play();
                    m_ShootingSound = true;
                    while (WaveOut.PlaybackState == PlaybackState.Playing && m_ShootingSound)
                        Thread.Sleep(15);
                }
            }
        }

        public override PlayerData User
        {
            set
            {
                m_Player = value;
            }
        }



        public override void Reload()
        {
            //this one is somewhat useless since you can't reload the Rocket Launcher
           /* if (!Reloading)
            {
                NBulletLeft += NBulletInCharger;
                NBulletInCharger = 0;
                Reloading = true;
                CanShoot = false;
                m_WeaponTimer.Interval = ReloadingTime;
                m_WeaponTimer.Start();
            }*/

        }

        private PointF m_MouseDir;
        public override PointF MouseDirection
        {
            set { m_MouseDir = value; }
        }

        public RocketLauncher(PlayerData Player)
        {
            m_Player = Player;
            NBulletLeft = 5;//int.MaxValue - 100;
            NBulletInCharger = m_ClipSize;
            m_WeaponTimer = new System.Timers.Timer(m_Firerate) {AutoReset = false};
            m_WeaponTimer.Elapsed += _timer_Elapsed;
            m_IsAiming = false;
            m_HasExploded = true;

        }


        private volatile bool m_ExplosionSound;
        public void PlayExplosionSound()
        {
            m_ExplosionSound = false;
            new Task(Explosion).Start();
        }
        private void Explosion()
        {
            
            using (WaveStream BlockAlignedStream = new WaveFileReader(MultiplayerTopDownShooter.Properties.Resources.ExplosionSound))
            {
                using (WaveOut WaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    WaveOut.Init(BlockAlignedStream);
                    WaveOut.Play();
                    m_ExplosionSound = true;
                    while (WaveOut.PlaybackState == PlaybackState.Playing && m_ExplosionSound)
                        Thread.Sleep(15);
                }
            }
        }


        public void BringTheHavoc()//make it go boom
        {
            m_HasExploded = true;


            OnShot(this, (byte)WeaponSound.Explosion);
            PlayExplosionSound();


            lock (m_Player.BulletLock)
            {
                for (int i = m_Player.Bullet.Count - 1; i >= 0; i--)
                {
                    if (m_Player.Bullet[i].TypeOfProjectile == (byte)ProjectileType.Rocket)
                    {
                        for (int j = 0; j < 30; j++)
                        {
                            double Radians = Math.Atan2(m_Player.Bullet[i].Direction.Y, m_Player.Bullet[i].Direction.X) + ((m_RNG.NextDouble() * 340) - 340 / 2.0) * (Math.PI / 180.0);
                            MouseDirection = new PointF((float)Math.Cos(Radians), (float)Math.Sin(Radians));
                            m_Player.AddProjectile(new Projectile(m_Player.Bullet[i].Position, m_MouseDir, m_BulletSpeed, (byte)ProjectileType.Bullet));
                        }
                        m_Player.Bullet.RemoveAt(i);
                    }
                }
            }
        }





        public override void MouseDown(PointF MouseDir)
        {
            
            if (m_CanShoot)
            {
                    m_CanShoot = false;
                    m_IsAiming = true;
                    m_WeaponTimer.Start();
                    m_MouseDir = MouseDir;
            }
            if (!m_HasExploded)
            {
                BringTheHavoc();
            }
        }





        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_Reloading)
            {
                if (NBulletLeft > 0)
                {
                    NBulletInCharger = m_ClipSize;
                    NBulletLeft -= m_ClipSize;
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_Firerate;
                    m_Reloading = false;
                    m_CanShoot = true;
                }
            }
            else
            {
                if (m_IsAiming)
                {
                    NBulletInCharger--;
                    OnShot(this, (byte)WeaponSound.RocketLauncher);
                    PlayShootingSound();
                    double Radians = Math.Atan2(m_MouseDir.Y, m_MouseDir.X) + ((m_RNG.NextDouble() * m_SpreadAngle) - m_SpreadAngle / 2.0) * (Math.PI / 180.0);
                    MouseDirection = new PointF((float)Math.Cos(Radians), (float)Math.Sin(Radians));
                    m_Player.AddProjectile(new Projectile(m_Player.Position, m_MouseDir, m_BulletSpeed, (byte)ProjectileType.Rocket));
                    m_Reloading = true;
                    m_HasExploded = false;
                    m_WeaponTimer.Stop();
                    m_WeaponTimer.Interval = m_ReloadingTime;
                    m_WeaponTimer.Start();
                }
            }
        
        }
        public override void MouseUp()
        {
            m_IsAiming = false;
            if (NBulletInCharger > 0)
            {
                m_CanShoot = true;
            }
        }




    }

}
