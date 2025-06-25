using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

namespace Dialouge.LogicalLines
{
    public class LogicalLineManager 
    {
        private DialougeSystem dialougeSystem => DialougeSystem.instance;
        public List<ILogicalLine> logicalLines = new List<ILogicalLine>();

        public LogicalLineManager() => LoadLogicalLine();

        private void LoadLogicalLine()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] lineTypes = assembly.GetTypes().Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface).ToArray();

            foreach (Type lineType in lineTypes)
            {
                ILogicalLine line = (ILogicalLine)Activator.CreateInstance(lineType);
                logicalLines.Add(line);
            }
        }
        public bool TryGetLogic(DialougeLine line, out Coroutine logic)
        {
            foreach(var logicalLine in logicalLines)
            {
                if(logicalLine.Matches(line))
                {
                    logic = dialougeSystem.StartCoroutine(logicalLine.Execute(line));

                    return true;
                }
            }

            logic = null;
            return false;
        }
    }
}