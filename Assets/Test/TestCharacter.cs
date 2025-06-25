using UnityEngine;
using Characters;
using System.Collections;
using System.Collections.Generic;
using Dialouge;
using TMPro;
using TreeEditor;
namespace Testing
{
    public class TestCharacter : MonoBehaviour
    {
        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);
        public TMP_FontAsset tempFont;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Character Seigneur = CharacterManager.instance.CreateCharacter("Merlin");
            //Character Seigneur2 = CharacterManager.instance.CreateCharacter("Seigneur");
            //Character Book = CharacterManager.instance.CreateCharacter("Book");
            StartCoroutine(Test());
        }
        IEnumerator Test()
        {
            CharacterSprite Merlin = CreateCharacter("Merlin") as CharacterSprite;
            CharacterSprite Wanderer = CreateCharacter("Wanderer") as CharacterSprite;
            CharacterSprite guard1 = CreateCharacter("NPC") as CharacterSprite;

            Merlin.SetPosition(new Vector2(0.20f,0));
            Wanderer.SetPosition(new Vector2(0.25f, 0));
            guard1.SetPosition(new Vector2(0.35f, 0));

            Merlin.SetPriority(15);
            Wanderer.SetPriority(10);
            guard1.SetPriority(1);

            yield return new WaitForSeconds(1);

            CharacterManager.instance.SortCharacters(new string[] {"Wanderer", "guard1"});

            yield return null;
        }
        IEnumerator Talk()
        {
            CharacterSprite Merlin = CreateCharacter("Merlin") as CharacterSprite;
            CharacterSprite Wanderer = CreateCharacter("Wanderer") as CharacterSprite;

            Merlin.SetPosition(Vector2.zero);
            Wanderer.SetPosition(new Vector2(1, 0));

            Wanderer.UnHighlight();
            yield return Merlin.Say("hey");

            Merlin.UnHighlight();
            Wanderer.Highlight();
            yield return Wanderer.Say("wtf?");

            Wanderer.UnHighlight();
            Merlin.Highlight();
            Merlin.TransitionColor(Color.red);
            yield return Merlin.Say("what did you just say?");

            yield return Wanderer.Flip(immediate: true);
            Wanderer.Highlight();
            Wanderer.SetPosition(new Vector2(0.5f, 0));
            yield return Wanderer.Say("sorry");

            Wanderer.UnHighlight();
            Merlin.TransitionColor(Color.white);
            yield return Merlin.Say("ok");

            Merlin.UnHighlight();
            Wanderer.Highlight();
            yield return Wanderer.FaceLeft();
            Wanderer.SetPosition(new Vector2(1, 0));
            yield return Wanderer.Say("pheww");
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}