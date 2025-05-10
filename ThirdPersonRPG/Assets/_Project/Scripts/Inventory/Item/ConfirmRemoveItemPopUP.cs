using TMPro;
using UnityEngine;

namespace LB.Inventory
{
    public class ConfirmRemoveItemPopUP : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameDisplay;
        [SerializeField] private GameObject containerDisplay;

        private InventorySlot currentItemSlot;
        private InventorySlotInfo inventorySlotInfo;

        public void GetItemData(InventorySlot _inventorySlot, InventorySlotInfo _inventorySlotInfo = null)
        {
            currentItemSlot = _inventorySlot;
            containerDisplay.SetActive(true);
            nameDisplay.text = currentItemSlot.item.name;

            if (_inventorySlotInfo != null)
                inventorySlotInfo = _inventorySlotInfo;
        }

        public void RemoveItem()
        {
            currentItemSlot.RemoveItem();
            containerDisplay.SetActive(false);

            if (inventorySlotInfo != null)
                inventorySlotInfo.ReleaseInventorySlotInfo();
        }

        public void CancelRemoveItem()
        {
            containerDisplay.SetActive(false);
        }
    }
}