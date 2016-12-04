using System;
using System.IO;

namespace ClonesEngine
{
    static class TramePreGen
    {

        public static byte[] AskNumberOfPlayer
        {
            get { return new byte[1]; }
        }

        public static byte[] AnswerListeJoueur(byte NJoueur, byte ID)
        {
            byte[] Answer = new byte[3];
            Answer[0] = (byte)PacketUse.AnswerNumberOfPlayer;
            Answer[1] = ID;
            Answer[2] = NJoueur;
            return Answer;
        }

        public static PlayerData ReceiverInfoJoueur(byte[] Data)
        {
            byte[] Player = new byte[Data.Length - 6];
            for (int i = 0; i < Player.Length; i++)
            {
                Player[i] = Data[i + 6];
            }
            return Deserialize(Player);
        }



        public static byte[] InfoJoueur(PlayerData Joueur, byte ID, int NoPacket)
        {
            
            byte[] JoueurBytes = Serialize(Joueur);
            byte[] Packet = new byte[1 + 1 + 4 + JoueurBytes.Length];
            byte[] NoPacketBytes = BitConverter.GetBytes(NoPacket);
            Packet[0] = (byte)PacketUse.InfoJoueur;
            Packet[1] = ID;
            for (int i = 2; i < 6; i++)
            {
                Packet[i] = NoPacketBytes[i - 2];
            }
            for (int i = 0; i < JoueurBytes.Length; i++)
            {
                Packet[i + 6] = JoueurBytes[i];
            }
            return Packet;
        }

        

        public static byte[] Serialize(PlayerData TData)
        {
            using (MemoryStream MemStream = new MemoryStream())
            {
                lock (TData.BulletLock)
                {
                    ProtoBuf.Serializer.Serialize(MemStream, TData);
                }
                return MemStream.ToArray();
            }
        }
        public static PlayerData Deserialize(byte[] TData)
        {
            using (MemoryStream MemStream = new MemoryStream(TData))
            {
                return ProtoBuf.Serializer.Deserialize<PlayerData>(MemStream);
            }
        }

        public static byte[] AskAutoVerif(byte ID)
        {
            byte[] Trame = new byte[2];
            Trame[0] = (byte)PacketUse.AskAutoVerif;
            Trame[1] = ID;
            return Trame;
        }
        public static byte[] AnswerAutoVerif(int Data, byte ID)
        {
            byte[] ByteData = BitConverter.GetBytes(Data);
            byte[] Trame = new byte[6];
            Trame[0] = (byte)PacketUse.AnswerAutoVerif;
            Trame[1] = ID;

            for (int i = 0; i < ByteData.Length; i++)
            {
                Trame[i + 2] = ByteData[i];
            }

            return Trame;
        }

        public static byte[] PlayerDamage(byte ID, PlayerDamage Info)
        {
            byte[] Trame = new byte[4];
            Trame[0] = (byte)PacketUse.InfoPlayerDamage;
            Trame[1] = ID;
            Trame[2] = Info.ID;
            Trame[3] = Info.Damage;

            return Trame;
        }
        public static byte[] AcknowledgeDamage(byte ID)
        {
            byte[] Trame = new byte[2];
            Trame[0] = (byte)PacketUse.AcknowledgeDamage;
            Trame[1] = ID;
            return Trame;
        }
        public static byte[] AnswerMapSeed(byte ID, int Seed)
        {
            byte[] Trame = new byte[6];
            byte[] ByteSeed = BitConverter.GetBytes(Seed);
            Trame[0] = (byte)PacketUse.AnswerMap;
            Trame[1] = ID;
            for (int i = 0; i < 4; i++)
            {
                Trame[i + 2] = ByteSeed[i];
            }
            return Trame;
        }
        public static byte[] AskMapSeed()
        {
            return new[] { (byte)PacketUse.AskMap };
        }

        public static byte[] PlaySound(byte ID, byte SoundID)
        {
            byte[] Trame = new byte[3];

            Trame[0] = (byte)PacketUse.PlaySound;
            Trame[1] = ID;
            Trame[2] = SoundID;


            return Trame;
        }
    }
}
