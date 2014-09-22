using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.Menu;

namespace ProjectHook
{
    public interface IMenu
    {
        void ShowMenu();
        void Update(GameTime gameTime);
        void OpenSelection();
    }
}