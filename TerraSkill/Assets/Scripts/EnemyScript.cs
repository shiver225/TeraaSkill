using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    Animator animator;
    float health = 3;
    float timePassed;
    float attackCooldown = 2f;
    float attackRange = 1f;
    float spottingRange = 8f;
    float newDestinationCooldown = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);
        if(timePassed >= attackCooldown)
        {
            if(Vector3.Distance(player.position, transform.position) <= attackRange)
            {
                animator.SetTrigger("attack");
                timePassed = 0;
            }
        }
        timePassed += Time.deltaTime;

        if(newDestinationCooldown <= 0 && Vector3.Distance(player.position, transform.position) <= spottingRange)
        {
            newDestinationCooldown = 0.5f;
            agent.SetDestination(player.position);
        }
        newDestinationCooldown -= Time.deltaTime;
        transform.LookAt(player);
    }
}
