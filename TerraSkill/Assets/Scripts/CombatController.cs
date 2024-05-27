using System.Collections;
using System.Linq;
using UnityEngine;

public class SwordCombat : MonoBehaviour
{
    public Animator animator;
    public float baseDamage = 10f;
    public float unarmedRange = 1.2f; // Default attack range without a sword
    public LayerMask hitLayers;
    public Transform swordTransform;  // Reference to the transform of the sword
    public EquipmentObject equippedWeapon;  // Reference to the equipped weapon
    private bool isAttacking = false;
    public HealthController healthController;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }
    }

    private void FindSwordInHand()
    {
        // Find all transforms in children and look for the sword by name
        swordTransform = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name.Contains("First Blade(Clone)"));

        // If we found the sword, get its GroundItem component and then the ItemObject
        if (swordTransform != null)
        {
            GroundItem groundItem = swordTransform.GetComponent<GroundItem>();
            if (groundItem != null)
            {
                ItemObject item = groundItem.item;
                if (item != null && item is EquipmentObject equipmentObject)
                {
                    equippedWeapon = equipmentObject;
                }
                else
                {
                    equippedWeapon = null;
                }
            }
            else
            {
                equippedWeapon = null;
            }
        }
        else
        {
            equippedWeapon = null; // Ensure equippedWeapon is null if the sword is not found
        }
    }

    public void PerformAttack()
    {
        FindSwordInHand(); // Ensure we find the sword in hand before attacking

        Debug.Log("PerformAttack");
        float totalDamage = baseDamage + (equippedWeapon != null ? equippedWeapon.atkBonus : 0);

        // Calculate the attack range based on whether a sword is equipped
        float attackRange = (equippedWeapon != null) ? equippedWeapon.range + 0.2f : unarmedRange;

        // Calculate the origin for the raycast
        Vector3 raycastOrigin = transform.position + Vector3.up * 0.7f;

        // If the sword is equipped and it's in the right hand, use its position for the raycast
        if (equippedWeapon != null && swordTransform != null && IsSwordInRightHand())
        {
            raycastOrigin = swordTransform.position;
        }

        // Cast a ray from the calculated origin forward
        RaycastHit[] hits = Physics.RaycastAll(raycastOrigin, transform.forward, attackRange, hitLayers);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit);
            GameObject hitObject = hit.collider.gameObject;
            healthController = hitObject.GetComponent<HealthController>();

            if (healthController != null)
            {
                Debug.Log($"Hit {hitObject.name} for {totalDamage} damage.");
                healthController.TakeDamage(totalDamage);
            }
        }

        // Draw a debug ray to visualize the raycast
        Debug.DrawRay(raycastOrigin, transform.forward * attackRange, Color.red, 1f);
    }

    public void ResetAttack()
    {
        Debug.Log("ResetAttack");
        isAttacking = false;
    }

    bool IsSwordInRightHand()
    {
        // Check if the sword transform is a child of the right hand
        if (swordTransform != null && swordTransform.IsChildOf(transform))
        {
            Transform parent = swordTransform.parent;
            while (parent != null)
            {
                Debug.Log("Parent: " + parent.name); // Add this line for debugging
                if (parent.name == "RhandWrapper")
                {
                    return true;
                }
                parent = parent.parent;
            }
        }
        return false;
    }
    bool IsStaffInRightHand()
    {
        // Check if the sword transform is a child of the right hand
        if (swordTransform != null && swordTransform.IsChildOf(transform))
        {
            Transform parent = swordTransform.parent;
            while (parent != null)
            {
                Debug.Log("Parent: " + parent.name); // Add this line for debugging
                if (parent.name == "RhandWrapper")
                {
                    return true;
                }
                parent = parent.parent;
            }
        }
        return false;
    }

    // This method is called by another script (e.g., a script on the sword object)
    public void DetectSword(Transform sword)
    {
        swordTransform = sword;
        equippedWeapon = sword.GetComponent<EquipmentObject>();
        Debug.Log("Sword detected!");
    }

}
