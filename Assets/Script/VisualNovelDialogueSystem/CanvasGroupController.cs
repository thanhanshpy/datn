using Dialouge;
using System.Collections;
using UnityEngine;

public class CanvasGroupController 
{
    private const float defaultFadeSpeed = 3f;
    private MonoBehaviour onwer;
    private CanvasGroup rootCG;

    private Coroutine co_showing = null;
    private Coroutine co_hiding = null;
    public bool isShowing => co_showing != null;
    public bool isHiding => co_hiding != null;
    public bool isFading => isHiding || isShowing;

    public bool isVisible => co_showing != null || rootCG.alpha > 0;
    public float alpha { get { return rootCG.alpha; } set { rootCG.alpha = value; } }
    public CanvasGroupController(MonoBehaviour onwer, CanvasGroup root)
    {
        this.rootCG = root;
        this.onwer = onwer;
    }

    public Coroutine Show(float speed = 1f, bool immediate = false)
    {
        if (isShowing)
        {
            return co_showing;
        }
        else if (isHiding)
        {
            DialougeSystem.instance.StopCoroutine(co_hiding);
            co_hiding = null;
        }

        co_showing = DialougeSystem.instance.StartCoroutine(Fading(1, speed, immediate));
        return co_showing;
    }

    public Coroutine Hide(float speed = 1f, bool immediate = false)
    {
        if (isHiding)
        {
            return co_hiding;
        }
        else if (isShowing)
        {
            DialougeSystem.instance.StopCoroutine(co_showing);
            co_showing = null;
        }

        co_hiding = DialougeSystem.instance.StartCoroutine(Fading(0, speed, immediate));
        return co_hiding;
    }

    public IEnumerator Fading(float alpha, float speed, bool immediate)
    {
        CanvasGroup cg = rootCG;

        if (immediate)
        {
            cg.alpha = alpha;
        }

        while (cg.alpha != alpha)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * defaultFadeSpeed * speed);
            yield return null;
        }

        co_hiding = null;
        co_showing = null;
    }

    public void SetInteracableState(bool active)
    {
        rootCG.interactable = active;
        rootCG.blocksRaycasts = active;
    }
}
