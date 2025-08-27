using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //public float speed = 2f;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    private float lockedY;
    private bool yLocked = false;
    //private int direction = 1;

    void Update()
    {
        if (!yLocked && IsGrounded())
        {
            lockedY = transform.position.y;
            yLocked = true;
        }

        // Lock Y after touching ground
        if (yLocked)
        {
            Vector3 pos = transform.position;
            pos.y = lockedY;
            transform.position = pos;
        }

        // Move only in X axis
        //transform.position += new Vector3(direction * speed * Time.deltaTime, 0f, 0f);

        // Optional: reverse direction at edges (demo)
        //if (transform.position.x >= 5f || transform.position.x <= -5f)
        //{
        //    direction *= -1;
        //}
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
