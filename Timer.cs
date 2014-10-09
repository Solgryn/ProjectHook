using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    class Timer : TextObject
    {
        private int _minutes;
        private int _seconds;
        private int _miliseconds;
        private TextObject _timer;
        private string timerText = "ss";
        private Dictionary<Globals.Levels, string> records = new Dictionary<Globals.Levels, string>();

        GameHost _game;

        public Timer(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public void StartTimer(Globals.Levels level)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            _miliseconds++;
            if (_miliseconds == 60)
            {
                _seconds++;
                _miliseconds = 0;
            }
            if (_seconds == 60)
            {
                _minutes++;
                _seconds = 0;
           }

            Counter();

        }

        private void Counter()
        {
            if (_miliseconds < 10 && _seconds < 10)
                Text = "0" + _minutes + ":0" + _seconds + ":0" + _miliseconds;
            else if (_miliseconds < 10 && _seconds > 10)
                Text = "0" + _minutes + ":" + _seconds + ":0" + _miliseconds;
            else if (_seconds < 10 && _miliseconds > 10)
                Text = "0" + _minutes + ":0" + _seconds + ":" + _miliseconds;
            else
                Text = "0" + _minutes + ":" + _seconds + ":" + _miliseconds;
        }



        internal void ResetTimer()
        {
            Text = "";
        }
    }
}
