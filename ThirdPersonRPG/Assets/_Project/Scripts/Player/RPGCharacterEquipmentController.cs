using LB.Inventory;
using System;
using System.Collections;
using UnityEngine;

namespace LB
{
    public class RPGCharacterEquipmentController : MonoBehaviour
    {
        /// <summary>
        /// Event called when the equipment has been changed
        /// </summary>
        public event Action OnEquipmentChanged = delegate { };

        /// <summary>
        /// PlayerStats component
        /// </summary>
        private RPGCharacterStats rpgCharacterStats;

        /// <summary>
        /// RPGCharacterWeaponController component
        /// </summary>
        private RPGCharacterWeaponController rpgCharacterWeaponController;

        /// <summary>
        /// Reference to the equipment ScriptableObject
        /// </summary>
        public InventoryObject equipment;

        /// <summary>
        /// Returns whether the equipment is loaded or not
        /// </summary>
        public bool isEquipmentLoaded => _isEquipmentLoaded;
        private bool _isEquipmentLoaded;

        /// <summary>
        /// Returns the currently equipped weapon item.
        /// </summary>
        public ItemObject equippedWeapon => _equippedWeapon;
        private ItemObject _equippedWeapon;

        // Index constant for the single weapon slot within the equipment container
        // TODO: Verify this index matches your equipment ScriptableObject setup!
        private const int WEAPON_SLOT_INDEX = 8; // Assuming index 8 based on original code (Slots[Length - 2])

        /// <summary>
        /// Equip the unarmedWeapon ItemObject automatically
        /// </summary>
        public ItemObject unarmedWeapon;

        // References to combined armor meshes (kept for armor logic)
        private Transform helmet;
        private Transform chest;
        private Transform gloves;
        private Transform legs;
        private Transform boots;
        // Removed Transform weapon, offhand - managed by RPGCharacterWeaponController

        private BoneCombiner boneCombiner;

        private void Awake()
        {
            rpgCharacterWeaponController = GetComponent<RPGCharacterWeaponController>();
            rpgCharacterStats = GetComponent<RPGCharacterStats>();
            StartCoroutine(LoadEquipment());
        }

        private IEnumerator LoadEquipment()
        {
            _isEquipmentLoaded = false;
            if (equipment == null)
            {
                Debug.LogError("Equipment InventoryObject is not assigned!", this);
                yield break; // Stop if equipment SO is missing
            }

            equipment.Load();
            yield return new WaitForSeconds(0.1f);

            // Initial equipment application after loading
            foreach (InventorySlot slot in equipment.Container.Slots)
            {
                if (slot != null && slot.ItemObject != null)
                {
                    // Use Add logic to apply stats and visuals for initially equipped items
                    OnAddEquipmentItem(slot);
                }
            }

            yield return new WaitForSeconds(0.1f);
            _isEquipmentLoaded = true;
            UpdateEquippedWeaponData(); // Update internal weapon reference
            OnEquipmentChanged?.Invoke(); // Notify listeners equipment is ready
        }

        private void Start()
        {
            boneCombiner = new BoneCombiner(gameObject);
            // Subscribe AFTER initial load to avoid null refs if UpdateEquippedWeaponData relies on loaded state
            OnEquipmentChanged += UpdateEquippedWeaponData;
        }

        /// <summary>
        /// Updates the internal reference to the currently equipped weapon based on the equipment slot.
        /// </summary>
        private void UpdateEquippedWeaponData()
        {
            if (equipment == null || equipment.Container == null || equipment.Container.Slots == null || WEAPON_SLOT_INDEX < 0 || WEAPON_SLOT_INDEX >= equipment.Container.Slots.Length)
            {
                 Debug.LogError($"Cannot update weapon data. Equipment setup invalid or WEAPON_SLOT_INDEX ({WEAPON_SLOT_INDEX}) out of bounds.");
                 _equippedWeapon = unarmedWeapon; // Default to unarmed on error
                 return;
            }

            InventorySlot weaponSlot = equipment.Container.Slots[WEAPON_SLOT_INDEX];

            _equippedWeapon = (weaponSlot != null && weaponSlot.ItemObject != null) ? weaponSlot.ItemObject : unarmedWeapon;

            // Optional: Log the equipped weapon
            // Debug.Log($"Equipped Weapon Updated: {(_equippedWeapon != null ? _equippedWeapon.Name : "Unarmed")}");
        }

