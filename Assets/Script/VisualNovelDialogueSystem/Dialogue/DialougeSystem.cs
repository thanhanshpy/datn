using UnityEngine;
using System.Collections.Generic;
using Characters;

namespace Dialouge
{
    public class DialougeSystem : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainCanvas;
        [SerializeField] private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO config => _config;

        public DialougeContainer dialougeContainer = new DialougeContainer();
        public ConversationManager conversationManager {  get; private set; }
        private TextArchitect architect;
        private AutoReader autoReader;
        public static DialougeSystem instance { get; private set; }
        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;
        public event DialogueSystemEvent onClear;
        public bool isRunningConversation => conversationManager.isRunning;
        public DialogueContinuePrompt prompt;
        private CanvasGroupController cgController;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
                //DontDestroyOnLoad(gameObject);
            }
                
            else
                DestroyImmediate(gameObject);
        }

        bool _initialize = false;
        private void Initialize()
        {
            if (_initialize) return;

            architect = new TextArchitect(dialougeContainer.dialogueText, TABuilder.BuilderTypes.Typewriter);
            conversationManager = new ConversationManager(architect);

            cgController = new CanvasGroupController(this, mainCanvas);
            dialougeContainer.Initialized();

            autoReader = GetComponent<AutoReader>();
            if (TryGetComponent(out autoReader))
            {
                autoReader.Initialize(conversationManager);
            }
        }

        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();

            if(autoReader != null && autoReader.isOn)
            {
                autoReader.Disable();
            } 
        }
        public void OnSystemPrompt_Clear()
        {
            onClear?.Invoke();
        }
        public void OnSystemPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }
        public void OnStartViewingHistory()
        {
            prompt.Hide();
            autoReader.allowToggle = false;
            conversationManager.allowUserPrompts = false;
            if (autoReader.isOn)
            {
                autoReader.Disable();
            }
        }

        public void OnStopViewingHistory()
        {
            prompt.Show();
            autoReader.allowToggle = true;
            conversationManager.allowUserPrompts = true;
        }
        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName); 
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            ApplySpeakerDataToDialogueContainer(config);
        }
        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            dialougeContainer.SetDialogueColor(config.dialogueColor);
            dialougeContainer.SetDialogueFont(config.dialogueFont);
            dialougeContainer.nameContainer.SetNameColor(config.nameColor);
            dialougeContainer.nameContainer.SetNameFont(config.nameFont);
        }
        public void ShowSpeakerName(string speakerName = "")
        {
            //if (speakerName.ToLower() != "ya")
            if (!string.IsNullOrEmpty(speakerName) || speakerName.ToLower() != "narrator")
            {
                dialougeContainer.nameContainer.Show(speakerName);
            }
            else
            {
                HideSpeakerName();
                dialougeContainer.nameContainer.nameText.text = "";
            }
        }
        public void HideSpeakerName() => dialougeContainer.nameContainer.Hide();


        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \" {dialogue} \"" };
            return Say(conversation);
        }

        public Coroutine Say(List<string> lines, string filePath = "")
        {
            Conversation conversation = new Conversation(lines, file: filePath);
            return conversationManager.StartConversation(conversation);
        }

        public Coroutine Say(Conversation conversation)
        {
            return conversationManager.StartConversation(conversation);
        }
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
       
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);

        public bool isVisible => cgController.isVisible;
       
    }
}

