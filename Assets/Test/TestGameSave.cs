using UnityEngine;
using VisualNovel;
namespace Testing
{
    public class TestGameSave : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            VNGameSave.activeFile = new VNGameSave();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                VNGameSave.activeFile.Save();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                VNGameSave.activeFile  = FileManager.Load<VNGameSave>($"{FilePath.gameSaves}1{VNGameSave.fileType}");
                VNGameSave.activeFile.Activate();
            }
        }
    }
}