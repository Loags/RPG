using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float health = 100f;
    public float attackValue = 20f;
    public float blockStaminaCost = 10f;
    public float blockStaminaRegen = 2f;
    public float healthRegen = 5f;

    private float currentStamina;
    private float currentHealth;
    private bool isBlocking = false;
    private bool isInCombat = false;

    void Start()
    {
        currentStamina = blockStaminaCost;
        currentHealth = health;
    }

    void Update()
    {
       
    }

    void Attack()
    {
        print("Attack");
    }

    public void TakeDamage(float damage)
    {
        if (isBlocking)
        {
            currentStamina -= damage / 2;
        }
        else
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        print("Dead");
    }
}
