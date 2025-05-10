using UnityEngine;

namespace LB.Inventory
{
	public class InventorySlotOptionPanel : MonoBehaviour
	{
		/// <summary>
		/// The player's equipment inventory.
		/// </summary>
		[SerializeField] private InventoryObject playerEquipmentInventory;

		/// <summary>
		/// The player's general inventory.
		/// </summary>
		[SerializeField] private InventoryObject playerInventory;

		/// <summary>
		/// The UI element for consuming an item.
		/// </summary>
		[SerializeField] private GameObject consumeItemObject;

		/// <summary>
		/// The UI element for equipping an item.
		/// </summary>
		[SerializeField] private GameObject equipItemObject;

		/// <summary>
		/// The UI element for unequipping an item.
		/// </summary>
		[SerializeField] private GameObject unequipItemObject;

		/// <summary>
		/// The UI element for deleting an item.
		/// </summary>
		[SerializeField] private GameObject deleteItemObject;

		/// <summary>
		/// The UI element for storing an item.
		/// </summary>
		[SerializeField] private GameObject storeItemObject;

		/// <summary>
		/// The UI element for retrieving an item.
		/// </summary>
		[SerializeField] private GameObject retrieveItemObject;

		/// <summary>
		/// The inventory slot information.
		/// </summary>
		private InventorySlotInfo inventorySlotInfo;

		/// <summary>
		/// The selected item object.
		/// </summary>
		private ItemObject selectedItemObject;

		/// <summary>
		/// The selected inventory slot.
		/// </summary>
		private InventorySlot selectedInventorySlot;

		/// <summary>
		/// Initializes the inventory slot option panel with the selected item and slot information.
		/// </summary>
		/// <param name="_itemObject">The selected item object.</param>
		/// <param name="_inventorySlot">The selected inventory slot.</param>
		/// <param name="_inventorySlotInfo">The inventory slot information.</param>
		public void Initialize(ItemObject _itemObject, InventorySlot _inventorySlot,
			InventorySlotInfo _inventorySlotInfo)
		{
			selectedItemObject = _itemObject;
			selectedInventorySlot = _inventorySlot;
			inventorySlotInfo = _inventorySlotInfo;

			ResetSlotOptions();

			deleteItemObject.SetActive(true);

			if (selectedItemObject as EquipmentObject)
			{
				if (StateManager.Instance.CurrentFlowState == AppStates.FlowStates.INVENTORY)
				{
					if (_inventorySlot.parent.inventory.type == InterfaceType.Equipment)
					{
						unequipItemObject.SetActive(true);
					}
					else if (_inventorySlot.parent.inventory.type == InterfaceType.Inventory)
					{
						equipItemObject.SetActive(true);
					}
				}
			}
			else if (selectedItemObject as ConsumableObject)
			{
				consumeItemObject.SetActive(true);
			}
			else if (selectedItemObject as DefaultObject)
			{
			}

			if (StateManager.Instance.CurrentFlowState == AppStates.FlowStates.STORAGEBOX)
			{
				if (_inventorySlot.parent.inventory.type == InterfaceType.Inventory)
				{
					storeItemObject.SetActive(true);
				}

				if (_inventorySlot.parent.inventory.type == InterfaceType.StorageBox)
				{
					retrieveItemObject.SetActive(true);
				}
			}
		}

		/// <summary>
		/// Equips the selected item.
		/// </summary>
		public void EquipItem()
		{
			ItemObject itemObject = selectedInventorySlot.ItemObject;
			int index = -1;

			if (itemObject is EquipmentObject equipmentObject)
			{
				switch (equipmentObject.equipmentType)
				{
					case Equipment.Weapon:
						WeaponObject weaponObject = (WeaponObject)equipmentObject;
						index = RPGCharacterController.Instance.rpgCharacterEquipmentController
							.GetEquipmentSlotIndexWithType(
								ItemType.Equipment, Equipment.Weapon, weaponType: weaponObject.weaponType);
						break;
					case Equipment.Armor:
						ArmorObject armorObject = (ArmorObject)equipmentObject;
						index = RPGCharacterController.Instance.rpgCharacterEquipmentController
							.GetEquipmentSlotIndexWithType(
								ItemType.Equipment, Equipment.Armor, armorType: armorObject.armorType);
						break;
					case Equipment.Accessory:
						AccessoryObject accessoryObject = (AccessoryObject)equipmentObject;
						index = RPGCharacterController.Instance.rpgCharacterEquipmentController
							.GetEquipmentSlotIndexWithType(
								ItemType.Equipment, Equipment.Accessory, accessoryType: accessoryObject.accessoryType);
						break;
				}
			}

			if (index <= -1)
				return;

			playerInventory.SwapItems(selectedInventorySlot, playerEquipmentInventory.Container.Slots[index]);
			ReleaseSlotInfo();
		}

