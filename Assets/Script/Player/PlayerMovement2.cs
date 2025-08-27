using System.Collections;
using UnityEngine;


public class PlayerMovement2 : MonoBehaviour
{
    private float horizontal;
    private float moveSpeed = 5f, jumpHigh = 30f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Health health;

    [SerializeField] float startDashTime = 0.3f;
    [SerializeField] float dashSpeed = 15f;
    float dashCooldown = 0.75f;
    private bool isDead = false;

    float currentDashTime;

    bool canDash = true;
    bool isDashing = false;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();

        health.Died.AddListener(Died);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        DashDirection();
        FlipSprite();
        Attack();
        horizontal = Input.GetAxisRaw("Horizontal");

        bool isMoving = Mathf.Abs(horizontal) > 0.01f;
        bool isOnGround = IsGrounded();
        animator.SetBool("isWalking", isMoving && isOnGround);

        if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHigh);
        }

        if (Input.GetKeyUp(KeyCode.W) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        if(!IsGrounded())
        {
            rb.gravityScale = 10f;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        }
    }

    
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void FlipSprite()
    {
        bool hasHori = Mathf.Abs(rb.linearVelocity.x) > Mathf.Epsilon;
        if (hasHori)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.linearVelocity.x) > 0 ? +2 : -2, 2f);
        }
    }

    IEnumerator Dash(Vector2 direction)
    {
        Physics2D.IgnoreLayerCollision(6, 7, true);

        isDashing = true;
        animator.SetBool("isDashing", true);
        canDash = false;

        currentDashTime = startDashTime;

        while (currentDashTime > 0f)
        {

            currentDashTime -= Time.deltaTime;
            if (IsGrounded())
            {
                rb.linearVelocity = direction * dashSpeed;
            }
            else
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(direction.x * dashSpeed, rb.linearVelocity.y);
            }

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = IsGrounded() ? 1f : 10f;

        animator.SetBool("isDashing", false);
        isDashing = false;
        Physics2D.IgnoreLayerCollision(6, 7, false);

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    public void DashDirection()
    {
        if (canDash && Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.A))
            {
                StartCoroutine(Dash(Vector2.left));
            }

            else if (Input.GetKey(KeyCode.D))
            {
                StartCoroutine(Dash(Vector2.right));
            }
        }
    }

    public void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    public void Died()
    {
        StartCoroutine(Dying());
    }

    IEnumerator Dying()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        if (!isDead)
        {
            animator.SetTrigger("isDying");
            isDead = true;
        }

        yield return new WaitForSeconds(2f);

        PlayAgain.instance.ShowDeathScreen();
        gameObject.SetActive(false);
    }
}
