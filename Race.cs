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
    /// Used when in a level, keeps track of a race timer
    /// can see who's in first (who is most to the right)
    /// </summary>
    public class Race
    {
        public bool IsStarted;
        public RaceTimer Timer = new RaceTimer();
        public PlayerIndex FirstPlace;

        public void StartRace()
        {
            IsStarted = true;
            Timer.Start();
        }

        public void FinishRace()
        {
            IsStarted = false;
            Timer.Stop();
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
            if (IsStarted)
                Timer.Update();
        }
    }
}
