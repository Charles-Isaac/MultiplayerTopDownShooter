using System.Net;
using System.Net.Sockets;

namespace ClonesEngine
{
    class UDPConnecter
    {
        IPEndPoint endPoint;
        public UDPConnecter(int LobbyPort)
        {
            port = LobbyPort;
            InitializeSender();
            
            //InitializeReceiver();
            receivingClient = new UdpClient();
            receivingClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receivingClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            /*
                        receivingClient = new UdpClient(port);*/
            endPoint = new IPEndPoint(IPAddress.Any, port);
        }


        //byte[] m_Receiver;
        /*public byte[] Data
        {
            get { return m_Receiver; }
        }*/

        //string userName;
       
        int port = 54545;
        const string broadcastAddress = "255.255.255.255";//"127.0.0.1";

        UdpClient receivingClient;
        UdpClient sendingClient;

        //public Thread receivingThread;
        private void InitializeSender()
        {
            sendingClient = new UdpClient(broadcastAddress, port);
            sendingClient.EnableBroadcast = true;
            
        }
        
        public byte[] Receiver()
        {
                return receivingClient.Receive(ref endPoint);
        }

        public void Send(byte[] DataToSend)
        {
            if (DataToSend != null)
            {
                sendingClient.Send(DataToSend, DataToSend.Length);
            }
        }
    }
}
