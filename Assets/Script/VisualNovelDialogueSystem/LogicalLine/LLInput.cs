using System.Collections;

namespace Dialouge.LogicalLines
{
    public class LLInput : ILogicalLine
    {
        public string keyWord => "input";
        public IEnumerator Execute(DialougeLine line)
        {
            string title = line.dialougeData.rawData;
            InputPanel panel = InputPanel.instance;
            panel.Show(title);

            while(panel.isWaitingOnUserInput)
            {
                yield return null;
            }
        }

        public bool Matches(DialougeLine line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyWord);
        }
    }
}