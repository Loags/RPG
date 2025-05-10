using System.Collections.Generic;
using UnityEngine;

namespace LB.Inventory
{
    public class InventorySlotInfo : MonoBehaviour
    {
        public UserInterface userInterface;
        public UserInterface otherUserInterface;

        [SerializeField] private GameObject inventorySlotDescriptionPrefab;
        [SerializeField] private GameObject inventorySlotOptionPanelPrefab;

        [SerializeField] private Transform inventorySlotParentTarget;

        [HideInInspector] public InventorySlotOptionPanel inventorySlotOptionPanel;

        private MainCanvasUIItem mainCanvasUIItem;

        private void OnEnable()
        {
            mainCanvasUIItem = GetComponent<MainCanvasUIItem>();
        }

        public void Initialize(ItemObject _itemObject, InventorySlot _inventorySlot)
        {
            mainCanvasUIItem.ShowItem(true);
            InitializeSlotDescriptions(_itemObject, _inventorySlot);
            InitializeSlotOptions(_itemObject, _inventorySlot);
        }

        public void ReleaseInventorySlotInfo()
        {
            mainCanvasUIItem.ShowItem(false);
        }

        private void InitializeSlotDescriptions(ItemObject _itemObject, InventorySlot _inventorySlot)
        {
            // Clear existing slot descriptions if needed
            ClearSlotDescriptions();

            List<string> data = ExtractItemData(_itemObject, _inventorySlot);

            for (int i = 0; i < data.Count; i += 2)
            {
                // Ensure there are enough elements in data
                if (i + 1 < data.Count)
                {
                    string header = data[i];
                    string content = data[i + 1];

                    GameObject spawnedDescriptionSlot =
                        Instantiate(inventorySlotDescriptionPrefab, inventorySlotParentTarget);
                    InventorySlotDescription inventorySlotDescription =
                        spawnedDescriptionSlot.GetComponent<InventorySlotDescription>();
                    inventorySlotDescription.Initialize(header, content);
                }
            }
        }

        private void ClearSlotDescriptions()
        {
            int childCount = inventorySlotParentTarget.childCount;

            // Start from the second child (index 1) to ignore the first child
            for (int i = 1; i < childCount; i++)
            {
                Transform child = inventorySlotParentTarget.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        private List<string> ExtractItemData(ItemObject _itemObject, InventorySlot _inventorySlot)
        {
            List<string> extractedItemData = new();

            if (_itemObject.data.name != string.Empty)
                extractedItemData.Add(_itemObject.data.name);
            else
                extractedItemData.Add("Item Name");

            if (_itemObject.description != string.Empty)
                extractedItemData.Add(_itemObject.description);
            else
                extractedItemData.Add("Item Description");

            if (_inventorySlot.item.tier != ItemTier.NONE)
            {
                extractedItemData.Add(string.Empty);
                extractedItemData.Add(_inventorySlot.item.tier.ToString());
            }

            if (_inventorySlot.item.buffs.Count > 0)
            {
                foreach (ItemBuff itemBuff in _inventorySlot.item.buffs)
                {
                    extractedItemData.Add(itemBuff.attribute.ToString());
                    extractedItemData.Add(itemBuff.value.ToString());
                }
            }

            // Debug when the extracted item data list is not filled with info
            // Item was not set up properly!
            if (extractedItemData.Count <= 0)
                Debug.LogError(
                    $"Extracted item data for InventorySlotInfo could not been found!+\n Check item: {_itemObject.name}!");

            return extractedItemData;
        }

        private void InitializeSlotOptions(ItemObject _itemObject, InventorySlot _inventorySlot)
        {
            GameObject spawnedOptionPanel = Instantiate(inventorySlotOptionPanelPrefab, inventorySlotParentTarget);
            inventorySlotOptionPanel = spawnedOptionPanel.GetComponent<InventorySlotOptionPanel>();
            inventorySlotOptionPanel.Initialize(_itemObject, _inventorySlot, this);
        }
    }
}