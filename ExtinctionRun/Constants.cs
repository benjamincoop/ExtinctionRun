using System;
using Microsoft.Xna.Framework;

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
        const int terrainHeight = 192;
        const int playerWidth = 388;
        const int playerHeight = 357;
        const float playerScale = 0.25f;
        const float forceJump = -750f;
        const float forceGravity = 25f;

        public static string Title { get { return title; } }
        public static int GameWidth { get { return gameWidth; } }
        public static int GameHeight { get { return gameHeight; } }
        public static int TerrainHeight { get { return terrainHeight; } }
        public static int PlayerWidth { get { return playerWidth; } }
        public static int PlayerHeight { get { return playerHeight; } }
        public static float PlayerScale {  get { return playerScale; } }
        public static float ForceJump {  get { return forceJump; } }
        public static float ForceGravity {  get { return forceGravity; } }
    }
}
