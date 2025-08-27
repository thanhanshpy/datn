using UnityEngine;

public class FlipToPlayer : MonoBehaviour
{
    Transform player;
    Rigidbody2D rb;
    public bool isFlipped = false;
    BossStateManager boss;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        boss = GetComponent<BossStateManager>();
    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    void Update()
    {
        if (boss != null && boss.canFlip)
        {
            LookAtPlayer();
        }
    }
}
