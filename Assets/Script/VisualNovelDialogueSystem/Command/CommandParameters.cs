using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class CommandParameters 
    {
        private const char parameterIdentifier = '-';
        private Dictionary<string, string> parameters  = new Dictionary<string, string>();
        private List<string> unlabledParameters = new List<string>();

        public CommandParameters(string[] parameterArray, int startingIndex = 0)
        {
            for (int i = startingIndex; i < parameterArray.Length; i++)
            {
                if (parameterArray[i].StartsWith(parameterIdentifier) && !float.TryParse(parameterArray[i], out _))
                {
                    string pName = parameterArray[i];
                    string pValue = "";

                    if(i + 1 < parameterArray.Length && !parameterArray[i + 1].StartsWith(parameterIdentifier))
                    {
                        pValue = parameterArray[i + 1];
                        i++;
                    }

                    parameters.Add(pName, pValue);
                }
                else
                {
                    unlabledParameters.Add(parameterArray[i]);
                }
            }
        }

        public bool TryGetValue<T>(string parameterName, out T value, T defaultValue = default(T)) => TryGetValue(new string[] { parameterName }, out value, defaultValue);
        public bool TryGetValue<T>(string[] parameterNames, out T value, T defaultValue = default(T))
        {
            foreach(string parameterName in parameterNames)
            {
                if(parameters.TryGetValue(parameterName, out string parameterValue))
                {
                    if(TryCastParameter(parameterValue, out value)) return true;
                }
            }

            //search the unlabled match in the identified parameters if present
            foreach (string parameterName in unlabledParameters)
            {
                if (TryCastParameter(parameterName, out value))
                {
                    unlabledParameters.Remove(parameterName);
                    return true;
                }
            }

            value = defaultValue;
            return false;
        }

        private bool TryCastParameter<T>(string parameterValue, out T value)
        {
            if (typeof(T) == typeof(bool))
            {
                if(bool.TryParse(parameterValue, out bool boolValue))
                {
                    value = (T)(object)boolValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(parameterValue, out int boolValue))
                {
                    value = (T)(object)boolValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(parameterValue, out float boolValue))
                {
                    value = (T)(object)boolValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(string))
            {              
                value = (T)(object)parameterValue;
                return true;
            }

            value = default(T);
            return false;
        }
    }
}