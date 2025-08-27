using UnityEngine;

public class EnemyDealtDmg : MonoBehaviour
{
    private float timer = 0f;

    public void OnTriggerStay2D(Collider2D collision)
    {
        timer += Time.deltaTime;
        if (collision.gameObject.tag.Equals("Player")  && timer >=1f)
        {
            if (collision.TryGetComponent<Health>(out var health))
            {
                health.Damage(10);
            }

            timer = 0f;
        }
    }

   
}
