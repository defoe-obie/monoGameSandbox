using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace MonoGameSandbox
{
	public class ProperTileSet
	{

        private struct Tile{
            public int TextureId;
            public byte hit;
            public float depth;
            public Point[] TileLocations;
            public bool AutoTileFlag;
        }

        private List<Tile> tiles;
        private List<Texture2D> textures;

        private static Point[][] locations;
        private static int ht = Constants.TileSize / 2; //halftile

        static ProperTileSet(){
            locations = new Point[5][];
            locations[0] = new Point[4];
            locations[1] = new Point[4];
            locations[2] = new Point[4];
            locations[3] = new Point[4];
            locations[4] = new Point[4];
            //corners
            locations[0][0] = new Point(0, 0);
            locations[0][1] = new Point(3 * ht, 0);
            locations[0][2] = new Point(0, 3 * ht);
            locations[0][3] = new Point(3 * ht, 3 * ht);
            //sides
            locations[1][0] = new Point(0, 2 * ht);
            locations[1][1] = new Point(3 * ht, 2 * ht);
            locations[1][2] = new Point(0, ht);
            locations[1][3] = new Point(3 * ht, ht);
            //bottom and top
            locations[2][0] = new Point(2 * ht, 0);
            locations[2][1] = new Point(ht, 0);
            locations[2][2] = new Point(2 * ht, 3 * ht);
            locations[2][3] = new Point(ht, 3 * ht);
            //inner corners
            locations[3][0] = new Point(4 * ht, 2 * ht);
            locations[3][1] = new Point(5 * ht, 2 * ht);
            locations[3][2] = new Point(4 * ht, 3 * ht);
            locations[3][3] = new Point(5 * ht, 3 * ht);
            //insides
            locations[4][0] = new Point(ht, 2 * ht);
            locations[4][1] = new Point(2 * ht, 2 * ht);
            locations[4][2] = new Point(ht, ht);
            locations[4][3] = new Point(2 * ht, ht);
        }

        public ProperTileSet(){
            tiles = new List<Tile>();
            textures = new List<Texture2D>();
        }

        public void AddTiles(ContentManager cm, List<Point> tilesInSet){
            List<int> includedIds = new List<int>();
            foreach(Point p in tilesInSet){

                TileInfo ti = Data.Tiles[p.X];
                Tile newTile;
                int textureid = includedIds.IndexOf(p.X);
                if(textureid == -1)
                {
                    includedIds.Add(p.X);
                    textures.Add(cm.Load<Texture2D>(System.IO.Path.Combine("Tiles", ti.FileName)));
                    textureid = textures.Count - 1;
                }
                if (ti.AutoTileFlag){
                    newTile = new Tile(){ TextureId = textureid, hit = ti.HitBox[0], depth = ti.DepthBox[0], TileLocations = GetAutoTileSourceLocations(p.Y), AutoTileFlag = true};
                }
                else{
                    int across = textures[textureid].Width / Constants.TileSize;
                    int subtileX = p.Y % across;
                    int subtileY = p.Y / across;
                    int hitdepthindex = (subtileX % ti.Width) + (subtileY % ti.Height) * ti.Width;
                    Point sourcelocation = new Point(subtileX * Constants.TileSize, subtileY * Constants.TileSize);
                    newTile = new Tile(){ TextureId = textureid, hit = ti.HitBox[hitdepthindex], depth = ti.DepthBox[hitdepthindex], TileLocations = new Point[]{sourcelocation}, AutoTileFlag = false};

                }
                tiles.Add(newTile);
            }

        }

        private Point[] GetAutoTileSourceLocations(int typeid){
            int[] result = new int[]{ 1, 1, 1, 1 };
            if ((typeid & 1) == 1){
                result[0] |= 2;
                result[1] |= 2;
            }
            if ((typeid & 2) == 2){
                result[0] |= 4;
                result[2] |= 4;
            }
            if ((typeid & 4) == 4){
                result[1] |= 4;
                result[3] |= 4;
            }
            if ((typeid & 8) == 8){
                result[2] |= 2;
                result[3] |= 2;
            }
            if ((typeid & 19) == 19){
                result[0] |= 8;
            }
            if ((typeid & 37) == 37){
                result[1] |= 8;
            }
            if ((typeid & 74) == 74){
                result[2] |= 8;
            }
            if ((typeid & 140) == 140){
                result[3] |= 8;
            }
            Point[] finalLocations = new Point[4];
            for (int i = 0; i < 4; ++i){
                if (result[i] == 1)
                    finalLocations[i] = locations[0][i];
                else if (result[i] == 3)
                    finalLocations[i] = locations[1][i];
                else if (result[i] == 5)
                    finalLocations[i] = locations[2][i];
                else if (result[i] == 7)
                    finalLocations[i] = locations[3][i];
                else if (result[i] == 15)
                    finalLocations[i] = locations[4][i];
            }
            return finalLocations;
        }

        public byte GetTileHit(int mapTileId){
            if (mapTileId == -1)
                return 0;
            return tiles[mapTileId].hit;
        }

        public void DrawTile(SpriteBatch sb, Vector2 location, int mapTileId, float depthoffset){
            if (mapTileId == -1)
                return;
            Tile currentTile = tiles[mapTileId];
            if (currentTile.AutoTileFlag){
                for (int i = 0; i < currentTile.TileLocations.Length; ++i){
                    Rectangle sourceRectangle = new Rectangle(currentTile.TileLocations[i].X, currentTile.TileLocations[i].Y, ht, ht);
                    Vector2 newLocation = new Vector2(location.X + (i % 2) * ht, location.Y + (i / 2) * ht);
                    sb.Draw(textures[currentTile.TextureId], newLocation, null, sourceRectangle, null, 0f, null, Color.White, SpriteEffects.None, currentTile.depth - depthoffset);
                }
            }
            else{
                Rectangle sourceRectangle = new Rectangle(currentTile.TileLocations[0].X, currentTile.TileLocations[0].Y, Constants.TileSize, Constants.TileSize);
                sb.Draw(textures[currentTile.TextureId], location,null, sourceRectangle, null, 0f, null, Color.White, SpriteEffects.None, currentTile.depth-depthoffset);
            }

        }
	}

}

