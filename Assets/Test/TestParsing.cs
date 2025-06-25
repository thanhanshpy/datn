using Dialouge;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class TestParsing : MonoBehaviour
    {
        //[SerializeField] private TextAsset file;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SendFileToParse();
        }

        void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTextAsset("testFile");

            foreach (string line in lines)
            {
                if (line == string.Empty) continue;
                DialougeLine dl = DialougeParser.Parse(line);
            }
        }
    }

}
