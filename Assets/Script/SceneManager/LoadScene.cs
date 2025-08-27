using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Commands;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private VisualNovelSO visualNovelConfig;
    [SerializeField] private TextAsset newStartingFile;

    // SceneLoaderData.cs
    public static class SceneLoaderData
    {
        public static bool isLoadingFromLoadScene = false;
    }
    public void LoadTextFile()
    {
        if (newStartingFile != null)
        {
            SceneLoaderData.isLoadingFromLoadScene = true;
            // setting file as defult file in vnc config
            visualNovelConfig?.SetStartingFile(newStartingFile);
            string[] commandParams = new string[] { "-file", newStartingFile.name };
            DatabaseExtendsionGeneral.LoadNewDialogueFile(commandParams);
        }
    }
    public void LoadNextScene()
    {
        SceneLoaderData.isLoadingFromLoadScene = true;
        SceneManager.LoadScene(sceneName);
    }
    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(1f);
    }
}