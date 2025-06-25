using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{
    public GameObject prefab;
    public GameObject spawn;
    public GameObject parent;

    public void Spawning()
    {
        StartCoroutine(Spawner());
    }

    IEnumerator Spawner()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(1);
            GameObject clone = Instantiate(prefab, spawn.transform.position, Quaternion.identity);
            clone.transform.parent = parent.transform;
        }
    }
    
}
