using Dialouge;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VisualNovel;


public class TestDialogue : MonoBehaviour
    {
        [SerializeField] private TextAsset fileToRead = null;

        void Start()
        {
            StartConversation();
        }

        void StartConversation()
        {
            string fullPath = AssetDatabase.GetAssetPath(fileToRead);

            int resourcesIndex = fullPath.IndexOf("Resources/");
            string relativePath = fullPath.Substring(resourcesIndex + 10);
            
            string filePath = Path.ChangeExtension(relativePath, null);
            
            VNManager.instance.LoadFile(filePath);
        }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.UpArrow))
        {
            DialougeSystem.instance.dialougeContainer.Show();
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            DialougeSystem.instance.dialougeContainer.Hide();
        }
    }
}

