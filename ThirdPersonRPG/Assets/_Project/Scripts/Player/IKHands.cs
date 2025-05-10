using LB.Inventory;
using System.Collections;
using UnityEngine;

namespace LB
{
    public class IKHands : MonoBehaviour
    {
        private Animator animator;
        private RPGCharacterWeaponController rpgCharacterWeaponController;
        
        [Tooltip("Main transform of the character's Left Hand bone.")]
        public Transform leftHandObj;
        
        [Tooltip("Transform within the weapon prefab where the left hand should attach.")]
        public Transform attachLeft; // This will be dynamically assigned by GetCurrentWeaponAttachPoint
        
        [Tooltip("Can IK be used with the current setup?")]
        public bool canBeUsed; // Should be set based on whether leftHandObj is assigned
        
        [Tooltip("Is IK currently active and being applied?")]
        public bool isUsed; // Controlled by BlendIK
        
        [Range(0, 1)] public float leftHandPositionWeight;
        [Range(0, 1)] public float leftHandRotationWeight;
        
        private Transform currentWeaponAttachPoint; // Stores the found attach point
        private Coroutine blendCoroutine = null; // Reference to the blending coroutine
        private Coroutine pauseCoroutine = null; // Reference to the pause coroutine

        private void Awake()
        {
            animator = GetComponent<Animator>();
            // Find the controller in the parent
            rpgCharacterWeaponController = GetComponentInParent<RPGCharacterWeaponController>();

            if (animator == null) Debug.LogError("IKHands requires an Animator component.", this);
            if (rpgCharacterWeaponController == null) Debug.LogError("IKHands could not find RPGCharacterWeaponController in parent.", this);
            if (leftHandObj == null) Debug.LogWarning("IKHands: Left Hand Obj transform is not assigned.", this);

            // Determine if IK *can* be used based on essential components/refs
            canBeUsed = leftHandObj != null && animator != null && rpgCharacterWeaponController != null;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            // Only apply IK if it's set to be used and we have a valid attach point
            if (!canBeUsed || !isUsed || attachLeft == null)
            {
                // Ensure weights are zero if not used or attach point is missing
                if (leftHandPositionWeight > 0) animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                if (leftHandRotationWeight > 0) animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                return;
            }

            // Apply IK weights and targets
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, attachLeft.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, attachLeft.rotation);
        }

        /// <summary>
        /// Smoothly blends IK on or off for the specified weapon.
        /// </summary>
        /// <param name="blendOn">True to blend IK on, false to blend off.</param>
        /// <param name="delay">Delay before starting the blend.</param>
        /// <param name="timeToBlend">Duration of the blend.</param>
        /// <param name="weapon">The weapon being equipped (used to find the attach point).</param>
        public void BlendIK(bool blendOn, float delay, float timeToBlend, Weapon weapon)
        {
            if (!canBeUsed)
            {
                //Debug.LogWarning("BlendIK called but canBeUsed is false.");
                return;
            }

            // Stop any existing blend or pause coroutines
            StopBlendCoroutine();
            StopPauseCoroutine();

            if (blendOn)
            {
                // Find the attach point for the given weapon.
                currentWeaponAttachPoint = GetCurrentWeaponAttachPoint(weapon);
                if (currentWeaponAttachPoint != null)
                {
                    attachLeft = currentWeaponAttachPoint; // Set the target for OnAnimatorIK
                    isUsed = true; // Mark IK as active
                    blendCoroutine = StartCoroutine(_BlendIKWeight(true, delay, timeToBlend));
                }
                else
                {
                    // If no attach point found, ensure IK is off
                     Debug.LogWarning($"No IK attach point found for weapon {weapon}. Disabling IK.", this);
                    isUsed = false;
                    blendCoroutine = StartCoroutine(_BlendIKWeight(false, 0f, 0.1f)); // Blend off quickly
                }
            }
            else // Blending Off
            {
                isUsed = false; // Mark IK as inactive
                blendCoroutine = StartCoroutine(_BlendIKWeight(false, delay, timeToBlend));
            }
        }

