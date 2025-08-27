using System.Collections;
using UnityEngine;

public class ContinueStory : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;
    public static ContinueStory instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeIn()
    {
        Debug.Log("Starting FadeIn at time: " + Time.time);
        //StopAllCoroutines();

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        //yield return new WaitForSeconds(0.1f);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void ShowContinueStory()
    {
        StartCoroutine(FadeIn());
    }
}
