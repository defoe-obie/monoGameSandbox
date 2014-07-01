using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox
{
    [Debug]
    public class Debug : Attribute
    {
        public override string ToString()
        {
            return "Some part of this section contains Debugger code.";
        }
    }

    public static class Constants
    {
        public const int ScreenWidth = 700;
        public const int ScreenHeight = 420;
        public const int PlayAreaWidth = 476;

        public const int MessageAreaWidth = ScreenWidth - PlayAreaWidth;
        public const int TileSize = 28;
        public const int ScreenTilesAcross = PlayAreaWidth / TileSize;
        public const int ScreenTilesDown = ScreenHeight / TileSize;

        public static SpriteFont defaultFont;
        public static Texture2D defaultLookAndFeel;
        public static Texture2D defaultLookAndFeelBackground;
        public static Texture2D defaultLookAndFeelForeground;

    }
}

