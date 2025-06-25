using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Vector2 dashDirection;

    [SerializeField] float startDashTime = 0.3f;
    [SerializeField] float dashSpeed = 15f;
    float dashCooldown = 2f;

    float currentDashTime;

    bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DashDirection();
        rb.linearVelocity = moveInput * moveSpeed;
        FlipSprite();
        Attack();
    }

    public void Move(InputAction.CallbackContext context)
    {       
        animator.SetBool("isWalking", true);
        moveInput = context.ReadValue<Vector2>();
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
        }
        
    }

    void FlipSprite()
    {
        bool hasHori = Mathf.Abs(rb.linearVelocity.x) > Mathf.Epsilon;
        if (hasHori)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.linearVelocity.x) > 0 ? +2 : -2 , 2f);
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

    public void DashDirection()
    {
        if (canDash && Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                StartCoroutine(Dash(new Vector2(1f, 1f)));
            }

            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                StartCoroutine(Dash(new Vector2(1f, -1f)));
            }

            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
            {
                StartCoroutine(Dash(new Vector2(-1f, 1f)));
            }

            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                StartCoroutine(Dash(new Vector2(-1f, -1f)));
            }

            else if (Input.GetKey(KeyCode.W))
            {
                StartCoroutine(Dash(Vector2.up));
            }

            else if (Input.GetKey(KeyCode.A))
            {
                StartCoroutine(Dash(Vector2.left));
            }

            else if (Input.GetKey(KeyCode.S))
            {
                StartCoroutine(Dash(Vector2.down));
            }

            else if (Input.GetKey(KeyCode.D))
            {
                StartCoroutine(Dash(Vector2.right));
            }
        }
    }

    public void Attack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

}
