using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            byte[] Answer = new byte[3/*1 + 1 + 1*/];
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
            int i = 0;
            byte[] JoueurBytes = Serialize(Joueur);
            byte[] Packet = new byte[1 + 1 + 4 + JoueurBytes.Length];
            byte[] NoPacketBytes = BitConverter.GetBytes(NoPacket);
            Packet[i++] = (byte)PacketUse.InfoJoueur;
            Packet[i++] = ID;
            for (i = 2; i < 6; i++)
            {
                Packet[i] = NoPacketBytes[i - 2];
            }
            for (i = 0; i < JoueurBytes.Length; i++)
            {
                Packet[i + 6] = JoueurBytes[i];
            }
            return Packet;
        }
        public static byte[] Serialize(PlayerData tData)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, tData);
                return ms.ToArray();
            }
        }
        public static PlayerData Deserialize(byte[] tData)
        {
            using (var ms = new MemoryStream(tData))
            {
                return ProtoBuf.Serializer.Deserialize<PlayerData>(ms);
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
            byte[] bData = BitConverter.GetBytes(Data);
            byte[] Trame = new byte[6];
            Trame[0] = (byte)PacketUse.AnswerAutoVerif;
            Trame[1] = ID;

            for (int i = 0; i < bData.Length; i++)
            {
                Trame[i + 2] = bData[i];
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
    }
}
