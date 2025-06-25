using Characters;
using System.Collections;
using UnityEngine;
using Characters;
using Dialouge;

namespace Testing
{
    public class AudioTestinh : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(Running());
        }

        Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        IEnumerator Running()
        {
            CharacterSprite Merlin = CreateCharacter("Merlin") as CharacterSprite;
            Merlin.Show();

            GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("BackGround/phancanh4.2");

            AudioManager.instance.PlayTrack("Audio/Ambience/RainyMood", 0);
            AudioManager.instance.PlayTrack("Audio/Music/Comedy", 1, pitch: 0.7f);

            yield return Merlin.Say("2 auudios");

            AudioManager.instance.StopTrack(1);

        }
        IEnumerator Running2()
        {
            AudioChannel channel = new AudioChannel(1);

            yield return null;
        }

    }
}