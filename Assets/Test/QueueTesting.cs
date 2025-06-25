
using Dialouge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueTesting : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(abx());
    }

    IEnumerator abx()
    {
        List<string> lines = new List<string>()
        {
            "dfsvasdf asdf as",
            "3 9a8fh aw9yefh ",
            "lf 2fyh ifne 4 alsdfga hlkasdhf ahsdfkja sdfhaslkj asjdfaslk alkh falk jaf asdf asjd af",
            "line 4 alsdfga hlkasdhf ahsdfkja sdfhaslkj asjdfaslk alkh falk jaf asdf asjdbfsflashfasjfasjfakjfalfa aj jasfh"
        };

        yield return DialougeSystem.instance.Say(lines);

        DialougeSystem.instance.Hide();
    }

    void Update()
    {
        List<string> lines = new List<string>();
        Conversation conversation = null;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            lines = new List<string>
            {
            "This is the start of an enqueued conversation.",
            "We can keep it going!"
            };
            conversation = new Conversation(lines);
            DialougeSystem.instance.conversationManager.Enqueue(conversation);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            lines = new List<string>
            {
            "This is an important conversation!",
            "August 26, 2023 is international dog day!"
            };
            conversation = new Conversation(lines);
            DialougeSystem.instance.conversationManager.EnqueuePriority(conversation);
        }
    }

}
