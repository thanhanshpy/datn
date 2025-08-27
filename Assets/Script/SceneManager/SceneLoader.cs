using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
