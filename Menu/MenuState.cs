using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectHook.Menu
{
    public abstract class MenuState
    {
        public enum TitleScreen
        {
            Race,
            Options,
            Exit,
        }

        public enum PauseMenu
        {
            Resume,
            Options,
            Exit
        }

        public enum Race
        {
            Start,
            Exit
        }
    }
}
