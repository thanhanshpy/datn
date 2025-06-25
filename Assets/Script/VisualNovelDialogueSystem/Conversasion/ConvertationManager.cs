using Characters;
using Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialouge.LogicalLines;
namespace Dialouge
{
    public class ConversationManager 
    {
        public bool allowUserPrompts = true;
        public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top);
        public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress());

        private ConversationQueue conversationQueue;

        private LogicalLineManager logicalLineManager;
        private DialougeSystem dialougeSystem => DialougeSystem.instance;

        private Coroutine process = null;
        public bool isRunning => process != null;

        public TextArchitect architect = null;
        private bool userPrompt = false;
        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialougeSystem.onUserPrompt_Next += OnUserPrompt_Next;

            logicalLineManager = new LogicalLineManager();

            conversationQueue = new ConversationQueue();
        }

        public Conversation[] GetConversationQueue() => conversationQueue.GetReadOnly();
        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);

        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        private void OnUserPrompt_Next()
        {
            if(allowUserPrompts)
            {
                userPrompt = true;
            }
        }

        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();
            Enqueue(conversation);
            process = dialougeSystem.StartCoroutine(RunningConversation());
            return process;
        }

        public void StopConversation()
        {
            if(!isRunning)
            {
                return;
            }
            dialougeSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation()
        {
            while(!conversationQueue.IsEmpty())
            {
                Conversation currentConversation = conversation;

                if (currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = currentConversation.CurrentLine();

                //no black line
                if (string.IsNullOrWhiteSpace(rawLine)) 
                {
                    TryAdvanceConversation(currentConversation);
                    continue; 
                }

                DialougeLine line = DialougeParser.Parse(rawLine);

                if (logicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    yield return logic;
                }
                else
                {
                    //show dialogue
                    if (line.hasDialogue)
                    {
                        yield return Line_RunDialogue(line);
                    }

                    //run any commands
                    if (line.hasCommands)
                    {
                        yield return Line_RunCommands(line);
                    }
                    //wait for user input
                    if (line.hasDialogue)
                    {
                        yield return WaitForUserInput();

                        CommandManager.instance.StopAllProcess();

                        dialougeSystem.OnSystemPrompt_Clear();
                    }
                }

                TryAdvanceConversation(currentConversation);
            }

            process = null;
        }

        private void TryAdvanceConversation(Conversation conversation)
        {
            conversation.IncrementProgress();

            if(conversation != conversationQueue.top)
            {
                return;
            }

            if (conversation.HasReachedEnd())
            {
                conversationQueue.Dequeue();
            }
        }
        IEnumerator Line_RunDialogue(DialougeLine line)
        {
            //show present speaker's name
            if (line.hasSpeaker)
            {
                HandleSpeakerLogic(line.speakerData);
            }

            if (!dialougeSystem.dialougeContainer.isVisible)
            {
                dialougeSystem.dialougeContainer.Show();
            }

            yield return BuildLineSegment(line.dialougeData);

        }
        public void HandleSpeakerLogic(SpeakerData speakerData)
        {
            bool characterMustBeCreated = (speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpression);

            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: characterMustBeCreated);

            if (speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing))
            {
                character.Show();
            }

            dialougeSystem.ShowSpeakerName(TagManager.Inject(speakerData.displayName));

            DialougeSystem.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

            if (speakerData.isCastingPosition)
            {
                character.MoveToPosition(speakerData.castPosition);
            }

            if(speakerData.isCastingExpression)
            {
                foreach(var ce in speakerData.CastExpression)
                {
                    character.OnReciveCastingExpresstion(ce.layer, ce.expression);
                }
            }
        }
        IEnumerator Line_RunCommands(DialougeLine line)
        {
            List<CommandData.Command> commands = line.commandsData.commands;

            foreach(CommandData.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while (!cw.isDone)
                    {
                        if (userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                {
                    CommandManager.instance.Execute(command.name, command.arguments);
                }
            }

            yield return null;
        }

        IEnumerator BuildLineSegment(DialogueData line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DialogueData.DialogueSegment segment = line.segments[i];

                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }
        public bool isWaitingOnAutoTimer { get; private set; } = false;
        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DialogueData.DialogueSegment segment)
        {
            switch (segment.startSignal)
            {
                case DialogueData.DialogueSegment.StartSignal.C:
                    yield return WaitForUserInput();
                    dialougeSystem.OnSystemPrompt_Clear();
                    break;
                case DialogueData.DialogueSegment.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DialogueData.DialogueSegment.StartSignal.WC:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    dialougeSystem.OnSystemPrompt_Clear();
                    break;
                case DialogueData.DialogueSegment.StartSignal.WA:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;
            }
        }
        IEnumerator BuildDialogue(string dialouge, bool append = false)
        {
            dialouge =TagManager.Inject(dialouge);

            //build dialogue
            if (!append)
            {
                architect.Build(dialouge);
            }
            else
            {
                architect.Append(dialouge);
            }
            
            //wait dialogue complete
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                    {
                        architect.hurryUp = true;
                    }
                    else
                    {
                        architect.ForceComplete();
                    }

                    userPrompt = false;
                }
                yield return null;
            }
        }
        IEnumerator WaitForUserInput()
        {
            dialougeSystem.prompt.Show();

            while(!userPrompt)
                yield return null;

            dialougeSystem.prompt.Hide();

            userPrompt = false;
        }
    }
}

