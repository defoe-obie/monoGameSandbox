using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox
{
    public struct EventPage
    {
        public bool RepeatMove;
        public bool IgnoreImpossible;

        public TriggerType Trigger;
        public ArrayList Commands;
        public List<Requirement> Requirements;

        public int[] Movements;
    }


    public struct Requirement
    {
        private RequirementType rType;
        private int target;
        private int id;
        private string comparitor;

        public Requirement(RequirementType rt, int targetValue, int targetId, string comparitor)
        {
            rType = rt;
            target = targetValue;
            id = targetId;
            this.comparitor = comparitor;
        }

        public bool CheckRequirement()
        {
            switch (rType)
            {
                case(RequirementType.Variable):
                    int variableValue = GameOfRPG.Variables[id];
                    return Compare(target, variableValue);
                case(RequirementType.Switch):
                    int switchValue = Convert.ToInt32(GameOfRPG.Variables.GetSwitch(id));
                    return Compare(target, switchValue);
            }
            return true;
        }

        private bool Compare(int target, int value)
        {
            switch (comparitor)
            {
                case("<"):
                    return (target < value);
                case("<="):
                    return (target <= value);
                case("=="):
                    return (target == value);
                case(">="):
                    return (target >= value);
                case(">"):
                    return (target > value);
            }
            return true;
        }
    }

    public enum RequirementType
    {
        None,
        Variable,
        Switch
    }

    public enum TriggerType
    {
        None,
        Touch,
        Query,
        Automatic
    }

    public struct EventInfo
    {
        public int ID;
        public int MapID;
        public int StartX;
        public int StartY;
        public string ImageFileName;
        public bool MovingSpriteFlag;
        public int[] SpriteInfo;

        public List<EventPage> Pages;
    }

    public class EventData
    {
        // Events are sorted by MapID
        // Hopefully that will make things quicker on the load.
        private List<EventInfo> Events;

        public EventData()
        {
            Events = new List<EventInfo>();
        }

        public void LoadData()
        {
            string filename = System.IO.Path.Combine(Data.DataPath, "events.rpgdata");
            using (StreamReader sr = new StreamReader(@filename))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    // comment
                    if (s.StartsWith("//"))
                        continue;
                    if (s == "beginevent")
                    {
                        EventInfo ei = new EventInfo();
                        ei.ID = Convert.ToInt32(sr.ReadLine());
                        ei.MapID = Convert.ToInt32(sr.ReadLine());
                        ei.StartX = Convert.ToInt32(sr.ReadLine());
                        ei.StartY = Convert.ToInt32(sr.ReadLine());
                        ei.ImageFileName = sr.ReadLine();
                        s = sr.ReadLine();
                        if (s == "true")
                            ei.MovingSpriteFlag = true;
                        ei.SpriteInfo = new int[4];
                        for (int i = 0; i < 4; ++i)
                        {
                            ei.SpriteInfo[i] = Convert.ToInt32(sr.ReadLine());
                        }

                        s = sr.ReadLine();
                        ei.Pages = new List<EventPage>();
                        while (s != "endevent")
                        {
                            if (s == "beginpage")
                            {
                                EventPage ep = new EventPage();
                                ep.Commands = new ArrayList();
                                ep.Requirements = new List<Requirement>();
                                while (s != "endpage")
                                {
                                    s = sr.ReadLine();
                                    switch (s)
                                    {
                                        case("movepattern"):
                                            s = sr.ReadLine();
                                            if (s == "uselast")
                                            {
                                                ep.Movements = new int[]{ -1 };
                                                break;
                                            }
                                            if (s == "repeat")
                                            {
                                                ep.RepeatMove = true;
                                                s = sr.ReadLine();
                                            }
                                            if (s == "ignore")
                                            {
                                                ep.IgnoreImpossible = true;
                                                s = sr.ReadLine();
                                            }

                                            List<int> moves = new List<int>();
                                            while (s != "endpattern")
                                            {
                                                moves.AddRange(ConvertToMovePattern(s));
                                                s = sr.ReadLine();
                                            }
                                            ep.Movements = moves.ToArray();
                                            break;
                                        case("trigger"):
                                            s = sr.ReadLine();
                                            if (s == "none")
                                                ep.Trigger = TriggerType.None;
                                            else if (s == "touch")
                                                ep.Trigger = TriggerType.Touch;
                                            else if (s == "query")
                                                ep.Trigger = TriggerType.Query;
                                            else if (s == "automatic")
                                                ep.Trigger = TriggerType.Automatic;
                                            break;
                                        case("requirement"):
                                            ep.Requirements.Add(ConvertRequirement(sr.ReadLine().Split(',')));
                                            break;
                                        case("message"):
                                            ep.Commands.Add(1);
                                            ep.Commands.Add(sr.ReadLine());
                                            break;
                                        case("switch"): // id, [true,false,flip]
                                            ep.Commands.Add(2);
                                            string[] stuff = sr.ReadLine().Split(',');
                                            ep.Commands.Add(Convert.ToInt32(stuff[0]));
                                            ep.Commands.Add(stuff[1]);
                                            break;
                                        case("variable"): // id, operation, amount
                                            ep.Commands.Add(3);
                                            string[] stufg = sr.ReadLine().Split(',');
                                            ep.Commands.Add(stufg[0]);
                                            if (stufg[1] == "random")
                                            {
                                                int min = Convert.ToInt32(stufg[2]);
                                                int max = Convert.ToInt32(stufg[3]);
                                                ep.Commands.Add(GameOfRPG.randomGenerator.Next(max - min) + min);
                                            }
                                            else
                                            {
                                                ep.Commands.Add(Convert.ToInt32(stufg[1]));
                                            }
                                            break;


                                    }
                                }
                                ei.Pages.Add(ep);
                            }
                            s = sr.ReadLine();
                        }
                        Events.Add(ei);
                    }
                }
            }
            Events.Sort(delegate(EventInfo x, EventInfo y)
            {
                if (x.MapID < y.MapID)
                    return -1;
                if (x.MapID > y.MapID)
                    return 1;
                return 0;
            });
        }

        private Requirement ConvertRequirement(string[] commands)
        {
            int targetvalue = 0;
            int id = -1;
            string comparitor = "==";
            RequirementType rt = RequirementType.None;
            switch (commands[0])
            {
                case("switch"):
                    rt = RequirementType.Switch;
                    id = Convert.ToInt32(commands[1]);
                    if (commands[2] == "true")
                        targetvalue = 1;
                    break;
                case("variable"):
                    rt = RequirementType.Variable;
                    id = Convert.ToInt32(commands[1]);
                    targetvalue = Convert.ToInt32(commands[2]);
                    comparitor = commands[3];
                    break;
            }
            return new Requirement(rt, targetvalue, id, comparitor);
        }

        private List<int> ConvertToMovePattern(string s)
        {
            List<int> moves = new List<int>();
            string[] commands = s.Split(',');
            int currmove = 7; // Face Down, an innocuous move
            int amount = 0;
            switch (commands[0])
            {
                case("move"):
                case("face"):
                    if (commands[1] == "down")
                        currmove = 0;
                    else if (commands[1] == "right")
                        currmove = 1;
                    else if (commands[1] == "up")
                        currmove = 2;
                    else if (commands[1] == "left")
                        currmove = 3;
                    else if (commands[1] == "random")
                        currmove = 4;
                    else if (commands[1] == "towards")
                        currmove = 5;
                    else if (commands[1] == "away")
                        currmove = 6;
                    if (commands[0] == "face")
                        currmove += 7;
                    amount = Convert.ToInt32(commands[2]);

                    break;
                case("wait"):
                    currmove = 14;
                    amount = Convert.ToInt32(commands[1]);
                    break;
            }
            for (int i = 0; i < amount; ++i)
            {
                moves.Add(currmove);
                Console.WriteLine(currmove);
            }
            return moves;
        }

        public List<Event> GetEvents(ContentManager cm, int mapId)
        {
            List<Event> mapEvents = new List<Event>();
            int first = Events.FindIndex(delegate(EventInfo obj)
            {
                return (obj.MapID == mapId);
            });
            if (first != -1)
            {
                // the Events are sorted by MapId
                for (int i = first; i < Events.Count; ++i)
                {
                    EventInfo ei = Events[i];
                    if (ei.MapID != mapId)
                        break;
                    mapEvents.Add(new Event(cm, ei));
                }
            }

            return mapEvents;
        }
    }

    public class Event
    {
        public static EventInterpreter Interpreter = new EventInterpreter();

        private RPGBaseSprite sprite;
        private EventInfo info;
        private int currentPage;

        public bool Interpreting;
        //public bool MovingSprite;

        public RPGBaseSprite Sprite
        {
            get{ return sprite; }
        }


        public Event(ContentManager cm, EventInfo ei)
        {
            Interpreting = false;
            currentPage = 0;
            info = ei;
            if (ei.MovingSpriteFlag)
            {
                sprite = new RPGMovingSprite("", cm.Load<Texture2D>(info.ImageFileName), info.SpriteInfo[1]);
                ((RPGMovingSprite)sprite).SetMovementPattern(ei.Pages[0].RepeatMove, ei.Pages[0].Movements);
                // MovingSprite = true;
            }
            else
            {
                sprite = new RPGBaseSprite("", cm.Load<Texture2D>(info.ImageFileName), info.SpriteInfo[1], info.SpriteInfo[0]);
            }
            sprite.SetLocationByTile(info.StartX, info.StartY);


        }

        public void Update()
        {
            if (Interpreting && info.MovingSpriteFlag)
            {
                ((RPGMovingSprite)sprite).FacePlayerForInterpreter();
            }

            // TODO: Find a way to reduce the amount of checks in this case
            // Like, maybe only check requirements if a variable
            // or a switch has changed value?
            // But this reduces the amount of etc
            if (!Interpreting)
            {
                for (int i = (info.Pages.Count - 1); i >= 0; i--)
                {
                    bool requirementCheck = true;
                    foreach (Requirement r in info.Pages[i].Requirements)
                    {
                        requirementCheck = r.CheckRequirement() && requirementCheck;
                    }
                    if (requirementCheck)
                    {
                        if (currentPage != i)
                        {
                            currentPage = i;
                            if (info.MovingSpriteFlag)
                            {
                                if (info.Pages[currentPage].Movements[0] != -1)
                                {
                                    ((RPGMovingSprite)sprite).SetMovementPattern(info.Pages[currentPage].RepeatMove,
                                        info.Pages[currentPage].Movements);
                                }

                            }
                        }
                        break;
                    }
                }
            }

            sprite.Update();
        }

        public TriggerType GetTrigger()
        {
            return info.Pages[currentPage].Trigger;
        }

        public ArrayList GetCommands()
        {
            return info.Pages[currentPage].Commands;
        }

    }
}

