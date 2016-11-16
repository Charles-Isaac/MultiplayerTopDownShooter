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
        const byte _DeFaultBulletSpeed = 6;

        const int _GameSizeX = 1000;
        const int _GameSizeY = 1000;

        public static Size GameSize
        {
            get { return new Size(_GameSizeX, _GameSizeY); }
        }
        public static byte DefaultPlayerSpeed
        {
            get { return _DefaultPlayerSpeed; }
        }
        public static byte DefaultBulletSpeed
        {
            get { return _DeFaultBulletSpeed; }
        }
    }
}
