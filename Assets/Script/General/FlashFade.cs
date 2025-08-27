using UnityEngine;
using UnityEngine.UI;

public class FlashFade : MonoBehaviour
{
    public float fadeDuration = 0.5f;

    private Image image;
    private Color originalColor;
    private float timer;

    void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
