using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectHook.GameFrameWork
{
    public static class Camera
    {
        public static Vector2 Position;
        public static Vector2 Scale = new Vector2(1, 1);

        private static float _zoom;
        public static float Zoom
        {
            get { return _zoom; }
            set
            {
                var percentage = _zoom*0.01f;
                Scale = new Vector2(percentage, percentage);
                _zoom = value;
            }
        }

        public static int Width = 768;
        public static int Height = 432;
    }
}
