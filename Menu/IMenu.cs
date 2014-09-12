using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHook
{
    internal interface IMenu
    {
        void ShowMenu();
        void Update(GameTime gameTime);

    }
}