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
    /// <summary>
    /// A racetimer used in the race class, keeps track of milliseconds/seconds/minutes
    /// </summary>
    public class RaceTimer
    {
        private bool running;
        private int _minutes;
        private int _seconds;
        private int _miliseconds;

        public void Start()
        {
            running = true;
        }

        public void Stop()
        {
            running = false;
        }

        public void Update()
        {
            if (!running) return;

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
        }

        public string GetAsText()
        {
            return _minutes.ToString("00") + ":" + _seconds.ToString("00") + ":" + _miliseconds.ToString("00");
        }

        public void ResetTimer()
        {
            _minutes = 0;
            _seconds = 0;
            _miliseconds = 0;
            Stop();
        }
    }
}
