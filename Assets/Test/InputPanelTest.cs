using UnityEngine;
using Characters;
using System.Collections;
public class InputPanelTest : MonoBehaviour
{
    public InputPanel inputPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(test());
    }

   IEnumerator test()
    {
        Character Merlin = CharacterManager.instance.CreateCharacter("Merlin", revealAfterCreation: true);

        yield return Merlin.Say("hi");

        inputPanel.Show("what is your name?");

        while (inputPanel.isWaitingOnUserInput)
        {
            yield return null;
        }

        string characterName = inputPanel.lastInput;

        yield return Merlin.Say($"hi, {characterName} ");
    }
}
