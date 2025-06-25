using UnityEngine;
using TMPro;
using System.Collections;

namespace Dialouge
{
    public class AutoReader : MonoBehaviour
    {
        private const int defaultCharactersReadPerSecond = 10;
        private const float readTimePadding = 0.5f;
        private const float minReadTime = 1f;
        private const float maxReadTime = 99f;
        private const string statusTextAuto = "Auto";
        private const string statusTextSkip = "Skipping";

        private ConversationManager conversationManager;
        private TextArchitect architect => conversationManager.architect;

        public bool skip {  get; set; } = false;
        public float speed { get; set; } = 1f;

        private Coroutine co_running = null;
        public bool isOn => co_running != null;
        [SerializeField] private TextMeshProUGUI statusText;
        [HideInInspector] public bool allowToggle = true;
        public void Initialize(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;

            statusText.text = string.Empty;
        }

        public void Enable()
        {
            if (isOn)
            {
                return;
            }

            co_running = StartCoroutine(AutoRead());
        }

        public void Disable()
        {
            if (!isOn)
            {
                return;
            }

            StopCoroutine(co_running);
            skip = false;
            co_running = null;
            statusText.text = string.Empty;
        }

        private IEnumerator AutoRead()
        {
            if(!conversationManager.isRunning)
            {
                Disable();
                yield break;
            }

            if(!architect.isBuilding && architect.currentText != string.Empty)
            {
                DialougeSystem.instance.OnSystemPrompt_Next();
            }

            while (conversationManager.isRunning)
            {
                if (!skip)
                {
                    while (!architect.isBuilding && !conversationManager.isWaitingOnAutoTimer)
                    {
                        yield return null;
                    }

                    float timeStarted = Time.time;
                    while (architect.isBuilding || conversationManager.isWaitingOnAutoTimer)
                    {
                        yield return null;
                    }

                    float timeToRead = Mathf.Clamp(((float)architect.tmpro.textInfo.characterCount / defaultCharactersReadPerSecond), minReadTime, maxReadTime);
                    timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStarted)), minReadTime, maxReadTime);
                    timeToRead = (timeToRead / speed) + readTimePadding;

                    yield return new WaitForSeconds(timeToRead);
                }
                else
                {
                    architect.ForceComplete();
                    yield return new WaitForSeconds(0.05f);
                }

                DialougeSystem.instance.OnSystemPrompt_Next();
            }

            Disable();
        }
        public void Toggle_Auto()
        {
            if (!allowToggle)
            {
                return;
            }

            bool prevState = skip;
            skip = false;

            if (prevState)
            {
                Enable();
            }
            else
            {
                if(!isOn)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }

            if(isOn)
            {
                statusText.text = statusTextAuto;
            }           
        }

        public void Toggle_Skip()
        {
            if (!allowToggle)
            {
                return;
            }

            bool prevState = skip;
            skip = true;

            if (!prevState)
            {
                Enable();
            }
            else
            {
                if (!isOn)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }

            if (isOn)
            {
                statusText.text = statusTextSkip;
            }
        }
    }
}