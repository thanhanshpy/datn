using UnityEngine;

public class PlayerDealtDmg : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.TryGetComponent<Health>(out var health))
        {
            health.Damage(10);
        }
        
    }
}
