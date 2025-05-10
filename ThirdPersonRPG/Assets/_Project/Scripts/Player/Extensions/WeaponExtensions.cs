using LB.Inventory;
using UnityEngine;

namespace LB
{
    public static class WeaponExtensions
    {
        /// <summary>
        /// Checks if the weapon is the primary melee weapon type.
        /// </summary>
        public static bool IsMeleeWeapon(this Weapon weapon)
        {
            // Check against the new MeleeWeapon enum value
            return weapon == Weapon.MeleeWeapon;
        }

        /// <summary>
        /// Checks if a weapon is equipped (i.e., not Unarmed).
        /// </summary>
        public static bool HasEquippedWeapon(this Weapon weapon)
        {
            return weapon != Weapon.Unarmed;
        }

        /// <summary>
        /// Checks if the state is Unarmed.
        /// </summary>
        public static bool HasNoWeapon(this Weapon weapon)
        {
            return weapon == Weapon.Unarmed;
        }

        /// <summary>
        /// Returns true if the weapon type might use IK.
        /// Simplified: Assumes any equipped weapon could potentially use IK if set up.
        /// </summary>
        public static bool IsIKWeapon(this Weapon weapon)
        {
            // MeleeWeapon is the only type other than Unarmed now that might use IK.
            return weapon == Weapon.MeleeWeapon;
        }

        /// <summary>
        /// Converts the Weapon enum to the simplified AnimatorWeapon enum.
        /// Maps Unarmed to UNARMED, and MeleeWeapon to MELEE.
        /// </summary>
        public static AnimatorWeapon ToAnimatorWeapon(this Weapon weapon)
        {
            if (weapon == Weapon.Unarmed)
            {
                return AnimatorWeapon.UNARMED;
            }
            else if (weapon == Weapon.MeleeWeapon) // Check for MeleeWeapon
            {
                return AnimatorWeapon.MELEE; // Map to MELEE
            }
            else
            {
                Debug.LogWarning($"Unhandled Weapon type {weapon} in ToAnimatorWeapon. Defaulting to UNARMED.");
                return AnimatorWeapon.UNARMED; // Fallback
            }
        }

        /// <summary>
        /// Checks if the animator weapon state represents an armed (melee) state.
        /// </summary>
        public static bool IsArmedAnimWeapon(this AnimatorWeapon animWeapon)
        {
            // Check against the renamed MELEE state
            return animWeapon == AnimatorWeapon.MELEE;
        }

        /// <summary>
        /// Checks if the animator weapon state is Unarmed.
        /// </summary>
        public static bool HasNoAnimWeapon(this AnimatorWeapon animWeapon)
        {
            return animWeapon == AnimatorWeapon.UNARMED;
        }
    }
}