using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    Animator animator;
    float timePassed;
    float attackCooldown = 2f;
    float attackRange = 1f;
    float spottingRange = 8f;
    float newDestinationCooldown = 0.5f;

    public float attack = 25;
    private PlayerHealthContoller health;
    public LayerMask hitLayers;

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
                PerformAttack();
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

    void PerformAttack()
    {
        // Perform damage check
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, attackRange, hitLayers);
        foreach (RaycastHit hit in hits)
        {
            // Deal damage to the collided object if it has a collider
            if (hit.collider != null)
            {
                // Apply damage directly to the object
                ApplyDamage(hit.collider.gameObject);
            }
        }
    }

    void ApplyDamage(GameObject obj)
    {
        // Check if the object has a HealthController component and apply damage if it does
        health = obj.GetComponent<PlayerHealthContoller>();
        if (health != null)
        {
            health.TakeDamage(attack);
        }
    }
}
