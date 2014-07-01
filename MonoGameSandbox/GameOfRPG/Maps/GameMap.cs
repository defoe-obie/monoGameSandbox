using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

//TODO: Is it faster to use generic collections or none generic collections?
namespace MonoGameSandbox
{
    public class GameMap
    {
        private int width, height;
        private int tilesAcross, tilesDown;

        public int RealWidth{ get { return width; } }

        public int RealHeight{ get { return height; } }

        public string Name{ get; set; }

        private ProperTileSet tileSet;
        private int[][,] layers;
        private List<Light> lights;
        private List<Event> events;
        //private Light2 testlight;

     
        public GameMap(string mapName, int width, int height, ProperTileSet tileSet, int[][,] layers, List<Event> events)
        {
            //arbitrary numbers which will change in the future
            Name = mapName;
            tilesAcross = width;
            tilesDown = height;
            this.width = tilesAcross * Constants.TileSize;
            this.height = tilesDown * Constants.TileSize;
            this.tileSet = tileSet;
            this.layers = layers;
            this.events = events;
            //testlight = new Light2(5, 5, new Vector2(1, 0), (float)(Math.PI / 2), 1f, 3.5f, Color.White);
        }

        public void CheckTriggers(Vector2 location, params TriggerType [] triggers){
            //int tilex = (int)location.X / Constants.TileSize;
            //int tiley = (int)location.Y / Constants.TileSize;
            Event locationEvent = events.Find(delegate(Event obj)
            {
                return (obj.Sprite.Location == location);
            });
            if (locationEvent == null)
                return;
            foreach( TriggerType t in triggers){
                if (locationEvent.GetTrigger() == t && !locationEvent.Interpreting){
                    locationEvent.Interpreting = true;
                    Event.Interpreter.QueueEvent(locationEvent);
                }
            }
            Console.WriteLine(triggers[0]);
        }


        public bool CheckPassability(Vector2 previousLocation, Vector2 newLocation)
        {
            foreach (Event e in events)
            {
                if (!e.Sprite.Passible)
                {
                    Vector2 spriteLocation = e.Sprite.Location;
                    if (spriteLocation == newLocation)
                        return false;
                }
            }

            if (newLocation == GameOfRPG.Camera.PlayerLocation)
                return false;


            int oldx = (int)previousLocation.X / Constants.TileSize;
            int oldy = (int)previousLocation.Y / Constants.TileSize;
            int newx = (int)newLocation.X / Constants.TileSize;
            int newy = (int)newLocation.Y / Constants.TileSize;

            // up -> 1, left -> 2, right -> 4, down -> 8
            int direction = (newx < oldx) ? 2 : (newx > oldx) ? 4 : 0;
            direction = (newy < oldy) ? 1 : (newy > oldy) ? 8 : direction;
            byte currentPassability = 0;
            byte newPassability = 0;
            for (int i = 0; i < 5; ++i)
            {
                currentPassability |= tileSet.GetTileHit(layers[i][oldx, oldy]);
                newPassability |= tileSet.GetTileHit(layers[i][newx, newy]);
            }
            if (newPassability == 15)
                return false;
            if ((currentPassability & direction) == 0)
                return true;
            if (direction == 2)
                direction = 4;
            else if (direction == 4)
                direction = 2;
            else if (direction == 1)
                direction = 8;
            else if (direction == 8)
                direction = 1;
            if ((newPassability & direction) == 0)
                return true;
            return false;
        }

        public void SetUpLights(GraphicsDeviceManager graphics)
        {
//            Light l1 = new Light(graphics, 3, 3, new Vector2(0, 1), (float)Math.PI / 4, 1f, 3, Color.AntiqueWhite);
//            Light l4 = new Light(graphics, 4, 3, new Vector2(0, 1), (float)Math.PI / 4, 0.75f, 3, Color.AntiqueWhite);
//            Light l2 = new Light(graphics, 5, 3, new Vector2(0, 1), (float)Math.PI / 4, 0.75f, 3, Color.White);
//            Light l5 = new Light(graphics, 6, 3, new Vector2(0, 1), (float)Math.PI / 4, 0.75f, 3, Color.White);
//            Light l3 = new Light(graphics, 7, 3, new Vector2(0, 1), (float)Math.PI / 4, 0.5f, 3, Color.White);
            lights = new List<Light>();
//            lights.Add(l1);
//            lights.Add(l2);
//            lights.Add(l3);
//            lights.Add(l4);
//            lights.Add(l5);

        }

        public void Update()
        {
            foreach (Event e in events)
            {
                e.Update();
            }
        }

        public void Draw(SpriteBatch sb, RPGPlayerSprite player)
        {

            sb.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, GameOfRPG.Variables.Scale);
            Point startPoint = GameOfRPG.Camera.GetFirstVisibleTileLocation();
            Vector2 startLocation = new Vector2(startPoint.X * Constants.TileSize, startPoint.Y * Constants.TileSize);
            for (int y = startPoint.Y; y <= startPoint.Y + 2 + Constants.ScreenTilesDown; ++y)
            {
                if (y >= tilesDown)
                    break;
                for (int x = startPoint.X; x <= startPoint.X + 2 + Constants.ScreenTilesAcross; ++x)
                {
                    if (x >= tilesAcross)
                        break;
                    float depthoffset = 0f;
                    foreach (int[,] layer in layers)
                    {
                        tileSet.DrawTile(sb, GameOfRPG.Camera.GetScreenLocation(startLocation), layer[x, y], depthoffset);
                        depthoffset += 0.05f;
                    }
                    startLocation.X += Constants.TileSize;
                }
                startLocation.X = startPoint.X * Constants.TileSize;
                startLocation.Y += Constants.TileSize;
            }
            player.Draw(sb);
            foreach (Event e in events)
            {
                e.Sprite.Draw(sb);
            }
            foreach (Light l in lights)
            {
                l.Draw(sb);
            }
            sb.End();
        }
    }
}

