using UnityEngine;
using UnityEngine.AI;

public class Creep : MonoBehaviour
{
    private GameObject target;
    public NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
