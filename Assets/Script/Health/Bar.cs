using System.Collections;
using UnityEngine;

public class Bar : MonoBehaviour
{
    [field:SerializeField] public int MaxValue {  get; private set; }
    [field: SerializeField] public int Value { get; private set; }

    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;

    private float fullWidth;
    private float targetWidth => Value * fullWidth / MaxValue;

    [SerializeField] private float animationSpeed = 10f;

    private Coroutine adjustBarWidthCoroutine;

    private void Start()
    {
        fullWidth = topBar.rect.width;
    }
    public void Change(int amount)
    {
        Value = Mathf.Clamp(Value + amount, 0, MaxValue);
        if(adjustBarWidthCoroutine != null)
        {
            StopCoroutine(adjustBarWidthCoroutine);
        }

        adjustBarWidthCoroutine = StartCoroutine(AdjustBarWidth(amount));
    }

    private IEnumerator AdjustBarWidth(int amount)
    {
        var suddenChangeBar = amount >= 0 ? bottomBar : topBar;
        var slowChangeBar = amount >= 0 ? topBar : bottomBar;
        suddenChangeBar.SetWidth(targetWidth);
        while (Mathf.Abs(suddenChangeBar.rect.width - slowChangeBar.rect.width) > 1f)
        {
            slowChangeBar.SetWidth(Mathf.Lerp(slowChangeBar.rect.width, targetWidth, Time.deltaTime * animationSpeed));
            yield return null;
        }

        slowChangeBar.SetWidth(targetWidth);
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Change(20);
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    Change(-20);
        //}
    }

}
