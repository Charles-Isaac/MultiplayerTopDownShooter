using System.Net;
using System.Net.Sockets;

namespace ClonesEngine
{
    class UDPConnecter
    {
        private IPEndPoint m_EndPoint;
        public UDPConnecter(int LobbyPort)
        {
            m_Port = LobbyPort;
            InitializeSender();
            
            //InitializeReceiver();
            m_ReceivingClient = new UdpClient();
            m_ReceivingClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            m_ReceivingClient.Client.Bind(new IPEndPoint(IPAddress.Any, m_Port));
            /*
                        receivingClient = new UdpClient(port);*/
            m_EndPoint = new IPEndPoint(IPAddress.Any, m_Port);
        }


        //byte[] m_Receiver;
        /*public byte[] Data
        {
            get { return m_Receiver; }
        }*/

        //string userName;
       
        private readonly int m_Port;// = 54545;
        private const string m_BroadcastAddress = "255.255.255.255";//"127.0.0.1";

        private readonly UdpClient m_ReceivingClient;
        private UdpClient m_SendingClient;

        //public Thread receivingThread;
        private void InitializeSender()
        {
            m_SendingClient = new UdpClient(m_BroadcastAddress, m_Port) {EnableBroadcast = true};

        }
        
        public byte[] Receiver()
        {
                return m_ReceivingClient.Receive(ref m_EndPoint);
        }

        public void Send(byte[] DataToSend)
        {
            if (DataToSend != null)
            {
                m_SendingClient.Send(DataToSend, DataToSend.Length);
            }
        }
    }
}
