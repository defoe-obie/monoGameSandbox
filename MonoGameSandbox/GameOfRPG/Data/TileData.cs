using System;
using System.IO;
using System.Collections.Generic;

//using Microsoft.Xna.Framework.Content;

namespace MonoGameSandbox
{ 
    public struct TileInfo
    {
        public int ID;
        public string FileName;
        public bool AutoTileFlag;
        public int Width;
        public int Height;
        public float[] DepthBox;
        public byte[] HitBox;
    }

    public class TileData
    {
       
        private List<TileInfo> Tiles;

        [Debug]
        public TileInfo this[int tileid]{
            get{
                int index = Math.Min(tileid, Tiles.Count-1);

                TileInfo result = Tiles[index];
                // Since the Tiles are sorted by ID, but not necessarily all
                // IDs from 0 to "tileid" are included... this:
                while (result.ID != tileid)
                {
                    index -= 1;
                    if(index < 0)
                    {
                        GameOfRPG.Debugger.AddError("Attempt to find Tile with ID:" + tileid + " has failed.");
                        break;
                    }
                    result = Tiles[index];
                }
                return result;
            }
        } 

        public TileData()
        {
            Tiles = new List<TileInfo>();
        }

        public void LoadData()
        {
            string filename = System.IO.Path.Combine(Data.DataPath, "tiles.rpgdata");
            using (StreamReader sr = new StreamReader(@filename))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    if (s == "begintile")
                    {
                        int id = Convert.ToInt32(sr.ReadLine());
                        string file = sr.ReadLine();

                        int width = 1;
                        int height = 1;
                        bool autotileflag = true;
                        s = sr.ReadLine();
                        if (s != "autotile")
                        {
                            autotileflag = false;
                            width = Convert.ToInt32(s);
                            height = Convert.ToInt32(sr.ReadLine());
                        }

                        float[] depthbox = new float[width * height];
                        s = sr.ReadLine();
                        string[] temp = s.Split(',');
                        for (int i = 0; i < temp.Length; ++i)
                        {
                            depthbox[i] = (float)Convert.ToDouble(temp[i]);
                        }

                        byte[] hitbox = new byte[width * height];
                        s = sr.ReadLine();
                        temp = s.Split(',');
                        for (int i = 0; i < temp.Length; ++i)
                        {
                            hitbox[i] = Convert.ToByte(temp[i]);
                        }

                        TileInfo t = new TileInfo(){ ID = id, FileName = file, Width = width, Height = height, AutoTileFlag = autotileflag, DepthBox = depthbox, HitBox = hitbox };
                        Tiles.Add(t);
                    }
                }
            }
            Tiles.Sort(delegate(TileInfo x, TileInfo y)
            {
                if (x.ID < y.ID)
                    return -1;
                if (x.ID > y.ID)
                    return 1;
                return 0;
            });
        }


    }

}

