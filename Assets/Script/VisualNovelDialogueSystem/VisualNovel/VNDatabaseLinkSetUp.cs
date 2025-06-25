using UnityEngine;

namespace VisualNovel
{
    public class VNDatabaseLinkSetUp : MonoBehaviour
    {
        public void SetUpExternalLinks()
        {
            VariableStore.CreateVariable("VN.mainCharName", "", () => VNGameSave.activeFile.playerName, value => VNGameSave.activeFile.playerName = value);

        }
    }
}