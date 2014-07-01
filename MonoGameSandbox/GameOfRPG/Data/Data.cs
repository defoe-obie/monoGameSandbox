using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Reflection;

namespace MonoGameSandbox
{
    public static class Data
    {
        public static readonly TileData Tiles;
        public static readonly MapData Maps;
        public static readonly EventData Events;

        public static string ProjectPath;
        public static string DataPath;

        static Data(){

            //string app_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string prefix = Path.Combine(app_dir, "..", "Contents");
            ProjectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);//Path.GetFullPath(prefix);
            //Console.WriteLine(ProjectPath);
            DataPath = Path.Combine(ProjectPath,"GameOfRPG","Data","GameData");
            Tiles = new TileData();
            Tiles.LoadData();

            Maps = new MapData();
            Maps.LoadData();

            Events = new EventData();
            Events.LoadData();
        }

    }
}

