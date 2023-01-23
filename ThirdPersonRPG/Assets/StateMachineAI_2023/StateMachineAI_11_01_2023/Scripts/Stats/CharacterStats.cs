using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth { get; private set; }

    public Stat Damage;
    public Stat Armor;
    public Stat Health;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            TakeDamage(10);
    }

    public void TakeDamage(int _damage)
    {
        _damage -= Armor.GetValue();
        _damage = Mathf.Clamp(_damage, 0, int.MaxValue);

        CurrentHealth -= _damage;
        print(transform.name + " takes " + _damage + " damage.");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // This Method will be overriden
        print(transform.name + " died.");
    }
}
