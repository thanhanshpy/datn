using System.Collections;
using UnityEngine;

namespace Testing
{
    public class ChoicePanelTesting : MonoBehaviour
    {
        ChoicePanel panel;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            panel = ChoicePanel.instance;

            StartCoroutine(asd());

            
        }

        IEnumerator asd()
        {
            string[] choices = new string[]
            {
                "dfsvasdf asdf as",
                "3 9a8fh aw9yefh ",
                "lf 2fyh ifne 4 alsdfga hlkasdhf ahsdfkja sdfhaslkj asjdfaslk alkh falk jaf asdf asjd af",
                "line 4 alsdfga hlkasdhf ahsdfkja sdfhaslkj asjdfaslk alkh falk jaf asdf asjdbfsflashfasjfasjfakjfalfa aj jasfh"
            };

            panel.Show("Line?", choices);

            while (panel.isWaitingOnUserChoice)
            {
                yield return null;
            }

            var decision = panel.lastDecision;

            Debug.Log($"made choice {decision.answerIndex} '{decision.choices[decision.answerIndex]}'");
        }


    }
}