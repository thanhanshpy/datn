using Dialouge;
using System.Data;
using UnityEngine;

public class Save : MonoBehaviour
{
    public static GameObject[] persistentObjects = new GameObject[4];
    public int objectIndex;
    private void Awake()
    {
        if (persistentObjects[objectIndex] == null)
        {
            persistentObjects[objectIndex] = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (persistentObjects[objectIndex] != gameObject)
        {
            Destroy(gameObject);
            return;
        }
    }
}
