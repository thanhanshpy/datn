using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownMobileMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Health health;
    private bool isDead = false;

    [SerializeField] float startDashTime = 0.3f;
    [SerializeField] float dashSpeed = 15f;
    float dashCooldown = 2f;

    float currentDashTime;
    bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

        health.Died.AddListener(Died);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        rb.linearVelocity = moveInput * moveSpeed;
        FlipSprite();
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isDead) return;

        animator.SetBool("isWalking", true);
        moveInput = context.ReadValue<Vector2>();
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
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

        animator.SetBool("isDashing", true);
        canDash = false;

        currentDashTime = startDashTime;

        while (currentDashTime > 0f)
        {

            currentDashTime -= Time.deltaTime;
            rb.linearVelocity = direction * dashSpeed;

            yield return null;
        }

        rb.linearVelocity = new Vector2(0f, 0f);

        animator.SetBool("isDashing", false);
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

}
