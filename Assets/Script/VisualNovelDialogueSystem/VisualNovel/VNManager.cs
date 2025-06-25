using Dialouge;
using System.Collections.Generic;
using UnityEngine;

namespace VisualNovel
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager instance { get; private set; }

        private void Awake()
        {
            instance = this;

            VNDatabaseLinkSetUp linkSetUp = GetComponent<VNDatabaseLinkSetUp>();
            linkSetUp.SetUpExternalLinks();
        }
        public void LoadFile(string filePath)
        {
            List<string> lines = new List<string>();
            TextAsset file = Resources.Load<TextAsset>(filePath);

            try
            {
                lines = FileManager.ReadTextAsset(file);
            }
            catch
            {
                Debug.LogError($"dialogue file path 'Resources/{filePath}' does not exist");
                return;
            }

            DialougeSystem.instance.Say(lines, filePath);
        }
    }
}