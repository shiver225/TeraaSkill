using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem deathParticle;
    public float maxHealth = 100f;
    public float currentHealth;
    public bool dead = false;
    public int gainedExp = 40;
    [SerializeField] FloatingHealthBar healthBar;
    [SerializeField] ExpController controller;
    [SerializeField] PlayerExpBar expBar;

    public Vector3 offset;

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        controller = FindObjectOfType<ExpController>();
        expBar = FindObjectOfType<PlayerExpBar>();
    }
    void Start()
    {
        //expBar = GetComponent<MainPlayerController>().inventoryPanel.gameObject.transform.parent.GetComponentInChildren<PlayerExpBar>();
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
        dead = true;
        Debug.Log(controller.currentExp);
        controller.currentExp = expBar.UpdateExpBar(controller.currentExp, gainedExp);
        Debug.Log("Enemy died");
        Instantiate(deathParticle, transform.position + offset, Quaternion.identity);
        Destroy(gameObject);
    }
}
