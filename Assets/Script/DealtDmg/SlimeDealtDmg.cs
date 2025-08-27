using System.Collections;
using UnityEngine;

public class SlimeDealtDmg : MonoBehaviour
{
    private float timer = 0f;
    private Health slimeHealth;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        slimeHealth = GetComponent<Health>();
        slimeHealth.Died.AddListener(OnDied);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        timer += Time.deltaTime;
        if (collision.gameObject.tag.Equals("Player") && timer >= 0.5f)
        {
            if (collision.TryGetComponent<Health>(out var health))
            {
                health.Damage(10);
            }

            timer = 0f;
        }
    }

    void OnDied()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        ContinueStory.instance.ShowContinueStory();

        StartCoroutine(Dying());
    }

    IEnumerator Dying()
    {
        yield return new WaitForSeconds(1.1f);

        Destroy(gameObject);
    }
}
