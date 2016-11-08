using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace ClonesEngine
{
    [XmlType]
    class PlayerDamage
    {
        byte m_ID;
        byte m_Damage;

        public PlayerDamage()
        {
            m_ID = 0;
            m_Damage = 0;
        }
        public PlayerDamage(byte ID, byte Damage)
        {
            m_ID = ID;
            m_Damage = Damage;
        }

        public byte ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        public byte Damage
        {
            get { return m_Damage; }
            set { m_Damage = value; }
        }
    }
}
