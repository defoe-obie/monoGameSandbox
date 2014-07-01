using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGameSandbox
{
    public class EventInterpreter
    {
        private bool waiting;
        private int currentCommand;

        private Event currentEvent;
        private Queue waitingEvents;

        public bool Busy;

        public EventInterpreter()
        {
            waiting = false;
            Busy = false;
            currentCommand = 0;
            waitingEvents = new Queue();
        }

        public void QueueEvent(Event eventToQueue)
        {
            waitingEvents.Enqueue(eventToQueue);
            Busy = true;
        }


        //Busy is my lock on the interpreter, I guess.

        public void UpdateInterpreter()
        {
            // no events needing to be interpreted!
            if (waitingEvents.Count <= 0)
            {
                Busy = false;
                return;
            }
            // waiting means that the interpreter holds off
            // running until receiving input
            waiting = false;
            currentEvent = (Event)waitingEvents.Peek();
            ArrayList commands = currentEvent.GetCommands();
           
            Console.WriteLine("Interpreter begin");
            Console.WriteLine("Interpreting commands number " + currentCommand);
            try
            {
                while (currentCommand < commands.Count)
                {
                    int c = (int)commands[currentCommand];
                    switch (c)
                    {
                        case(1): // Message
                            currentCommand += 1;
                            DoMessage((string)commands[currentCommand]);
                            break;
                        case(2): // Switch
                            DoSwitch((int)commands[currentCommand+1], (string)commands[currentCommand+2]);
                            currentCommand += 2;
                            break;
                        case(3):
                            DoVariable((int)commands[currentCommand+1], (string)commands[currentCommand+2], (int)commands[currentCommand+3]);
                            currentCommand += 3;
                            break;
                        default:
                            GameOfRPG.Debugger.AddWarning("Oh, Umm... A command labelled " + c + " does not exits.");
                            break;
                    }
                    currentCommand += 1;
                    if (waiting){ // waiting for user input
                        Console.WriteLine("Interpreter waiting for input.");
                        return;
                    }
                } 
                if (currentCommand >= commands.Count)
                {
                    waitingEvents.Dequeue();
                    currentEvent.Interpreting = false;
                    currentCommand = 0;
                    //  return;
                }
            }
            catch (InvalidCastException ice)
            {
                GameOfRPG.Debugger.AddError("Oh God! Something in the event interpreter failed! Invalid Cast Exception!");
            }

            Console.WriteLine("End Interpret");

        }

        #region --1:DoMessage
        // Command 1
        private void DoMessage(string messagetext)
        {
            waiting = true;
            GameOfRPG.Variables.MessageText = messagetext;
            GameOfRPG.Message.PrepMessage();
        }
        #endregion

        #region --2:DoSwitch
        // Command 2
        void DoSwitch(int switchId, string switchcommand)
        {
            switch(switchcommand){
                case("flip"):
                    GameOfRPG.Variables.FlipSwitch(switchId);
                    break;
                case("true"):
                    GameOfRPG.Variables.SetSwitch(switchId, true);
                    break;
                case("false"):
                    GameOfRPG.Variables.SetSwitch(switchId, false);
                    break;
            }
        }
        #endregion
    
        #region --3:DoVariable
        // Command 3
        void DoVariable(int variableId, string command, int amount)
        {
            int currentvalue = GameOfRPG.Variables[variableId];
            switch(command){
                case("+"):
                    currentvalue += amount;
                    break;
                case("-"):
                    currentvalue -= amount;
                    break;
                case("/"):
                    currentvalue /= amount;
                    break;
                case("*"):
                    currentvalue *= amount;
                    break;
                case("%"):
                    currentvalue %= amount;
                    break;
                case("set"):
                    currentvalue = amount;
                    break;
            }
            GameOfRPG.Variables[variableId] = currentvalue;
        }
        #endregion
    }
}

