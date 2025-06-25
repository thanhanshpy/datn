using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static Dialouge.LogicalLines.LogicalLineUtilities.Encapsulation;


namespace Dialouge.LogicalLines
{
    public class LLChoice : ILogicalLine
    {
        public string keyWord => "choice";
        
        private const char choiceIdentifier = '-';

        public IEnumerator Execute(DialougeLine line)
        {
            var currentConversation = DialougeSystem.instance.conversationManager.conversation;
            var progress = DialougeSystem.instance.conversationManager.conversationProgress;
            EncapsulatedData data = RipEncapsulationData(currentConversation, progress, ripHeaderAndEncapsulators: true, parentStartingIndex: currentConversation.fileStartIndex);

            List<Choice> choices = GetChoicesFromData(data);

            string title = line.dialougeData.rawData;
            ChoicePanel panel = ChoicePanel.instance;
            string[] choiceTitles = choices.Select(c => c.title).ToArray();

            panel.Show(title, choiceTitles);

            while(panel.isWaitingOnUserChoice)
            {
                yield return null;
            }

            Choice selectedChoice = choices[panel.lastDecision.answerIndex];

            Conversation newConversation = new Conversation(selectedChoice.resultLines, file: currentConversation.file, fileStartIndex: selectedChoice.startIndex, fileEndIndex: selectedChoice.endIndex);
            DialougeSystem.instance.conversationManager.conversation.SetProgress(data.endingIndex - currentConversation.fileStartIndex);
            DialougeSystem.instance.conversationManager.EnqueuePriority(newConversation);
        }

        private List<Choice> GetChoicesFromData(EncapsulatedData data)
        {
            List<Choice> choices = new List<Choice>();
            int encapsulationDepth = 0;
            bool isFirstChoice = true;

            Choice choice = new Choice
            {
                title = string.Empty,
                resultLines = new List<string>(),
            };

            int choiceIndex = 0, i = 0;
            for(i = 1; i < data.lines.Count; i++)
            //foreach(var line in data.lines.Skip(1))
            {
                //Debug.Log($"'{line}' at encap [{encapsulationDepth}] is choice ={IsChoiceStart(line)}");
                var line = data.lines[i];
                if(IsChoiceStart(line) && encapsulationDepth == 1)
                {
                    if (!isFirstChoice)
                    {
                        choice.startIndex = data.startingIndex+ (choiceIndex + 1);
                        choice.endIndex = data.startingIndex + (i - 1);
                        choices.Add(choice);
                        choice = new Choice
                        {
                            title = string.Empty,
                            resultLines = new List<string>(),
                        };
                    }

                    choiceIndex = i;
                    choice.title = line.Trim().Substring(1);
                    isFirstChoice = false;
                    continue;
                }

                AddLineToResults(line, ref choice, ref encapsulationDepth);
            }

            if (!choices.Contains(choice))
            {
                choice.startIndex = data.startingIndex + (choiceIndex + 1);
                choice.endIndex = data.startingIndex + (i - 2);
                choices.Add(choice);
            }

            return choices;
        }

        private void AddLineToResults(string line, ref Choice choice, ref int encapsulationDepth)
        {
            line.Trim();

            if (IsEncapsulationStart(line))
            {
                if (encapsulationDepth > 0)
                {
                    choice.resultLines.Add(line);
                }

                encapsulationDepth++;
                return;
            }

            if (IsEncapsulationEnd(line))
            {
                encapsulationDepth--;

                if (encapsulationDepth > 0)
                {
                    choice.resultLines.Add(line);
                }

                return;
            }

            choice.resultLines.Add(line);
        }          

        public bool Matches(DialougeLine line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyWord);
        }
       
        private bool IsChoiceStart(string line) => line.Trim().StartsWith(choiceIdentifier);
       
        private struct Choice
        {
            public string title;
            public List<string> resultLines;
            public int startIndex;
            public int endIndex;
        }
    }
}