using System;
using System.Collections.Generic;
using System.Text;

namespace ExtinctionRun
{
    /// <summary>
    /// Contains public accessors for retrieving various global constants
    /// </summary>
    public static class Constants
    {
        const string title = "Extinction Run";
        const int gameWidth = 1280;
        const int gameHeight = 960;

        public static string Title { get { return title; } }
        public static int GameWidth { get { return gameWidth; } }
        public static int GameHeight { get { return gameHeight; } }
    }
}
