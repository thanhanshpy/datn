using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using static Dialouge.LogicalLines.LogicalLineUtilities.Expressions;

namespace Dialouge.LogicalLines
{
    public class LLOperator : ILogicalLine
    {
        public string keyWord => throw new System.NotImplementedException();

        public IEnumerator Execute(DialougeLine line)
        {
            string strimmedLine = line.rawData.Trim();
            string[] parts = Regex.Split(strimmedLine, regexArithmatic);

            if(parts.Length < 3 )
            {
                Debug.LogError($"invalid command: {strimmedLine}");
                yield break;
            }

            string variable = parts[0].Trim().TrimStart(VariableStore.variableID);
            string op = parts[1].Trim();
            string[] remainingParts = new string[parts.Length - 2];
            Array.Copy(parts, 2, remainingParts, 0, parts.Length - 2);

            object value = CaculateValue(remainingParts);

            if(value == null)
            {
                yield break;
            }

            ProcessOperator(variable, op, value);
        }

        private void ProcessOperator(string variable, string op, object value)
        {
            if(VariableStore.TryGetValue(variable, out object currentValue))
            {
                ProcessOperatorOnVariable(variable, op, value, currentValue);
            }
            else if(op == "=")
            {
                VariableStore.CreateVariable(variable, value);
            }
        }
        public void ProcessOperatorOnVariable(string variable, string op, object value, object currentValue)
        {
            switch (op)
            {
                case "=":
                    VariableStore.TrySetValue(variable, value);
                    break;
                case "+=":
                    VariableStore.TrySetValue(variable, ConcatenateOrAdd(value, currentValue));
                    break;
                case "-=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                    break;
                case "*=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                    break;
                case "/=":
                    VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) / Convert.ToDouble(value));
                    break;
                default:
                    Debug.LogError($"invalid operator: {op}");
                    break;
            }
        }

        private object ConcatenateOrAdd(object value, object currentValue)
        {
            if(value is string)
            {
                return currentValue.ToString() + value;
            }

            return Convert.ToDouble(currentValue) + Convert.ToDouble(value);
        }
        public bool Matches(DialougeLine line)
        {
            Match match = Regex.Match(line.rawData.Trim(), regexOperatorLine);

            return match.Success;
        }
    }
}