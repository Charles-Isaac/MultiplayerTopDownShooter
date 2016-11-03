using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Drawing;

namespace ClonesEngine
{
    enum PacketUse
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


    }
    class GestionnaireDePacket
    {
        Stopwatch TickCounter;
        UDPConnecter ConnectionUDP;
        Thread ThreadReception;
        //public TramePreGen PreGen;
        PlayerData[] m_PlayerList = new PlayerData[255];
        int[] m_PlayerTime = new int[256];
        byte m_ID;
        int AutoVerifData = 0;
        Random RNG = new Random();
        int LastTickCheck = 0;


        byte m_PlayerCount;
        //byte[] m_PlayerList = new byte[255];
        //byte[,] m_LastPackage = new byte[255, 9];
        byte[] m_Receiver;
        //byte[] m_PlayerIntList = new byte[255];
        int m_PacketID;



        #region Propriete
        /*public byte[] PlayerList
        {
            get { return m_PlayerList; }
        }*/
        public byte PlayerCount
        {
            get { return m_PlayerCount; }
        }
        public byte ID
        {
            get { return m_ID; }
        }
        /*
        public byte[,] LastPackage
        {
            get { return m_LastPackage; }
        }*/
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

        #endregion


        public GestionnaireDePacket()
        {
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Point), true);
            ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(Point)].Add(1, "X").Add(2, "Y");
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(PointF), true);
            ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(PointF)].Add(1, "X").Add(2, "Y");

            for (int i = 0; i < 255; i++)
            {
                m_PlayerList[i] = new PlayerData();
                m_PlayerTime[i] = Environment.TickCount;
            }
            m_ID = 0;
            m_PlayerCount = 0;
            m_PacketID = 0;
            //PreGen = new TramePreGen();
            DemarrerLaConnection();

            ThreadStart start = new ThreadStart(Reception);
            ThreadReception = new Thread(start);
            ThreadReception.IsBackground = true;
            ThreadReception.Start();
            TickCounter = new Stopwatch();
            TickCounter.Start();
            //GetPlayerCount();
        }
        public bool DemarrerLaConnection()
        {
            try
            {
                ConnectionUDP = new UDPConnecter();
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                throw e;
            }
        }

        private void Reception()
        {
            int TryCount = 0;
            while (true)
            {


                do
                {
                    Exit();
                    TryCount++;
                    Thread.Sleep(250);
                    Send(TramePreGen.AskNumberOfPlayer);
                    Thread.Sleep(50);
                    do
                    {
                        m_Receiver = ConnectionUDP.Receiver();
                        switch (m_Receiver[0])
                        {
                            case (byte)PacketUse.AskNumberOfPlayer:
                                if (m_ID != 0)
                                {
                                    Send(TramePreGen.AnswerListeJoueur(m_PlayerCount, m_ID));
                                }
                                break;

                            case (byte)PacketUse.AnswerNumberOfPlayer:

                                Enter();
                                if (m_Receiver[2] > m_PlayerCount || m_ID == 0)
                                {
                                    //this may reset the last player

                                    for (; m_PlayerCount < m_Receiver[2]; m_PlayerCount++)
                                    {
                                        m_PlayerList[m_PlayerCount] = new PlayerData(m_PlayerCount, TickCounter.ElapsedTicks);
                                        m_PlayerTime[m_PlayerCount] = Environment.TickCount;
                                    }
                                    if (m_ID == 0)
                                    {
                                        m_ID = m_Receiver[2];
                                        m_ID++;
                                        m_PlayerCount++;
                                        m_PlayerList[m_ID] = new PlayerData(m_ID, TickCounter.ElapsedTicks);
                                        m_PlayerTime[m_ID] = Environment.TickCount;

                                        Send(TramePreGen.AnswerListeJoueur(m_PlayerCount, m_ID));
                                        Send(TramePreGen.AskAutoVerif(ID));
                                    }
                                }
                                Exit();

                                break;
                            case (byte)PacketUse.InfoJoueur:

                                m_PlayerList[m_Receiver[1]] = TramePreGen.ReceiverInfoJoueur(m_Receiver);
                                m_PlayerTime[m_Receiver[1]] = Environment.TickCount;



                                break;
                            case (byte)PacketUse.ResetAllID:
                                if (m_Receiver[1] == m_ID)
                                {
                                    m_ID = 1;
                                    m_PlayerCount = 1;
                                    Thread.Sleep(RNG.Next(0, 50));
                                    //GenMap();
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
                                    AutoVerifData = RNG.Next(1, int.MaxValue);
                                    Send(TramePreGen.AnswerAutoVerif(AutoVerifData, m_ID));
                                }
                                break;

                            case (byte)PacketUse.AnswerAutoVerif:
                                if (m_Receiver[1] == m_ID)
                                {
                                    if (AutoVerifData < BitConverter.ToInt32(m_Receiver, 2))
                                    {
                                        m_ID = 0;
                                        m_PlayerCount = 0;

                                        Send(TramePreGen.AskNumberOfPlayer);
                                    }
                                }
                                break;

                            default:
                                break;
                        }//Switch

                        if (LastTickCheck + 2500 < Environment.TickCount)
                        {
                            LastTickCheck = Environment.TickCount + RNG.Next(1, 500);
                            Send(TramePreGen.AskAutoVerif(m_ID));
                        }

                    } while (m_ID != 0);//While(true)
                    Enter();
                } while (TryCount < 10 && m_ID == 0);
                if (TryCount == 10 && m_ID == 0)
                {
                    //GenMap();
                    m_ID = 1;
                    m_PlayerCount = 1;

                    //Send(TramePreGen.AnswerMap)
                }
                TryCount = 0;
                Thread.Sleep(150);
                Send(TramePreGen.AskAutoVerif(ID));
            }
        }




        private bool Locked = false;
        public void Enter()
        {
            // Yield until acquired is false
            while (Locked) { Thread.Sleep(1); }
            Locked = true;
        }
        public void Exit()
        {
            Locked = false;
        }

        public void Send(byte[] data)
        {
            ConnectionUDP.Send(data);
            m_PacketID++;
        }

        public void Send(string sdata)
        {

        }

        public void UpdatePlayer(byte ID)
        {
            if (ID != 0)
            {
                m_PlayerList[ID].UpdateStats(m_PlayerTime[ID]);
                m_PlayerTime[ID] = Environment.TickCount;
            }
        }
    }
}
