using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ClonesEngine
{
    enum PacketUse : byte
    {
        AskNumberOfPlayer = 0, //{PacketUse,ID}
        AnswerNumberOfPlayer = 1, //{PacketUse,ID,PlayerCount}
        InfoJoueur = 2, //{PacketUse,ID,PlayerData}
        AskMap = 3, //{PacketUse,ID}
        AnswerMap = 4, //{PacketUse,ID,Data}
        ResetAllID = 5, //{PacketUse,ID}
        PlayerDisconnected = 6, //{PacketUse,ID,Data}?
        CloseConnection = 7, //{PacketUse,ID}
        Ping = 8, //{PacketUse,ID,TargetID,TickData}
        Pong = 9, //{PacketUse,ID,TargetID,TickData}
        AskAutoVerif = 10, //{PacketUse,ID}  gen random and keep in memory if rand is smaller than received then reset ID and PlayerCount
        AnswerAutoVerif = 11, //{PacketUse,ID,RandomData}
        InfoPlayerDamage = 12, //{PacketUse,ID,TagetID, Damage}
        AcknowledgeDamage = 13, //{PacketUse,ID}
        PlaySound = 14, //{PacketUse,ID,SoundToPlay}


    }
    class GestionnaireDePacket
    {
        private readonly object m_PlayerLock = new object();
        private UDPConnecter m_ConnectionUdp;
        private readonly PlayerData[] m_PlayerList = new PlayerData[255];

        private int[] m_PlayerTime = new int[255];
        private byte m_ID;
        private int m_AutoVerifData;
        private readonly Random m_RNG = new Random();
        private int m_LastTickCheck;

        private bool m_ResendDamage;
        private byte m_DamageTarget;
        private bool m_Updatable = true;
        private List<PlayerDamage> m_BulletDamage = new List<PlayerDamage>();

        private byte m_PlayerCount;
        private byte[] m_Receiver;
        private int m_PacketID;
        public byte m_SelectedWeapon = 0;

        private Map m_Murs;
        private int m_MapSeed;

        private readonly int m_LobbyPort;// = 54545;

        #region Propriete
        public Map Map
        {
            get { return m_Murs; }
        }
        public byte PlayerCount
        {
            get { return m_PlayerCount; }
        }
        public byte ID
        {
            get { return m_ID; }
        }
        public PlayerData[] PlayerList
        {
            get { return m_PlayerList; }
        }
        public int PacketID
        {
            get { return m_PacketID; }
        }
       
        protected int[] PlayerTime
        {
            get { return m_PlayerTime; }
            set { m_PlayerTime = value; }
        }

        public int LobbyPort
        {
            get
            {
                return m_LobbyPort;
            }
        }

        internal Map Murs
        {
            get
            {
                return m_Murs;
            }

            set
            {
                m_Murs = value;
            }
        }

        public object PlayerLock
        {
            get
            {
                return m_PlayerLock;
            }
        }

        #endregion


        public GestionnaireDePacket(int Lobby)
        {
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Point), true);
            ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(Point)].Add(1, "X").Add(2, "Y");
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(PointF), true);
            ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(PointF)].Add(1, "X").Add(2, "Y");
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Projectile), true);
            ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(Projectile)].Add(1, "Position").Add(2, "Direction").Add(3, "Velocite").Add("TypeOfProjectile");
            ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(Weapons)].Add(1, "NBulletLeft").Add(2, "NBulletInCharger");           

            m_MapSeed = 0;
            m_LobbyPort = Lobby;
            for (int i = 0; i < 255; i++)
            {
                m_PlayerList[i] = new PlayerData {Vitesse = Settings.DefaultPlayerSpeed};

                m_PlayerList[i].CallWeaponConstructor();
                m_PlayerTime[i] = Environment.TickCount;
            }
            m_ID = 0;
            m_PlayerCount = 0;
            m_PacketID = 0;
            DemarrerLaConnection();

            ThreadStart Start = Reception;
            Thread ThreadReception = new Thread(Start) {IsBackground = true};
            ThreadReception.Start();
        }
        public void DemarrerLaConnection()
        {
#if DEBUG
            try
            {
                m_ConnectionUdp = new UDPConnecter(m_LobbyPort);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message); 
            }
#else
            m_ConnectionUdp = new UDPConnecter(m_LobbyPort);
