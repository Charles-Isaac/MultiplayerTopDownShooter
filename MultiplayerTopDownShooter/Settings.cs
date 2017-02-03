using System.Drawing;

namespace ClonesEngine
{
    static class Settings
    {


        private const byte m_DefaultPlayerSpeed = 5;
        private const float m_DeFaultBulletSpeed = 1.2F;
        private const byte m_DefaltBulletSize = 18;//12
        private const byte m_PlayerSize = 80;
        private const int m_NumberOfBuilding = 40;//16;

        private const float m_GameStretchX = 16F/11;//2000;
        private const float m_GameStretchY = 1F;//2000;

        private const int m_GameSizeX = (int)(3000* m_GameStretchX);//2000;
        private const int m_GameSizeY = (int)(3000 * m_GameStretchY);//2000;

       

        public static byte DefaultBulletSize
        {
            get { return m_DefaltBulletSize; }
        }

        public static Size GameSize
        {
            get { return new Size(m_GameSizeX, m_GameSizeY); }
        }
        public static byte DefaultPlayerSpeed
        {
            get { return m_DefaultPlayerSpeed; }
        }
        public static int NumberOfBuilding
        {
            get { return m_NumberOfBuilding; }
        }
        public static byte DefaultPlayerSize
        {
            get { return m_PlayerSize; }
        }
        public static float DefaultBulletSpeed
        {
            get { return m_DeFaultBulletSpeed; }
        }

        public static float GameStretchX
        {
            get
            {
                return m_GameStretchX;
            }
        }

        public static float GameStretchY
        {
            get
            {
                return m_GameStretchY;
            }
        }
    }
}
