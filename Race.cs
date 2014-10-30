using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    /// <summary>
    /// Used when in a level, keeps track of a race timer
    /// can see who's in first (who is most to the right)
    /// </summary>
    public class Race
    {
        public bool IsStarted;
        public RaceTimer Timer = new RaceTimer();
        public RaceTimer CountDown;
        public PlayerIndex FirstPlace;
        private bool _recorded;

        public void StartRace()
        {
            CountDown = new RaceTimer(0, 3); //Start countdown at 3
            _recorded = false;     
        }

        public void FinishRace(Globals.Levels currentLevel)
        {
            string filepath = @"../../" + currentLevel + "records.txt";

            if (!File.Exists(filepath))
            {
                FileStream fs = File.Create(filepath);
                fs.Close();
            }

            IsStarted = false;
            Timer.Stop();

            if (!_recorded)
            {
                //Take Windows User Name
                File.AppendAllText(filepath, Timer.GetAsText() + " by " + Environment.UserName + "\n");
                _recorded = true;
            }
        }

        public void CalcFirstPlace()
        {
            var maxPosition = Collections.Players.Max(p => p.PositionX);
            foreach (var player in Collections.Players)
            {
                if (player.PositionX >= maxPosition)
                    FirstPlace = player.PlayerIndex;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!IsStarted)
            {
                if (!CountDown.CountDownUpdate())
                {
                    IsStarted = true;
                    Timer.Start();
                }
            }
                
            if (IsStarted)
                Timer.Update();
        }
    }
}
