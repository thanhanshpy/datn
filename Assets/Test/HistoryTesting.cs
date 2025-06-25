using History;
using UnityEngine;

namespace Testing
{
    public class HistoryTesting : MonoBehaviour
    {
        public HistoryState state = new HistoryState();

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyUp(KeyCode.H))
            {
                state = HistoryState.Capture();
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                state.Load();
            }
        }
    }
}