using System.Drawing;

namespace ClonesEngine
{
    static class Settings
    {


        const byte _DefaultPlayerSpeed = 5;
        const byte _DeFaultBulletSpeed = 15;
        const byte _DefaltBulletSize = 24;//12
        const byte _PlayerSize = 80;
        const byte _NumberOfWalls = 10;

        const int _GameSizeX = 2000;
        const int _GameSizeY = 2000;

        public static byte DefaultBulletSize
        {
            get { return _DefaltBulletSize; }
        }

        public static Size GameSize
        {
            get { return new Size(_GameSizeX, _GameSizeY); }
        }
        public static byte DefaultPlayerSpeed
        {
            get { return _DefaultPlayerSpeed; }
        }
        public static byte NumberOfWalls
        {
            get { return _NumberOfWalls; }
        }
        public static byte DefaultPlayerSize
        {
            get { return _PlayerSize; }
        }
        public static byte DefaultBulletSpeed
        {
            get { return _DeFaultBulletSpeed; }
        }
    }
}
