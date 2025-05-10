using LB.Inventory;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace LB
{
    public class RPGCharacterWeaponController : MonoBehaviour
    {
        /// <summary>
        /// RPGCharacterController component
        /// </summary>
        private RPGCharacterController rpgCharacterController;

        /// <summary>
        /// RPGCharacterEquipmentController component
        /// </summary>
        private RPGCharacterEquipmentController rpgCharacterEquipmentController;

        /// <summary>
        /// Animator component
        /// </summary>
        private Animator animator;

        /// <summary>
        /// Target GameObject where the weapon will be spawned (previously right hand).
        /// </summary>
        [SerializeField] private GameObject weaponTarget;

        [Header("Debug Options")]
        public bool debugWalkthrough = false;

        private bool isWeaponSwitching = false;

        [Header("Weapon Model Reference")]
        public GameObject equippedWeaponModel;

        private DamageCollider activeDamageCollider;
        private Coroutine weaponColliderCoroutine = null;

        [Header("Unarmed")]
        [SerializeField] private GameObject _unarmedWeaponObject;

        private void Awake()
        {
            rpgCharacterController = GetComponent<RPGCharacterController>();
            rpgCharacterEquipmentController = GetComponent<RPGCharacterEquipmentController>();
            animator = GetComponentInChildren<Animator>();

            if (animator == null)
            {
                Debug.LogError("Animator component not found in children!", this);
                enabled = false;
                return;
            }
            if (weaponTarget == null)
            {
                Debug.LogError("Weapon Target GameObject is not assigned!", this);
                enabled = false;
                return;
            }

            rpgCharacterController.SetHandler(HandlerTypes.SwitchWeapon, new SwitchWeapon());

        }

        private void Start()
        {
            // Listen for the animator's weapon switch event.
            if (animator.TryGetComponent<RPGCharacterAnimatorEvents>(out var animatorEvents))
            {
                animatorEvents.OnWeaponSwitch.AddListener(OnAnimator_WeaponSwitch);
            }
            else
            {
                Debug.LogError("RPGCharacterAnimatorEvents component not found on Animator GameObject!", animator.gameObject);
            }

            // Character starts Unarmed, ensure no weapon model is active initially
            HideAllWeapons(true);

        }

        /// <summary>
        /// Switch to the specified weapon type instantly, spawning the provided model.
        /// </summary>
        /// <param name="weaponType">The type of weapon being equipped (e.g., Unarmed, MeleeWeapon).</param>
        /// <param name="weaponModelPrefab">The prefab of the weapon model to instantiate.</param>
        public void InstantWeaponSwitch(Weapon weaponType, GameObject weaponModelPrefab)
        {
            // Prevent switching if already switching or prefab is null (unless switching to unarmed)
            //if (isWeaponSwitching || (weaponModelPrefab == null && weaponType != Weapon.Unarmed))
            if (isWeaponSwitching || weaponModelPrefab == null)
            {
                Debug.LogWarning($"InstantWeaponSwitch called prematurely or with null prefab for type {weaponType}.", this);
                return;
            }

            if (debugWalkthrough) Debug.Log($"_InstantWeaponSwitch: Type={weaponType}");

            isWeaponSwitching = true;
            animator.SetAnimatorTrigger(AnimatorTrigger.InstantSwitchTrigger);
            rpgCharacterController.SetIKOff();
            HideAllWeapons(false);

            rpgCharacterController.equippedWeapon = weaponType;
            animator.SetInteger(AnimationParameters.Weapon, (int)weaponType.ToAnimatorWeapon());

            //if (weaponType != Weapon.Unarmed)
            //{
            SpawnWeapon(weaponModelPrefab);

            if (debugWalkthrough) Debug.Log($"SetWeaponVisibility: Visible={true}");

            if (equippedWeaponModel != null) equippedWeaponModel.SetActive(true);

            if (weaponType.IsIKWeapon()) rpgCharacterController.SetIKOn();
            //}

            isWeaponSwitching = false;
        }

        /// <summary>
        /// Callback from Animator Event to signal the weapon switch animation point.
        /// </summary>
        public void OnAnimator_WeaponSwitch()
        {
            // This might be used for timing specific actions during a *non-instant* switch animation.
            // For instant switches, the flag is handled in the coroutine.
            if (isWeaponSwitching) // Potentially relevant for non-instant switches
            {
                isWeaponSwitching = false;
            }
            // Debug.Log("Animator WeaponSwitch Event Fired");
        }

        /// <summary>
        /// Hides the currently equipped weapon model and optionally resets the animator/controller state to Unarmed.
        /// </summary>
        /// <param name="resetToUnarmed">If true, sets Animator and Controller weapon states to Unarmed.</param>
        public void HideAllWeapons(bool resetToUnarmed)
        {
            if (debugWalkthrough) Debug.Log($"HideAllWeapons: resetToUnarmed={resetToUnarmed}");

            // Wait if currently in the middle of switching.
            //yield return new WaitUntil(() => !isWeaponSwitching);
            //yield return new WaitForSeconds(0.1f);

            DestroyWeapon();

            if (resetToUnarmed)
            {
                if (animator != null) animator.SetInteger(AnimationParameters.Weapon, (int)AnimatorWeapon.UNARMED);
                if (rpgCharacterController != null)
                {
                    rpgCharacterController.equippedWeapon = Weapon.Unarmed;
                    InstantWeaponSwitch(Weapon.Unarmed, rpgCharacterController.rpgCharacterEquipmentController.unarmedWeapon.characterDisplay);
                }
            }
        }

        /// <summary>
        /// Instantiates the weapon prefab, parents it, and sets up the damage collider reference.
        /// </summary>
        /// <param name="weaponPrefab">The weapon prefab to instantiate.</param>
        private void SpawnWeapon(GameObject weaponPrefab)
        {
            if (weaponPrefab == null)
            {
                Debug.LogError("SpawnWeapon called with null prefab!", this);
                return;
            }
            if (weaponTarget == null)
            {
                Debug.LogError("Cannot spawn weapon, weaponTarget is not assigned!", this);
                return;
            }

            // Destroy existing model first to prevent duplicates
            DestroyWeapon();

            equippedWeaponModel = Instantiate(weaponPrefab, weaponTarget.transform);
            equippedWeaponModel.SetActive(false);
            if (debugWalkthrough) Debug.Log($"Spawned Weapon: {equippedWeaponModel.name}");

            // Stop any previous collider toggling
            TerminateWeaponDamageColliderCoroutine();

            // Get and configure the damage collider
            activeDamageCollider = GetActiveWeaponDamageCollider();
        }

        /// <summary>
        /// Destroys the currently instantiated weapon model.
        /// </summary>
        private void DestroyWeapon()
        {
            TerminateWeaponDamageColliderCoroutine(); // Stop collider coroutine before destroying
            activeDamageCollider = null; // Clear reference

            if (equippedWeaponModel != null)
            {
                if (debugWalkthrough) Debug.Log($"Destroying Weapon: {equippedWeaponModel.name}");
                Destroy(equippedWeaponModel);
                equippedWeaponModel = null;
            }
        }

        /// <summary>
        /// Gets and configures the DamageCollider from the currently equipped weapon model.
        /// </summary>
        /// <returns>The configured DamageCollider, or null if not found.</returns>
        private DamageCollider GetActiveWeaponDamageCollider()
        {
            if (equippedWeaponModel == null) return null;

            DamageCollider collider = equippedWeaponModel.GetComponentInChildren<DamageCollider>();
            if (collider != null)
            {
                // Set damage based on character stats
                if (rpgCharacterController != null && rpgCharacterController.rpgCharacterStats != null)
                {
                    collider.SetWeaponDamage(rpgCharacterController.rpgCharacterStats.GetMaxValueOfAttributeType(Attributes.Strength));
                }
                else
                {
                    Debug.LogWarning("Could not set weapon damage - Character Controller or Stats missing.", this);
                }
            }
            else
            {
                Debug.LogWarning($"No DamageCollider found in children of {equippedWeaponModel.name}", equippedWeaponModel);
            }
            return collider;
        }

        /// <summary>
        /// Starts the coroutine to toggle the active weapon's damage collider based on timestamps.
        /// </summary>
        /// <param name="startTime">Time in seconds after which to enable the collider.</param>
        /// <param name="endTime">Time in seconds after startTime at which to disable the collider.</param>
        public void ToggleWeaponDamageCollider(float startTime, float duration)
        {
            if (activeDamageCollider == null)
            {
                // Debug.LogWarning("ToggleWeaponDamageCollider called but no active collider exists.");
                return;
            }
            // Stop any existing coroutine first
            TerminateWeaponDamageColliderCoroutine();
            weaponColliderCoroutine = StartCoroutine(_ToggleWeaponDamageCollider(startTime, duration));
        }

        /// <summary>
        /// Stops the weapon damage collider toggling coroutine if it's running.
        /// Made public to be callable from Action Handlers during cleanup (_EndAction).
        /// </summary>
        public void TerminateWeaponDamageColliderCoroutine()
        {
            if (weaponColliderCoroutine != null)
            {
                StopCoroutine(weaponColliderCoroutine);
                weaponColliderCoroutine = null;
                // Ensure collider is disabled when coroutine is stopped prematurely
                if (activeDamageCollider != null) activeDamageCollider.DisableDamageCollider();
            }
        }

        /// <summary>
        /// Coroutine to enable and then disable the damage collider after specified delays.
        /// </summary>
        private IEnumerator _ToggleWeaponDamageCollider(float startTime, float duration)
        {
            // Wait for the start time
            if (startTime > 0) yield return new WaitForSeconds(startTime);

            // Enable the collider
            if (activeDamageCollider != null) activeDamageCollider.EnableDamageCollider();
            else { yield break; } // Exit if collider became null

            // Wait for the duration
            if (duration > 0) yield return new WaitForSeconds(duration);
            else { Debug.LogWarning("ToggleWeaponDamageCollider duration is zero or negative."); }

            // Disable the collider
            if (activeDamageCollider != null) activeDamageCollider.DisableDamageCollider();

            weaponColliderCoroutine = null; // Mark coroutine as finished
        }
    }
}