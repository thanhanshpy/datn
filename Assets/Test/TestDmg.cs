using UnityEngine;

public class TestDmg : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            if (collision.TryGetComponent<Health>(out var health))
            {
                health.Damage(10);
            }
        }    
    }
}
