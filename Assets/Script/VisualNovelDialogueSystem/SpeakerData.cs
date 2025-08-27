using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Dialouge
{
    public class SpeakerData
    {
        public string rawData { get; private set; } = string.Empty;

        //show who is speaking in dialogue box
        public string displayName => !string.IsNullOrEmpty(castName) ? castName : name;

        public string name, castName;
        public Vector2 castPosition;
        public List<(int layer, string expression)> CastExpression { get; set; }
        public bool isCastingName =>castName != string.Empty;
        public bool isCastingPosition = false;
        public bool isCastingExpression => CastExpression.Count > 0;

        public bool makeCharacterEnter = false;
        private const string nameCastID = " as ";
        private const string positionCastID = " at ";
        private const string expressionCastID = " [";
        private const char axisDelimiterID = ':';
        private const char expressionLayerJoiner = ',';
        private const char expressionLayerDelimiter = ':';
        private const string enterKeyword = "enter ";
        private string ProcessKeywords(string rawSpeaker)
        {
            if (rawSpeaker.StartsWith(enterKeyword))
            {
                rawSpeaker = rawSpeaker.Substring(enterKeyword.Length);
                makeCharacterEnter = true;
            }

            return rawSpeaker;
        }

        public SpeakerData(string rawSpeaker)
        {
            rawData = rawSpeaker;
            rawSpeaker = ProcessKeywords(rawSpeaker);

            string pattern = @$"{nameCastID}|{positionCastID}|{expressionCastID.Insert(expressionCastID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

            //this will help avoid null references
            castName = "";
            castPosition = Vector2.zero;
            CastExpression = new List<(int layer, string expression)>();

            //no matches -> entire line is the speaker name
            if (matches.Count == 0)
            {
                name = rawSpeaker;
                return;
            }

            //or isolate the speaker name from casting data
            int index = matches[0].Index;
            name = rawSpeaker.Substring(0, index);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0, endIndex = 0;

                if (match.Value == nameCastID)
                {
                    startIndex = match.Index + nameCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }
                else if (match.Value == positionCastID)
                {
                    isCastingPosition = true;

                    startIndex = match.Index + positionCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPo = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPo.Split(axisDelimiterID, System.StringSplitOptions.RemoveEmptyEntries);

                    float.TryParse(axis[0], out castPosition.x);

                    if (axis.Length > 1)
                    {
                        float.TryParse(axis[1], out castPosition.y);
                    }
                }
                else if (match.Value == expressionCastID)
                {
                    startIndex = match.Index + expressionCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    CastExpression = castExp.Split(expressionLayerJoiner).Select(x =>
                    {
                        var parts = x.Trim().Split(expressionLayerDelimiter);
                        if (parts.Length == 2)
                        {
                            return (int.Parse(parts[0]), parts[1]);
                        }
                        else
                        {
                            return (0, parts[0]);
                        }
                    }).ToList();
                }
            }
        }
    }
}