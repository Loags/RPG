using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LB
{
    public class DamagePlayer : MonoBehaviour
    {
        public int damage = 25;

        private void OnTriggerEnter(Collider other)
        {
            RPGCharacterStats rpgCharacterStats = other.GetComponent<RPGCharacterStats>();

            if (rpgCharacterStats != null)
            {
                rpgCharacterStats.TakeDamage(damage);
            }
        }
    }
}