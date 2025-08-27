using Dialouge;
using System.Collections.Generic;
using UnityEngine;
using static LoadScene;

namespace VisualNovel
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager instance { get; private set; }
        [SerializeField] private VisualNovelSO config;
        public Camera mainCamera;

        private void Awake()
        {
            instance = this;

            VNDatabaseLinkSetUp linkSetUp = GetComponent<VNDatabaseLinkSetUp>();
            linkSetUp.SetUpExternalLinks();

            if (VNGameSave.activeFile == null)
            {
                VNGameSave.activeFile = new VNGameSave();
            }               
        }

        private void Start()
        {
            LoadGame();
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

        private void LoadGame()
        {
            if (VNGameSave.activeFile.newGame || SceneLoaderData.isLoadingFromLoadScene)
            {
                List<string> lines = FileManager.ReadTextAsset(config.startingFile);
                Conversation start = new Conversation(lines);
                DialougeSystem.instance.Say(start);

                // reset
                SceneLoaderData.isLoadingFromLoadScene = false;
            }
            else
            {
                VNGameSave.activeFile.Activate();
            }
        }
    }
}