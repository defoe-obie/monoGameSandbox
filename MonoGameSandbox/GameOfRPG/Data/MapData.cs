using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGameSandbox
{
    public class MapData
    {
        public struct MapInfo
        {
            public int ID;
            public string MapName;
            public string FileName;
        }

        private List<MapInfo> Maps;

        public MapInfo this [int mapId]
        {
            get
            {
                int index = Math.Min(mapId, Maps.Count - 1);

                MapInfo mi = Maps[index];
                while (mi.ID != mapId)
                {
                    index -= 1;
                    if (index < 0)
                    {
                        throw new ArgumentOutOfRangeException("Map width ID " + mapId + " does not exist! Critical Exception!");
                    }
                    mi = Maps[index];
                }
                return mi;
            }
        }

        public MapData()
        {
            Maps = new List<MapInfo>();
        }

        public void LoadData()
        {
            string filename = System.IO.Path.Combine(Data.DataPath, "maps.rpgdata");
            using (StreamReader sr = new StreamReader(@filename))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    if (s == "beginmap")
                    {
                        int id = Convert.ToInt32(sr.ReadLine());
                        string file = sr.ReadLine();
                        string mapname = sr.ReadLine();
                        MapInfo m = new MapInfo(){ ID = id, MapName = mapname, FileName = file };
                        Maps.Add(m);
                    }
                }
            }
            Maps.Sort(delegate(MapInfo x, MapInfo y)
            {
                if (x.ID < y.ID)
                    return -1;
                if (x.ID > y.ID)
                    return 1;
                return 0;
            });
        }

        public GameMap LoadMap(ContentManager cm, int mapId)
        {
          
            MapInfo mi = this[mapId];
            string filename = System.IO.Path.Combine(Data.DataPath, mi.FileName + ".mapdata");
            using (StreamReader sr = new StreamReader(@filename))
            {
                int mapwidth = Convert.ToInt32(sr.ReadLine());
                int mapheight = Convert.ToInt32(sr.ReadLine());
                string s = sr.ReadLine();
                if (s == "tiles")
                {
                    ProperTileSet pts = new ProperTileSet();
                    List<Point> tiles = new List<Point>();

                    s = sr.ReadLine();
                    while (s != "endtiles")
                    {
                        string[] temp = s.Split(',');
                        int tileid = Convert.ToInt32(temp[0]);
                        int subtileid = Convert.ToInt32(temp[1]);
                        tiles.Add(new Point(tileid, subtileid));
                        s = sr.ReadLine();
                    }
                    pts.AddTiles(cm, tiles);

                    s = sr.ReadLine();
                    if (s == "layers")
                    {
                        int[][,] layers = new int[5][,];
                        for (int i = 0; i < 5; ++i)
                        {
                            layers[i] = new int[mapwidth, mapheight];
                            for (int y = 0; y < mapheight; ++y)
                            {
                                s = sr.ReadLine();
                                string[] tilesacross = s.Split(',');
                                for (int x = 0; x < mapwidth; ++x)
                                {
                                    layers[i][x, y] = Convert.ToInt32(tilesacross[x]);
                                }
                            }
                        }
                        List<Event> mapEvents = Data.Events.GetEvents(cm, mapId);
                       
                        GameMap gm = new GameMap(mi.MapName, mapwidth, mapheight, pts, layers, mapEvents);
                        return gm;
                    }
                    else
                    {
                        throw new InvalidDataException(mi.FileName + ".mapdata is in improper format at \"layers\" line.");
                    }
                }
                else
                {
                    throw new InvalidDataException(mi.FileName + ".mapdata is in improper format, at \"tiles\" line.");
                }

            }

        }
    }
}
