using System.Collections;
using UnityEngine;

public class SwordCombat : MonoBehaviour
{
    public Animator animator;
    public float damage = 10f;
    public float range = 2f;
    public LayerMask hitLayers;

    private HealthController health;

    private bool isAttacking = false;

    void Update()
    {
        // Check for attack input
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        // Perform damage check
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, range, hitLayers);
        foreach (RaycastHit hit in hits)
        {
            // Deal damage to the collided object if it has a collider
            if (hit.collider != null)
            {
                // Apply damage directly to the object
                ApplyDamage(hit.collider.gameObject);
            }
        }

        // Reset attack state after a short delay
        Invoke("ResetAttack", 0.5f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void ApplyDamage(GameObject obj)
    {
        // Check if the object has a HealthController component and apply damage if it does
        health = obj.GetComponent<HealthController>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}