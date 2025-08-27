
namespace Dialouge
{
    public class DialougeLine 
    {
        public string rawData {  get; private set; } = string.Empty;
        public SpeakerData speakerData;
        public DialogueData dialougeData;
        public CommandData commandsData;

        public bool hasSpeaker => speakerData != null;
        public bool hasDialogue => dialougeData != null;
        public bool hasCommands => commandsData != null;

        public DialougeLine( string rawLine, string speaker, string dialouge, string commands)
        {
            rawData = rawLine;
            this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new SpeakerData(speaker));
            this.dialougeData = (string.IsNullOrWhiteSpace(dialouge) ? null : new DialogueData(dialouge));
            this.commandsData = (string.IsNullOrWhiteSpace(commands) ? null : new CommandData(commands));
        }
    }
}