        // Coroutine solely responsible for blending the IK weights
        private IEnumerator _BlendIKWeight(bool blendOn, float delay, float timeToBlend)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            
            float t = 0f;
            float startWeightPos = leftHandPositionWeight;
            float startWeightRot = leftHandRotationWeight;
            float targetWeight = blendOn ? 1.0f : 0.0f;
            
            // Ensure timeToBlend is positive to avoid division by zero
            timeToBlend = Mathf.Max(timeToBlend, Time.deltaTime); 

            while (t < 1)
            {
                t += Time.deltaTime / timeToBlend;
                leftHandPositionWeight = Mathf.Lerp(startWeightPos, targetWeight, t);
                leftHandRotationWeight = Mathf.Lerp(startWeightRot, targetWeight, t);
                yield return null;
            }
            
            // Ensure weights are exact at the end
            leftHandPositionWeight = targetWeight;
            leftHandRotationWeight = targetWeight;
            blendCoroutine = null; // Mark coroutine as finished
        }

        /// <summary>
        /// Pauses IK (blends off, waits, blends on) for a specified duration.
        /// </summary>
        /// <param name="pauseTime">Total duration of the pause sequence.</param>
        public void SetIKPause(float pauseTime)
        {
            if (!canBeUsed || !isUsed || pauseCoroutine != null) // Don't start if already pausing
            { 
                return;
            }
            
            StopBlendCoroutine(); // Stop any active blending
            pauseCoroutine = StartCoroutine(_SetIKPause(pauseTime));
        }

        private IEnumerator _SetIKPause(float pauseTime)
        {
            float blendOffTime = 0.1f;
            float blendOnTime = 0.1f;
            float waitTime = pauseTime - blendOffTime - blendOnTime;

            // Blend Off
            yield return StartCoroutine(_BlendIKWeight(false, 0f, blendOffTime)); 

            // Wait (if duration allows)
            if (waitTime > 0) yield return new WaitForSeconds(waitTime);
            
            // Blend On
            yield return StartCoroutine(_BlendIKWeight(true, 0f, blendOnTime));

            pauseCoroutine = null; // Mark pause as finished
        }

        /// <summary>
        /// Finds the left hand attachment point transform within the currently equipped weapon's prefab.
        /// Assumes a child GameObject named "Attach_L" exists.
        /// </summary>
        /// <param name="weapon">The weapon type (e.g., MeleeWeapon or Unarmed). Used to ensure we only search on melee weapons.</param>
        /// <returns>The attach point transform, or null if not found or not a melee weapon.</returns>
        private Transform GetCurrentWeaponAttachPoint(Weapon weapon)
        {
            // Don't search if it's not a MeleeWeapon type that uses IK
            if (weapon != Weapon.MeleeWeapon || rpgCharacterWeaponController == null || rpgCharacterWeaponController.equippedWeaponModel == null)
            {
                return null; // No weapon controller or model available, or not a melee weapon
            }

            // Standardize the search for the attach point, assuming consistent naming
            // Modify "Attach_L" if your naming convention is different.
            Transform attachPoint = rpgCharacterWeaponController.equippedWeaponModel.transform.Find("Attach_L");

            if (attachPoint == null)
            {
                 Debug.LogWarning($"Could not find IK attach point named 'Attach_L' in {rpgCharacterWeaponController.equippedWeaponModel.name}", rpgCharacterWeaponController.equippedWeaponModel);
            }

            return attachPoint;
        }
        
        // Helper methods to safely stop coroutines
        private void StopBlendCoroutine()
        {
             if (blendCoroutine != null) { StopCoroutine(blendCoroutine); blendCoroutine = null; }
        }
         private void StopPauseCoroutine()
        {
             if (pauseCoroutine != null) { StopCoroutine(pauseCoroutine); pauseCoroutine = null; }
        }
    }
}