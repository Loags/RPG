using LB.Inventory;
using UnityEngine;

namespace LB
{
    // Context removed/simplified as the action now reads state directly
    // public class SwitchWeaponContext { ... }

    // Specify 'object' as the context type since none is needed
    public class SwitchWeapon : BaseActionHandler<object> 
    {
        public override bool CanStartAction(RPGCharacterController controller)
        {
            // Can switch if not already switching (isWeaponSwitching flag in WeaponController handles this internally)
            // and if actions are generally allowed.
            return !IsActive() && controller.CanAction; 
        }

        public override bool CanEndAction(RPGCharacterController controller)
        {
            // Action ends immediately after initiating the switch
            return IsActive();
        }

        // Add 'object context' parameter back to match base class signature
        protected override void _StartAction(RPGCharacterController controller, object context)
        {
            // Get necessary components
            if (!controller.TryGetComponent<RPGCharacterWeaponController>(out var weaponController))
            {
                Debug.LogError("SwitchWeapon Action requires RPGCharacterWeaponController.", controller);
                EndAction(controller); // End immediately if component missing
                return;
            }
             if (!controller.TryGetComponent<RPGCharacterEquipmentController>(out var equipmentController))
            {
                Debug.LogError("SwitchWeapon Action requires RPGCharacterEquipmentController.", controller);
                EndAction(controller); // End immediately if component missing
                return;
            }

            // Check if currently armed
            bool isCurrentlyArmed = controller.equippedWeapon != Weapon.Unarmed;

            if (isCurrentlyArmed)
            {
                // --- Switch to Unarmed --- 
                if (equipmentController.unarmedWeapon != null && equipmentController.unarmedWeapon.characterDisplay != null)
                {
                    weaponController.InstantWeaponSwitch(Weapon.Unarmed, equipmentController.unarmedWeapon.characterDisplay);
                }
                else
                {
                    Debug.LogError("Cannot switch to Unarmed: Unarmed Weapon or its CharacterDisplay prefab is not assigned in EquipmentController.", equipmentController);
                }
            }
            else
            {
                // --- Switch to Equipped Weapon --- 
                ItemObject equippedItem = equipmentController.equippedWeapon;

                // Check if there is an item equipped and it's not the unarmed item itself
                if (equippedItem != null && equippedItem != equipmentController.unarmedWeapon && equippedItem is WeaponObject weaponData)
                {
                    if (weaponData.characterDisplay != null)
                    {
                       weaponController.InstantWeaponSwitch(weaponData.weaponType, weaponData.characterDisplay);
                    }
                    else
                    {
                         Debug.LogWarning($"Cannot equip {weaponData.name}: CharacterDisplay prefab is missing.", weaponData);
                    }
                }
                // else: No weapon equipped in the slot, or it's the unarmed item, so remain unarmed. No action needed.
            }

            // End the action handler state immediately after initiating the switch.
            // The visual switch itself is handled by the InstantWeaponSwitch coroutine.
            EndAction(controller);
        }


        protected override void _EndAction(RPGCharacterController controller)
        {
            // Cleanup if needed, though most logic is in _StartAction for this instant toggle.
        }
    }
}