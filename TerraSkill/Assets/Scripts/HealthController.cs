using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem deathParticle;
    public float maxHealth = 100f;
    public float currentHealth;

    [SerializeField] FloatingHealthBar healthBar;

    public Vector3 offset;

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");
        Instantiate(deathParticle, transform.position + offset, Quaternion.identity);
        Destroy(gameObject);
    }
}
