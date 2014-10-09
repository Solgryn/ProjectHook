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
        private bool _started;
        public RaceTimer Timer = new RaceTimer();

        public void StartRace()
        {
            _started = true;
            Timer.Start();
        }

        public void FinishRace()
        {
            _started = false;
            Timer.Stop();
        }

        public PlayerIndex GetFirstPlace()
        {
            var maxPosition = Collections.Players.Max(p => p.PositionX);
            foreach (var player in Collections.Players)
            {
                if(player.PositionX >= maxPosition)
                    return player.PlayerIndex;
            }
            return default(PlayerIndex);
        }

        public void Update(GameTime gameTime)
        {
            if(_started)
                Timer.Update();
        }
    }
}