		/// <summary>
		/// Unequips the selected item.
		/// </summary>
		public void UnEquipItem()
		{
			int index = playerInventory.GetNextFreeInventorySlot();

			// Return when there is no free slot in the inventory left
			if (index <= -1)
				return;

			playerInventory.SwapItems(playerInventory.Container.Slots[index], selectedInventorySlot);
			ReleaseSlotInfo();
		}

		/// <summary>
		/// Consumes the selected item.
		/// </summary>
		public void ConsumeItem()
		{
			Debug.Log("[Consume Item] Not yet implemented!");
			ReleaseSlotInfo();
		}

		/// <summary>
		/// Deletes the selected item.
		/// </summary>
		public void DeleteItem()
		{
			selectedInventorySlot.parent.ConfirmRemoveItemPopUp(selectedInventorySlot, inventorySlotInfo);
		}

		/// <summary>
		/// Stores the selected item in the other inventory.
		/// </summary>
		public void StoreItem()
		{
			InventoryObject selectedInventory = selectedInventorySlot.parent.inventory;
			InventoryObject otherInventory = selectedInventory == inventorySlotInfo.userInterface.inventory
				? inventorySlotInfo.otherUserInterface.inventory
				: inventorySlotInfo.userInterface.inventory;

			TransferItem(selectedInventorySlot, otherInventory);
			ReleaseSlotInfo();
		}

		/// <summary>
		/// Retrieves the selected item from the storage box to the player's inventory.
		/// </summary>
		public void RetrieveItem()
		{
			InventoryObject selectedInventory = selectedInventorySlot.parent.inventory;
			InventoryObject otherInventory = selectedInventory == inventorySlotInfo.userInterface.inventory
				? inventorySlotInfo.otherUserInterface.inventory
				: inventorySlotInfo.userInterface.inventory;

			TransferItem(selectedInventorySlot, otherInventory);
			ReleaseSlotInfo();
		}

		/// <summary>
		/// Transfers an item from one inventory slot to another inventory.
		/// </summary>
		/// <param name="sourceSlot">The source inventory slot.</param>
		/// <param name="targetInventory">The target inventory.</param>
		private void TransferItem(InventorySlot sourceSlot, InventoryObject targetInventory)
		{
			Item itemToTransfer = sourceSlot.item;
			int amountToTransfer = sourceSlot.amount;

			// Try adding the item to the target inventory
			int remainingAmount = targetInventory.TryAddItem(itemToTransfer, amountToTransfer);

			if (remainingAmount < amountToTransfer)
			{
				// If items were successfully added to the target inventory, update the current slot
				sourceSlot.UpdateSlot(itemToTransfer, remainingAmount);
				if (remainingAmount == 0)
				{
					// Clear the slot if all items are transferred
					sourceSlot.UpdateSlot(new Item(), 0);
					sourceSlot.parent.inventory.Save();
				}
			}
			else
			{
				Debug.LogWarning("Failed to transfer item. The target inventory might be full.");
			}
		}

		/// <summary>
		/// Releases the inventory slot information and resets the UI.
		/// </summary>
		private void ReleaseSlotInfo()
		{
			ResetSlotOptions();
			inventorySlotInfo.ReleaseInventorySlotInfo();
		}

		/// <summary>
		/// Resets the slot options UI elements to be inactive.
		/// </summary>
		private void ResetSlotOptions()
		{
			equipItemObject.SetActive(false);
			unequipItemObject.SetActive(false);
			deleteItemObject.SetActive(false);
			consumeItemObject.SetActive(false);
			storeItemObject.SetActive(false);
			retrieveItemObject.SetActive(false);
		}
	}
}