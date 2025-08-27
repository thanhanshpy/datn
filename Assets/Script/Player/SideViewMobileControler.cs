using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class SideViewMobileControler : MonoBehaviour
{
    public Joystick joystick;
    private float horizontal;
    private float moveSpeed = 5f, jumpHigh = 30f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Health health;
    private Vector2 moveInput;

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

        FlipSprite();
        horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        moveInput = new Vector2(horizontal, 0f);

        bool isMoving = Mathf.Abs(horizontal) != 0;
        bool isOnGround = IsGrounded();
        animator.SetBool("isWalking", isMoving && isOnGround);

        if (vertical >= 0.5f && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHigh);
        }

        //if (vertical >= 0.5f && rb.linearVelocity.y > 0f)
        //{
        //    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        //}

        if (!IsGrounded())
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

    public void Attack()
    {
        animator.SetTrigger("Attack");
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

    public void TriggerDash()
    {
        if (!canDash || isDead) return;

        Vector2 inputDirection = moveInput.normalized;

        if (inputDirection != Vector2.zero)
        {
            StartCoroutine(Dash(inputDirection));
        }
    }

    public void MobileAttack()
    {
        if (!isDead)
        {
            Attack();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MobileAttack();
            Debug.Log("Attack performed");
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TriggerDash();
            Debug.Log("Dash performed");
        }
    }
}
