using UnityEngine;
using LB.Loot.Experience;

namespace LB
{
    public class DamageCollider : MonoBehaviour
    {
        private Collider damageCollider;
        private int currentWeaponDamage;
        private IExperienceReceiver ownerExperienceReceiver;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void SetWeaponDamage(int _damage) => currentWeaponDamage = _damage;

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            try
            {
                ownerExperienceReceiver = GetComponentInParent<IExperienceReceiver>();
            }
            catch (System.Exception e)
            {
                throw;
            }

            if (other.CompareTag("Player"))
            {
                RPGCharacterStats rpgCharacterStats = other.GetComponent<RPGCharacterStats>();

                if (rpgCharacterStats != null)
                {

                    rpgCharacterStats.TakeDamage(currentWeaponDamage, ownerExperienceReceiver);
                }
            }

            if (other.CompareTag("Enemy"))
            {
                EnemyStats enemyStats = other.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWeaponDamage, ownerExperienceReceiver);
                }
            }
        }
    }
}