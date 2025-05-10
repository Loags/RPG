using LB.Inventory;
using UnityEngine;

namespace LB
{
    /// <summary>
    /// Static class which contains hardcoded animation constants and helper functions.
    /// Simplified for single weapon slot.
    /// </summary>
    public class AnimationData
    {
        /// <summary>
        /// Returns the duration of an attack animation based on weapon type and attack number.
        /// </summary>
        /// <param name="weaponType">The type of weapon attacking.</param>
        /// <param name="attackNumber">Attack animation number.</param>
        /// <returns>Duration in seconds of attack animation.</returns>
        public static float AttackDuration(Weapon weaponType, int attackNumber)
        {
            float duration = 1f; // Default duration

            switch (weaponType)
            {
                case Weapon.MeleeWeapon: // Updated from TwoHandedWeapon
                    // Example durations based on attack number for armed state
                    // Cast attackNumber to the MeleeAttack enum for potential switch/if checks later
                    MeleeAttack meleeAttack = (MeleeAttack)attackNumber;
                    // Example: if (meleeAttack == MeleeAttack.Attack1) duration = 1.1f;
                    if (attackNumber == 1) duration = 1.1f;
                    else if (attackNumber == 2) duration = 0.9f;
                    else if (attackNumber == 3) duration = 1.0f;
                    // Add more cases as needed for your animations
                    else duration = 1.0f; // Default armed duration
                    break;

                case Weapon.Unarmed:
                    // Example durations for unarmed attacks
                    if (attackNumber == 1) duration = 0.75f;
                    else if (attackNumber == 2) duration = 0.8f;
                    // Add more cases as needed
                    else duration = 0.75f; // Default unarmed duration
                    break;

                // Removed OneHandedWeapon case
                // Removed Shield case

                default:
                    // Handle cases like Weapon.NONE or unexpected values
                    if (weaponType != Weapon.NONE)
                    {
                         Debug.LogError("AnimationData.AttackDuration: Unhandled weapon type " + weaponType);
                    }
                    duration = 1f; // Ensure a default duration even for NONE/Error
                    break;
            }
            return duration;
        }

        /// <summary>
        /// Returns the duration of the weapon sheath/unsheath animation based on weapon type.
        /// </summary>
        /// <param name="weaponType">Weapon being sheathed/unsheathed.</param>
        /// <returns>Duration in seconds of sheath animation.</returns>
        public static float SheathDuration(Weapon weaponType)
        {
            // Simplified logic: Unarmed = 0, MeleeWeapon = standard duration
            if (weaponType == Weapon.Unarmed)
            {
                return 0f;
            }
            else if (weaponType == Weapon.MeleeWeapon) // Updated check
            {
                // Use a generic duration for sheathing any weapon.
                // Adjust this value based on your actual sheath/unsheath animations.
                return 1.1f;
            }
            else
            {
                 // Handle cases like Weapon.NONE
                 if (weaponType != Weapon.NONE) {
                     Debug.LogWarning($"SheathDuration called for unexpected weapon type: {weaponType}. Returning 0.");
                 }
                 return 0f;
            }
        }

        /// <summary>
        /// Returns a random attack number usable as the animator's Action parameter, based on weapon type.
        /// </summary>
        /// <param name="weaponType">Weapon type attacking.</param>
        /// <returns>Attack animation number.</returns>
        public static int RandomAttackNumber(Weapon weaponType)
        {
            switch (weaponType)
            {
                case Weapon.MeleeWeapon: // Updated from TwoHandedWeapon
                    // Use TakeRandom on the renamed MeleeAttacks array
                    if (AnimationVariations.MeleeAttacks != null && AnimationVariations.MeleeAttacks.Length > 0)
                       return (int)AnimationVariations.MeleeAttacks.TakeRandom();
                    else {
                        Debug.LogWarning("AnimationVariations.MeleeAttacks array not found or empty. Defaulting attack number.");
                        return 1;
                    }

                case Weapon.Unarmed:
                     // Use TakeRandom on the defined UnarmedRightAttacks array as the default unarmed pool
                     if (AnimationVariations.UnarmedRightAttacks != null && AnimationVariations.UnarmedRightAttacks.Length > 0)
                        return (int)AnimationVariations.UnarmedRightAttacks.TakeRandom();
                     else {
                        Debug.LogWarning("AnimationVariations.UnarmedRightAttacks array not found or empty. Defaulting attack number.");
                        return 1;
                     }

                 // Removed OneHandedWeapon case
                 // Removed Shield case

                default:
                    if (weaponType != Weapon.NONE) {
                         Debug.LogError($"AnimationData.RandomAttackNumber: Unhandled weapon type {weaponType}");
                    }
                    return 1; // Default fallback
            }
        }

        public static Vector3 HitDirection(HitType hitType)
        {
            switch (hitType)
            {
                case HitType.Back1: return Vector3.forward;
                case HitType.Left1: return Vector3.right;
                case HitType.Right1: return Vector3.left;
                case HitType.Forward1:
                case HitType.Forward2:
                default: return Vector3.back;
            }
        }

        public static Vector3 HitDirection(KnockbackType hitType)
        {
            switch (hitType)
            {
                case KnockbackType.Knockback1:
                case KnockbackType.Knockback2:
                default: return Vector3.back;
            }
        }

        public static Vector3 HitDirection(KnockdownType hitType)
        {
            return Vector3.back;
        }
    }
}