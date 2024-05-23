using System.Collections;
using UnityEngine;

public class SwordCombat : MonoBehaviour
{
    public Animator animator;
    public float baseDamage = 10f;
    public float range = 2f;
    public LayerMask hitLayers;
    public EquipmentObject equippedWeapon;  // Reference to the equipped weapon

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            //PerformAttack();
        }
    }

    public void PerformAttack()
    {
        Debug.Log("PerformAttack");
        float totalDamage = baseDamage + (equippedWeapon != null ? equippedWeapon.atkBonus : 0);

        // Calculate the origin for the raycast
        Vector3 raycastOrigin = transform.position + Vector3.up * 0.5f;

        // Cast a ray from the calculated origin forward
        RaycastHit[] hits = Physics.RaycastAll(raycastOrigin, transform.forward, range, hitLayers);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit);
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log(hitObject.name);
            HealthController healthController = hitObject.GetComponent<HealthController>();

            if (healthController != null)
            {
                Debug.Log($"Hit {hitObject.name} for {totalDamage} damage.");
                healthController.TakeDamage(totalDamage);
            }
        }

        // Draw a debug ray to visualize the raycast
        Debug.DrawRay(raycastOrigin, transform.forward * range, Color.red, 1f);

    }

    public void ResetAttack()
    {
        Debug.Log("ResetAttack");
        isAttacking = false;
    }
}
