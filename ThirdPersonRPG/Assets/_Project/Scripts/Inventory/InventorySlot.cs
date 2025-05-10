using LB.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LB
{
	[Serializable]
	public class InventorySlot // What Item is in the Slot and what Item is allowed to be in the Slot
	{
		[SerializeField] public List<AllowedItem> AllowedItems = new();

		[NonSerialized] public UserInterface parent;
		[NonSerialized] public GameObject slotDisplay;
		[NonSerialized] public SlotUpdated OnAfterUpdate;
		[NonSerialized] public SlotUpdated OnBeforeUpdate;

		public Item item;
		public int amount;

		public InventorySlot()
		{
			UpdateSlot(new Item(), 0);
		}

		public InventorySlot(Item _item, int _amount)
		{
			UpdateSlot(_item, _amount);
		}

		public void UpdateSlot(Item _item, int _amount)
		{
			if ((OnBeforeUpdate != null && StateManager.Instance.CurrentFlowState == AppStates.FlowStates.INVENTORY) ||
			    (OnBeforeUpdate != null && StateManager.Instance.CurrentFlowState == AppStates.FlowStates.STORAGEBOX))
				OnBeforeUpdate.Invoke(this);

			this.item = _item;
			this.amount = _amount;

			if ((OnAfterUpdate != null && StateManager.Instance.CurrentFlowState == AppStates.FlowStates.INVENTORY) ||
			    (OnAfterUpdate != null && StateManager.Instance.CurrentFlowState == AppStates.FlowStates.STORAGEBOX))
				OnAfterUpdate.Invoke(this);
		}

		public void RemoveItem()
		{
			parent.inventory.RemoveItem(item);
		}

		public void AddAmount(int _value)
		{
			UpdateSlot(item, amount += _value);
		}

		public ItemObject ItemObject
		{
			get
			{
				if (item.Id >= 0)
				{
					return parent.inventory.database.ItemObjects[item.Id];
				}

				return null;
			}
		}

		public bool CanPlaceInSlot(ItemObject _itemObject)
		{
			if (AllowedItems.Count <= 0 || _itemObject == null || _itemObject.data.Id < 0)
				return true;

			foreach (AllowedItem allowedItem in AllowedItems)
			{
				if (_itemObject.type == allowedItem.itemType)
				{
					if (_itemObject is EquipmentObject equipmentObject)
					{
						switch (allowedItem.slotType)
						{
							case SlotType.Helmet or SlotType.Chest or SlotType.Gloves
								or SlotType.Legs or SlotType.Boots:
							{
								if (equipmentObject is ArmorObject armorObject)
								{
									if (armorObject.armorType == allowedItem.armorType)
										return true;
								}

								break;
							}
							case SlotType.Necklace or SlotType.Ring or SlotType.Earring:
							{
								if (equipmentObject is AccessoryObject accessoryObject)
								{
									if (accessoryObject.accessoryType == allowedItem.accessoryType)
										return true;
								}

								break;
							}
							case SlotType.Weapon or SlotType.Shield:
							{
								if (equipmentObject is WeaponObject weaponObject)
								{
									if (weaponObject.weaponType == allowedItem.weaponType)
										return true;
								}

								break;
							}
						}
					}
					else
					{
						// If the item is not equipment but matches the item type
						return true;
					}
				}
			}

			return false;
		}
	}
}