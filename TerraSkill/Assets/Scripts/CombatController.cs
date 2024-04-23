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
            //animator.SetTrigger("Attack");
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        // Perform raycast to detect hit objects within attack range and hitLayers
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, range, hitLayers);

        foreach (RaycastHit hit in hits)
        {
            // Get the GameObject of the collider hit by the raycast
            GameObject hitObject = hit.collider.gameObject;

            // Check if the hit object has a HealthController component
            HealthController healthController = hitObject.GetComponent<HealthController>();

            if (healthController != null)
            {
                // Apply damage to the object with HealthController (e.g., player or enemy)
                healthController.TakeDamage(damage);
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
        HealthController healthController = obj.GetComponent<HealthController>();

        if (healthController != null)
        {
            // Apply damage to the object with HealthController (e.g., player or enemy)
            healthController.TakeDamage(damage);
        }
    }
}