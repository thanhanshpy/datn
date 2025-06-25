using Dialouge;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace History
{
    
    public class HistoryNavigation : MonoBehaviour
    {
        public int progress = 0;
        [SerializeField] private TextMeshProUGUI statusText;
        HistoryManager manager => HistoryManager.instance;
        List<HistoryState> history => manager.history;
        HistoryState cachedState = null;
        private bool isOnCachedState = false;
        public bool isViewingHistory = false;
        public void GoFoward()
        {
            if (!isViewingHistory)
            {
                return;
            }

            HistoryState state = null;

            if (progress < history.Count - 1)
            {
                progress++;
                state = history[progress];
            }
            else
            {
                isOnCachedState = true;
                state = cachedState;
            }

            state.Load();

            if (isOnCachedState)
            {
                isViewingHistory = false;
                DialougeSystem.instance.onUserPrompt_Next -= GoFoward;
                statusText.text = "";
                DialougeSystem.instance.OnStopViewingHistory();
            }
            else
            {
                UpdateStatusText();
            }

        }

        public void GoBack()
        {
            if(history.Count == 0 || progress == 0 && isViewingHistory)
            {
                return;
            }

            progress = isViewingHistory ? progress - 1 : history.Count - 1;

            if (!isViewingHistory)
            {
                isViewingHistory = true;
                isOnCachedState = false ;
                cachedState = HistoryState.Capture();

                DialougeSystem.instance.onUserPrompt_Next += GoFoward;
                DialougeSystem.instance.OnStartViewingHistory();
            }

            HistoryState state = history[progress];
            state.Load();
            UpdateStatusText();
        }

        private void UpdateStatusText()
        {
            statusText.text = $"{history.Count - progress}/{history.Count}";
        }
    }
}