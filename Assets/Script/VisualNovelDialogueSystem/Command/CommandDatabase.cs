using System.Collections.Generic;
using UnityEngine;
using System;

namespace Commands
{
    public class CommandDatabase
    {
        private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();

        public bool HasCommand(string commandName) => database.ContainsKey(commandName.ToLower());

        public void AddCommand(string commandName, Delegate command)
        {
            commandName = commandName.ToLower();

            if (!database.ContainsKey(commandName))
            {
                database.Add(commandName, command);
            }
            else
            {
                Debug.LogError($"commnand already esixts '{commandName}'");
            }
        }
        public Delegate GetCommand(string commandName)
        {
            commandName = commandName.ToLower();

            if (!database.ContainsKey(commandName))
            {
                Debug.LogError($"Command: '{commandName}' doesnt esixts");
                return null;
            }
            return database[commandName];
        }
    }
}