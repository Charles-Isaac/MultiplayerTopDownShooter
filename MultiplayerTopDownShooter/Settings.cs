﻿using System.Drawing;

namespace ClonesEngine
{
    static class Settings
    {


        private const byte m_DefaultPlayerSpeed = 5;
        private const byte m_DeFaultBulletSpeed = 15;
        private const byte m_DefaltBulletSize = 18;//12
        private const byte m_PlayerSize = 80;
        private const byte m_NumberOfWalls = 100;

        private const int m_GameSizeX = 2000;
        private const int m_GameSizeY = 2000;

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
        public static byte NumberOfWalls
        {
            get { return m_NumberOfWalls; }
        }
        public static byte DefaultPlayerSize
        {
            get { return m_PlayerSize; }
        }
        public static byte DefaultBulletSpeed
        {
            get { return m_DeFaultBulletSpeed; }
        }
    }
}
