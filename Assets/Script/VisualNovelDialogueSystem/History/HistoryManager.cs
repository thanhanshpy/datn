using Dialouge;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace History
{
    [RequireComponent(typeof(HistoryNavigation))]
    public class HistoryManager : MonoBehaviour
    {
        public const int historyCacheLimit = 15;
        public static HistoryManager instance { get; private set; }
        public List<HistoryState> history = new List<HistoryState>();

        private HistoryNavigation navigation;

        private void Awake()
        {
            instance = this;
            navigation = GetComponent<HistoryNavigation>();
        }

        public void LogCurrentState()
        {
            HistoryState state = HistoryState.Capture();
            history.Add(state);

            if(history.Count > historyCacheLimit)
            {
                history.RemoveAt(0);
            }
        }

        public void LoadState(HistoryState state)
        {
            state.Load();
        }
        private void Start()
        {
            DialougeSystem.instance.onClear += LogCurrentState;
        }

        public void GoFoward() => navigation.GoFoward();
        public void GoBack() => navigation.GoBack();
    }
}