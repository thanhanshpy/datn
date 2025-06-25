using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;

    [SerializeField] RectTransform barRect;

    [SerializeField] private RectMask2D mask;

    [SerializeField] private TMP_Text hpIndicator;

    private float maxRightMax;
    private float initialRightMax;

    private float fullWidth;

    private void Start()
    {        
        fullWidth = barRect.rect.width;
        hpIndicator.SetText($"{health.Hp}/{health.MaxHp}");
    }
    public void SetValue(int newValue)
    {        
        float percent = (float)newValue / health.MaxHp;
        var size = barRect.sizeDelta;
        size.x = fullWidth * percent;
        barRect.sizeDelta = size;

        hpIndicator.SetText($"{newValue}/{health.MaxHp}");
    }
}