#endif
        }

        private void Reception()
        {
            int TryCount = 0;
            while (true)
            {
                do
                {
                    TryCount++;
                    Thread.Sleep(25);
                    Send(TramePreGen.AskNumberOfPlayer);
                    do
                    {
                        m_Receiver = m_ConnectionUdp.Receiver();
                        switch (m_Receiver[0])
                        {
                            case (byte)PacketUse.AskNumberOfPlayer:
                                if (m_ID != 0)
                                {
                                    Send(TramePreGen.AnswerListeJoueur(m_PlayerCount, m_ID));
                                }
                                break;

                            case (byte)PacketUse.AnswerNumberOfPlayer:

                                lock (m_PlayerLock)
                                {
                                    if (m_Receiver[2] > m_PlayerCount || m_ID == 0)
                                    {
                                        //this may reset the last players

                                        for (; m_PlayerCount < m_Receiver[2]; m_PlayerCount++)
                                        {
                                            m_PlayerList[m_PlayerCount] = new PlayerData(m_PlayerCount);
                                            m_PlayerTime[m_PlayerCount] = Environment.TickCount;
                                            
                                        }
                                        if (m_ID == 0)
                                        {
                                            m_ID = m_Receiver[2];
                                            m_ID++;
                                            m_PlayerCount++;
                                            m_PlayerList[m_ID] = new PlayerData(m_ID);
                                            m_PlayerTime[m_ID] = Environment.TickCount;


                                            Respawn();
                                            Send(TramePreGen.AnswerListeJoueur(m_PlayerCount, m_ID));
                                            Send(TramePreGen.AskAutoVerif(ID));
                                            Send(TramePreGen.AskMapSeed());
                                        }
                                    }
                                }

                                break;
                            case (byte)PacketUse.InfoJoueur:
                                lock (m_PlayerList[m_Receiver[1]].BulletLock)
                                {
                                   
                                    PlayerData tempPlayer = TramePreGen.ReceiverInfoJoueur(m_Receiver);
                                 
                                    if (m_Receiver[1] == m_ID)
                                    {
                                        tempPlayer.WeaponList = m_PlayerList[m_Receiver[1]].WeaponList;
                                        if (tempPlayer.WeaponList != null)
                                        {
                                            tempPlayer.UpdateWeaponWielder();
                                            for (int i = 0; i < (byte) WeaponType.NumberOfWeapons; i++)
                                            {
                                                tempPlayer.WeaponList[i].Shot -= OnSendSound;
                                                tempPlayer.WeaponList[i].Shot += OnSendSound;
                                            }

                                        }
                                        else
                                        {
                                            tempPlayer.CallWeaponConstructor();


                                            if (tempPlayer.WeaponList != null)
                                            {
                                                tempPlayer.UpdateWeaponWielder();
                                                for (int i = 0; i < (byte)WeaponType.NumberOfWeapons; i++)
                                                {
                                                    tempPlayer.WeaponList[i].Shot += OnSendSound;
                                                }

                                            }



                                        }
                                    }

                                    m_PlayerList[m_Receiver[1]] = tempPlayer;
                                    
                                    m_PlayerTime[m_Receiver[1]] = Environment.TickCount;
                                }
                                break;
                            case (byte)PacketUse.AskMap:
                                if (m_ID >= 1)
                                {
                                    Send(TramePreGen.AnswerMapSeed(m_ID, m_MapSeed));
                                }
                                break;
                            case (byte)PacketUse.AnswerMap:
                                if (m_MapSeed != BitConverter.ToInt32(m_Receiver, 2))
                                {
                                    m_MapSeed = BitConverter.ToInt32(m_Receiver, 2);
                                    m_Murs = new Map(m_MapSeed);
                                }
                                    
                                break;
                            case (byte)PacketUse.ResetAllID:
                                if (m_Receiver[1] == m_ID)
                                {
                                    m_ID = 1;
                                    m_PlayerCount = 1;
                                    Thread.Sleep(m_RNG.Next(0, 50));

                                }
                                else
                                {
                                    m_ID = 0;
                                    m_PlayerCount = 0;
                                    Send(TramePreGen.AskNumberOfPlayer);
                                }
                                break;

                            case (byte)PacketUse.Ping:
                                if (m_Receiver[2] == m_ID)
                                {
                                    m_Receiver[1] ^= m_Receiver[2] ^= m_Receiver[1] ^= m_Receiver[2];  //swap 1 w/ 2
                                }
                                m_Receiver[0] = (byte)PacketUse.Pong;
                                Send(m_Receiver);
                                break;

                            case (byte)PacketUse.AskAutoVerif:
                                if (m_Receiver[1] == m_ID)
                                {
                                    m_AutoVerifData = m_RNG.Next(1, int.MaxValue);
                                    Send(TramePreGen.AnswerAutoVerif(m_AutoVerifData, m_ID));
                                }
                                break;


                            case (byte)PacketUse.AnswerAutoVerif:
                                if (m_Receiver[1] == m_ID)
                                {   
                                    if (m_AutoVerifData < BitConverter.ToInt32(m_Receiver, 2))
                                    {
                                        m_ID = 0;
                                        m_PlayerCount = 0;

                                        Send(TramePreGen.AskNumberOfPlayer);
                                    }
                                }
                                break;

                            case (byte)PacketUse.InfoPlayerDamage:
                                if (m_Receiver[2] == m_ID)
                                {
                                    
                                    m_PlayerList[m_ID].CallWeaponConstructor();
                                    Send(TramePreGen.AcknowledgeDamage(m_ID));
                                    Respawn();
                                    Send(TramePreGen.InfoJoueur(m_PlayerList[ID], m_ID, m_PacketID));
                                }
                                break;
                            case (byte)PacketUse.AcknowledgeDamage:
                                if (m_Receiver[1] == m_DamageTarget)
                                {
                                    m_ResendDamage = false;
                                }
                                break;

                            case (byte)PacketUse.PlaySound:
                                if (m_Receiver[1] != m_ID)
                                {
                                    if (m_PlayerList[0].WeaponList != null)
                                    {
                                        if (m_Receiver[2] == (byte)WeaponSound.Explosion)
                                        {
                                            ((RocketLauncher)m_PlayerList[0].WeaponList[(byte)WeaponType.RocketLauncher]).PlayExplosionSound();
                                        }
                                        else
                                        {
                                            m_PlayerList[0].WeaponList[m_Receiver[2]].PlayShootingSound();
                                        }
                                    }
                                    else
                                    {
                                        System.Windows.Forms.MessageBox.Show("Mauvais son", "Debug");
                                    }
                                }
                                break;
                            default:
                                System.Windows.Forms.MessageBox.Show("Unknown package format");
                                break;
                        }//Switch case end

                        if (m_LastTickCheck + 2650 < Environment.TickCount)
                        {
                            m_LastTickCheck = Environment.TickCount + m_RNG.Next(1, 500);
                            Send(TramePreGen.AskAutoVerif(m_ID));
                            Send(TramePreGen.AskMapSeed());
                            if (m_MapSeed != 0 && m_ID != 0)
                            {
                                Send(TramePreGen.AnswerMapSeed(m_ID, m_MapSeed));
                            }
                        }

                    } while (m_ID != 0);//While(true)
                    lock (m_PlayerLock) {} //This isn't useless
                } while (TryCount < 100 && m_ID == 0);
                if (TryCount == 100 && m_ID == 0)
                {
                    m_MapSeed = m_RNG.Next() + 1;
                    m_Murs = new Map(m_MapSeed);
                    m_ID = 1;
                    m_PlayerCount = 1;
                    Respawn();
                }
                TryCount = 0;
                Thread.Sleep(150);
                Send(TramePreGen.AskAutoVerif(ID));
            }
        }

        public void Respawn()
        {
            bool Visible = true;
            PointF tempPoint = new PointF();
            lock (m_PlayerLock)
            {
                while (Visible)
                {
                    tempPoint = new PointF(m_RNG.Next(1, Settings.GameSize.Width - 1),
                        m_RNG.Next(1, Settings.GameSize.Height - 1));
                    if (PlayerCount > 1)
                    {
                        if (m_Murs != null)
                        {
                            Visible = false;

                        }
                        else
                        {
                            Visible = false;
                        }
                    }
                    else
                    {
                        Visible = false;
                    }
                }
                m_PlayerList[m_ID].Position = tempPoint;
            }
        }
        public void Send(byte[] data)
        {
            m_ConnectionUdp.Send(data);
            m_PacketID++;
        }

        public void OnSendSound(object w, byte e)
        {
            m_ConnectionUdp.Send(TramePreGen.PlaySound(m_ID, e));//This make the other players play the sounds from your guns
            m_PacketID++;
        }

        public void UpdatePlayer(byte PlayerID, Point MousePosition)
        {
            if (PlayerID != 0)
            {
                List<PlayerDamage> BulletDamageBuffer = m_PlayerList[PlayerID].UpdateStats(m_PlayerTime, Environment.TickCount, m_PlayerList, m_PlayerCount, m_ID, m_Murs, MousePosition);
                m_BulletDamage = m_BulletDamage.Concat(BulletDamageBuffer).ToList();

                if (m_Updatable)
                {
                    m_Updatable = false;
                    for (int i = m_BulletDamage.Count - 1; i >= 0; i--)
                    {
                        m_DamageTarget = m_BulletDamage[i].ID;
                        byte SendDamageTimeout = 12;
                        m_ResendDamage = true;
                        while (m_ResendDamage && SendDamageTimeout > 0)
                        {
                            Send(TramePreGen.PlayerDamage(m_ID, m_BulletDamage[i]));
                            Thread.Sleep(30);
                            SendDamageTimeout--;
                        }
                        m_BulletDamage.RemoveAt(i);
                    }
                    m_Updatable = true;
                }
            }
        }
    }
}