        /// <summary>
        /// Handles removing stats and visuals when an item is unequipped.
        /// </summary>
        /// <param name="_slot">The inventory slot being cleared.</param>
        public void OnRemoveEquipmentItem(InventorySlot _slot)
        {
            if (_slot == null || _slot.ItemObject == null || _slot.parent == null || _slot.parent.inventory == null)
            {
                // Debug.LogWarning("OnRemoveEquipmentItem called with invalid slot data.");
                return; // Invalid slot data
            }

            // Only process equipment interface types
            if (_slot.parent.inventory.type != InterfaceType.Equipment) return;

            // Remove Stat Modifiers
            try
            {
                for (int i = 0; i < _slot.item.buffs.Count; i++)
                {
                    ItemBuff buff = _slot.item.buffs[i];
                    for (int j = 0; j < rpgCharacterStats.attributes.Length; j++)
                    {
                        if (rpgCharacterStats.attributes[j].type == buff.attribute)
                        {
                            rpgCharacterStats.attributes[j].value.RemoveModifier(buff);
                            // Debug.Log($"Removed buff {buff.attribute} modifier: {buff.Value}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error removing buffs for {_slot.ItemObject.name}: {e.Message}", this);
            }

            // Handle Visuals Removal
            if (_slot.ItemObject.characterDisplay != null)
            {
                foreach (AllowedItem allowedItem in _slot.AllowedItems)
                {
                    if (allowedItem.equipmentType == Equipment.Armor)
                    {
                        // Destroy the corresponding combined mesh part
                        switch (allowedItem.armorType)
                        {
                            case Armor.Helmet: if (helmet != null) Destroy(helmet.gameObject); helmet = null; break;
                            case Armor.Chest: if (chest != null) Destroy(chest.gameObject); chest = null; break;
                            case Armor.Gloves: if (gloves != null) Destroy(gloves.gameObject); gloves = null; break;
                            case Armor.Legs: if (legs != null) Destroy(legs.gameObject); legs = null; break;
                            case Armor.Boots: if (boots != null) Destroy(boots.gameObject); boots = null; break;
                        }
                    }
                    else if (allowedItem.equipmentType == Equipment.Weapon)
                    {
                        // If the item being removed is a weapon, tell WeaponController to switch to Unarmed.
                        // Check if this slot IS the designated weapon slot before switching.
                        if (equipment.Container.Slots[WEAPON_SLOT_INDEX] == _slot)
                        {
                            // Use the ItemObject's weaponType for the switch
                            // This assumes unarmedWeapon has a valid characterDisplay prefab
                            if (unarmedWeapon != null && unarmedWeapon.characterDisplay != null)
                            {
                                rpgCharacterWeaponController.InstantWeaponSwitch(Weapon.Unarmed, unarmedWeapon.characterDisplay);
                            }
                            else
                            {
                                Debug.LogError("Unarmed weapon ItemObject or its characterDisplay is not assigned! Cannot switch visuals.", this);
                                // Potentially just hide the current weapon without switching visuals
                                // rpgCharacterWeaponController.HideWeapon(); // Assuming such a method exists
                            }
                        }
                        break; // Weapon type found, no need to check other allowedItems for this slot
                    }
                    // Add cases for Accessories if needed
                }
            }

            // Use a small delay before invoking the event to ensure weapon data is updated *after* this removal processing
            StartCoroutine(DelayedEquipmentChangeNotification());
        }

        /// <summary>
        /// Handles adding stats and visuals when an item is equipped.
        /// </summary>
        /// <param name="_slot">The inventory slot being populated.</param>
        public void OnAddEquipmentItem(InventorySlot _slot)
        {
            if (_slot == null || _slot.ItemObject == null || _slot.parent == null || _slot.parent.inventory == null)
            {
                // Debug.LogWarning("OnAddEquipmentItem called with invalid slot data.");
                return; // Invalid slot data
            }

            // Only process equipment interface types
            if (_slot.parent.inventory.type != InterfaceType.Equipment) return;

            // Add Stat Modifiers
            try
            {
                for (int i = 0; i < _slot.item.buffs.Count; i++)
                {
                    ItemBuff buff = _slot.item.buffs[i];
                    for (int j = 0; j < rpgCharacterStats.attributes.Length; j++)
                    {
                        if (rpgCharacterStats.attributes[j].type == buff.attribute)
                        {
                            rpgCharacterStats.attributes[j].value.AddModifier(buff);
                            // Debug.Log($"Added buff {buff.attribute} modifier: {buff.Value}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                 Debug.LogError($"Error adding buffs for {_slot.ItemObject.name}: {e.Message}", this);
            }

            // Handle Visuals Addition
            if (_slot.ItemObject.characterDisplay != null)
            {
                // We assume only one relevant AllowedItem per slot for visual processing
                AllowedItem relevantAllowedItem = _slot.AllowedItems.Count > 0 ? _slot.AllowedItems[0] : null;
                if (relevantAllowedItem == null)
                {
                     Debug.LogWarning($"Slot for {_slot.ItemObject.name} has no AllowedItem definitions.", this);
                     return;
                }

                if (relevantAllowedItem.equipmentType == Equipment.Armor)
                {
                    // Add the combined mesh part
                    switch (relevantAllowedItem.armorType)
                    {
                        case Armor.Helmet: helmet = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames); break;
                        case Armor.Chest: chest = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames); break;
                        case Armor.Gloves: gloves = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames); break;
                        case Armor.Legs: legs = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames); break;
                        case Armor.Boots: boots = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames); break;
                    }
                }
                else if (relevantAllowedItem.equipmentType == Equipment.Weapon)
                {
                    // If the item being added is a weapon, tell WeaponController to switch.
                    // Check if this slot IS the designated weapon slot.
                    if (equipment.Container.Slots[WEAPON_SLOT_INDEX] == _slot && _slot.ItemObject is WeaponObject weaponData)
                    {
                        // Use the ItemObject's weaponType for the switch
                        rpgCharacterWeaponController.InstantWeaponSwitch(weaponData.weaponType, _slot.ItemObject.characterDisplay);
                    }
                    // No break needed here if weapon is the only allowed type
                }
                 // Add cases for Accessories if needed
            }
            else
            {
                 Debug.LogWarning($"Item {_slot.ItemObject.name} has no character display prefab assigned.", this);
            }

            // Use a small delay before invoking the event to ensure weapon data is updated *after* this addition processing
            StartCoroutine(DelayedEquipmentChangeNotification());
        }

        /// <summary>
        /// Delays the OnEquipmentChanged event invocation slightly.
        /// </summary>
        private IEnumerator DelayedEquipmentChangeNotification()
        {
            yield return null; // Wait one frame
            UpdateEquippedWeaponData(); // Ensure data is current before notification
            OnEquipmentChanged?.Invoke();
        }

        /// <summary>
        /// Gets the index of the equipment slot matching the specified criteria.
        /// Assumes only one slot matches the primary type (e.g., one Helmet slot, one Weapon slot).
        /// </summary>
        public int GetEquipmentSlotIndexWithType(ItemType itemType, Equipment? equipmentType = null,
            Weapon? weaponType = null, Armor? armorType = null, Accessory? accessoryType = null)
        {
             if (equipment == null || equipment.Container == null || equipment.Container.Slots == null) return -1;

            for (int i = 0; i < equipment.Container.Slots.Length; i++)
            {
                InventorySlot slot = equipment.Container.Slots[i];
                if (slot == null || slot.AllowedItems == null || slot.AllowedItems.Count == 0) continue;

                // Check based on the *first* AllowedItem definition for the slot
                AllowedItem allowedItem = slot.AllowedItems[0];

                if (allowedItem.itemType == itemType)
                {
                    switch (itemType)
                    {
                        case ItemType.Equipment:
                            if (equipmentType.HasValue && allowedItem.equipmentType == equipmentType.Value)
                            {
                                switch (equipmentType.Value)
                                {
                                    case Equipment.Weapon:
                                        // Since we have only one weapon slot, we just check if it's the weapon slot type
                                        // The specific weaponType (MeleeWeapon) doesn't determine the slot index anymore.
                                        return i; // Found the weapon slot
                                    case Equipment.Armor:
                                        if (armorType.HasValue && allowedItem.armorType == armorType.Value) return i;
                                        break;
                                    case Equipment.Accessory:
                                        if (accessoryType.HasValue && allowedItem.accessoryType == accessoryType.Value) return i;
                                        break;
                                }
                            }
                            break;
                        // Handle Consumable/Default if needed for equipment screen (unlikely)
                        case ItemType.Consumable:
                        case ItemType.Default:
                             // These types usually don't have dedicated equipment slots.
                             // If they do, return i;
                            break;
                    }
                }
            }
            return -1; // No matching slot found
        }
    }
}