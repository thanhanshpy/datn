using System.Collections;
using UnityEngine;

public class TyphoonDied : MonoBehaviour
{
    private Health bossHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossHealth = GetComponent<Health>();
        bossHealth.Died.AddListener(OnBossDied);
    }

    void OnBossDied()
    {
        //StartCoroutine(Dying());
        ContinueStory.instance.ShowContinueStory();
    }

    IEnumerator Dying()
    {
        yield return new WaitForSeconds(2f);

        ContinueStory.instance.ShowContinueStory();
    }
}
