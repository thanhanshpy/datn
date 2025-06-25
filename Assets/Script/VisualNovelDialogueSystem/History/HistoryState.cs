using Dialouge;
using System.Collections.Generic;
using UnityEngine;

namespace History
{
    [System.Serializable]
    public class HistoryState 
    {
        public DialogueData1 dialogue;
        public List<CharacterData> characters;
        public List<AudioData> audio;
        public List<GraphicData> graphics;

        public static HistoryState Capture()
        {
            HistoryState state = new HistoryState();
            state.dialogue = DialogueData1.Capture();
            state.characters = CharacterData.Capture();
            state.audio = AudioData.Capture();
            state.graphics = GraphicData.Capture();

            return state;
        }

        public void Load()
        {
            DialogueData1.Apply(dialogue);
            CharacterData.Apply(characters);
            AudioData.Apply(audio);
            GraphicData.Apply(graphics);
        }

    }
}