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
    public class Race
    {
        private bool _started;
        public int Timer;

        public void StartRace()
        {
            _started = true;
        }

        public void FinishRace()
        {
            _started = false;
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
                Timer++;
        }
    }
}
