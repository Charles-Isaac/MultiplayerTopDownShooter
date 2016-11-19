using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClonesEngine
{
    static class Settings
    {


        const byte _DefaultPlayerSpeed = 5;
        const byte _DeFaultBulletSpeed = 15;
        const byte _DefaltBulletSize = 12;
        const byte _PlayerSize = 80;

        const int _GameSizeX = 1500;
        const int _GameSizeY = 1500;

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
