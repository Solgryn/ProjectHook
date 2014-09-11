using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    class Menu : TextObject
    {

        private List<String> menuItems = new List<string>() {"single player", "two player"}; 
        public Menu(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            
        }





    }
}
