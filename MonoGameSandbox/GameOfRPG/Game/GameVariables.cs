using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox
{

    public class GameVariables
    {
        private class GameVariable
        {
            public string Name;
            public int Data;

            public GameVariable(string name, int data)
            {
                Name = name;
                Data = data;
            }
        }


        public Matrix Scale;

       
        public bool MessageWaiting;
        public bool MessageShowing;
        public string MessageText;

        [Debug]
        public bool DebuggerWaiting;
        [Debug]
        public bool DebuggerShowing;

        private List<bool> gameSwitches;

        //omg i forgot how bad this was :(
        private Dictionary<int,int> eventVariables;
        private Dictionary<string, int> eventNames;

        [Debug]
        public int this [string variableName]
        {
            get
            {
                if (eventNames.ContainsKey(variableName))
                {
                    return this[eventNames[variableName]];
                }
                GameOfRPG.Debugger.AddError(String.Format("No variable with name \"{0}\" exists in the database, \nreturning value 0.", variableName));
                return 0;
            }
            set
            {
                if (eventNames.ContainsKey(variableName))
                {
                    this[eventNames[variableName]] = value;
                    return;
                }
                GameOfRPG.Debugger.AddError(String.Format("No variable with name \"{0}\" exists in the database, \nvariable not assigned.", variableName));

            }
        }


        [Debug]
        public int this [int variableIndex]
        {
            get
            {
                if (eventVariables.ContainsKey(variableIndex)){
                    return eventVariables[variableIndex];
                }
                GameOfRPG.Debugger.AddError(String.Format("Variables Key:{0:000} does not exist. Returning -1.", variableIndex));
                return -1;
            }
            set
            {
                if (eventVariables.ContainsKey(variableIndex))
                {
                    eventVariables[variableIndex] = value;
                }
                else
                {
                    GameOfRPG.Debugger.AddWarning(String.Format("Variable KEY:{0:000} does not exist yet. Creating a new, \nnameless variable.", variableIndex));
                    eventVariables.Add(variableIndex, value);
                   
                }
            }
        }

        public GameVariables()
        {
            Scale = Matrix.CreateScale(1, 1, 1);
            MessageWaiting = false;
            MessageShowing = false;
            MessageText = "";
            DebuggerWaiting = false;
            DebuggerShowing = false;

            gameSwitches = new List<bool>();
            eventVariables = new Dictionary<int, int>();
            eventNames = new Dictionary<string, int>();
        }

        public void SetScale(float viewportWidth, float viewportHeight)
        {
            float scaleX = viewportWidth / Constants.ScreenWidth;
            float scaleY = viewportHeight / Constants.ScreenHeight;
            float scaleAmount = Math.Min(scaleX, scaleY);
            Scale = Matrix.CreateScale(scaleAmount, scaleAmount, 1);
        }

        [Debug]
        public void Add(string variableName, int variableIndex, int value)
        {

            if (eventVariables.ContainsKey(variableIndex))
            {
                GameOfRPG.Debugger.AddError(String.Format("Variable KEY:{0:000} already exists, when attempting\n to add \"{1}\".", variableIndex, variableName));

            }
            else if (eventNames.ContainsKey(variableName))
            {
                GameOfRPG.Debugger.AddError(String.Format("A variable by name \"{0}\" alreay exists, associated\n with KEY:{1:000}!", variableName, eventNames[variableName]));
            }
            else
            {
                eventVariables.Add(variableIndex, value);
                eventNames.Add(variableName, variableIndex);
            }
            
        }

        public void SetSwitch(int id, bool switchvalue){
            while( gameSwitches.Count <= id){
                gameSwitches.Add(false);
                GameOfRPG.Debugger.AddWarning("In order to reach switch id " + id + " added unused switch at id " + (gameSwitches.Count-1));
            }
            gameSwitches[id] = switchvalue;
            Console.WriteLine("set switch id " + id + " to " + switchvalue + ", which has value " + Convert.ToInt32(switchvalue));
        }

        public void FlipSwitch(int id){
            SetSwitch(id, !GetSwitch(id));
        }

        public bool GetSwitch(int id){
            if ( id >= gameSwitches.Count){
                //GameOfRPG.Debugger.AddWarning("Accessing switch id " + id + ", which has not actually been set.\nOf course, this is easily an intentional situation. Returning false.");
                return false;
            }
            return gameSwitches[id];
        }

        [Debug]
        public List<string> GetDebugList()
        {
            List<string> returnList = new List<string>();
            foreach (int i in eventVariables.Keys)
            {
                string currstring = "";
                if (eventNames.ContainsValue(i))
                {
                    foreach (string s in eventNames.Keys){
                        if (eventNames[s] == i){
                            currstring = String.Format("KEY: {0:000}  Value: {1:00000}  Name: {2}", i, eventVariables[i], s);
                            break;
                        }
                    }
                        
                }
                else
                {
                    currstring = String.Format("KEY: {0:000}  Value: {1:00000}", i, eventVariables[i]);
                }
                returnList.Add(currstring);
            }
            return returnList;
        }

        [Debug]
        public List<string> GetSwitchList()
        {
            List<string> returnList = new List<string>();
            for (int i = 0; i < gameSwitches.Count; ++i)
            {
                string la =String.Format("SWITCH: {0:000}       " + gameSwitches[i], i);
                returnList.Add(la);
            }
            return returnList;
        }
    }
}

