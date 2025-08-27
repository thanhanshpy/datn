using System.Collections;
using static Dialouge.LogicalLines.LogicalLineUtilities.Conditions;
using static Dialouge.LogicalLines.LogicalLineUtilities.Encapsulation;

namespace Dialouge.LogicalLines
{
    public class LLCondition : ILogicalLine
    {
        public string keyWord => "if";
        private readonly string[] containers = new string[] { "(", ")" };
        private const string ELSE = "else";

        public IEnumerator Execute(DialougeLine line)
        {
            string rawCondition = ExtractCondition(line.rawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            Conversation currentConversation = DialougeSystem.instance.conversationManager.conversation;
            int currentProgress = DialougeSystem.instance.conversationManager.conversationProgress;

            EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, false, parentStartingIndex: currentConversation.fileStartIndex);
            EncapsulatedData elseData = new EncapsulatedData();

            if (ifData.endingIndex + 1 < currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();
                if(nextLine == ELSE)
                {
                    elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, false, parentStartingIndex: currentConversation.fileStartIndex);
                    ifData.endingIndex = elseData.endingIndex;
                }
            }

            currentConversation.SetProgress(elseData.isNull ? ifData.endingIndex : elseData.endingIndex);

            EncapsulatedData selData = conditionResult ? ifData : elseData;
            if(!selData.isNull && selData.lines.Count > 0)
            {
                selData.startingIndex += 2; //remove header and starting encapsulator
                selData.endingIndex -= 1; //remove ending encapsulator

                Conversation newConversation = new Conversation(selData.lines, file: currentConversation.file, fileStartIndex: selData.startingIndex, fileEndIndex: selData.endingIndex);
                DialougeSystem.instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
        }

        public bool Matches(DialougeLine line)
        {
            return line.rawData.Trim().StartsWith(keyWord);
        }

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(containers[0]) + 1;
            int endIndex = line.IndexOf(containers[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}