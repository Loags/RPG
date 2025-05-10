using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace LB.Inventory
{
    public abstract class UserInterface : MonoBehaviour
    {
        public InventoryObject inventory;
        protected Dictionary<GameObject, InventorySlot> slotsOnInterface = new();

        [SerializeField] private ConfirmRemoveItemPopUP confirmRemoveItemPopUp;
        [SerializeField] private InventorySlotInfo inventorySlotInfo;

        #region Double Click Logic

        // The time the last click on a slot has been performed
        private float lastClickTime;

        // Adjust this threshold as needed between first and second click
        private const float doubleClickThreshold = 0.2f;

        // Store the last clicked slot
        private GameObject lastClickedSlot;

        #endregion

        private void Start()
        {
            inventory.StartBatchSave();
            for (int i = 0; i < inventory.GetSlots.Length; i++)
            {
                inventory.GetSlots[i].parent = this;
                inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
            }

            CreateSlots();
            AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
            AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });

            foreach (InventorySlot slot in inventory.GetSlots)
                OnSlotUpdate(slot);
            inventory.EndBatchSave();
        }

        protected abstract void CreateSlots();

        private void OnSlotUpdate(InventorySlot _slot)
        {
            if (_slot.item.Id >= 0)
            {
                if (InterfaceType.Equipment == inventory.type)
                {
                    _slot.slotDisplay.transform.GetChild(0).GetComponent<Image>().enabled = false;
                }

                UpdateSlotDisplayColor(_slot.slotDisplay.GetComponent<Image>(), _slot.item);
                _slot.slotDisplay.transform.GetChild(1).GetComponentInChildren<Image>().sprite = _slot.ItemObject.icon;
                _slot.slotDisplay.transform.GetChild(1).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.slotDisplay.GetComponentInChildren<TMP_Text>().text =
                    _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
            }
            else
            {
                if (InterfaceType.Equipment == inventory.type)
                {
                    _slot.slotDisplay.transform.GetChild(0).gameObject.SetActive(true);
                    _slot.slotDisplay.transform.GetChild(0).GetComponent<Image>().enabled = true;
                }

                UpdateSlotDisplayColor(_slot.slotDisplay.GetComponent<Image>(), _slot.item);
                _slot.slotDisplay.transform.GetChild(1).GetComponentInChildren<Image>().sprite = null;
                _slot.slotDisplay.transform.GetChild(1).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.slotDisplay.GetComponentInChildren<TMP_Text>().text = "";
            }
        }

        private void UpdateSlotDisplayColor(Image _image, Item _item)
        {
            _image.color = _item == null
                ? ItemObjectExtension.Colors.DefaultBackground
                : ItemObjectExtension.Colors.GetColorForTier(_item.tier);
        }


        protected void AddEvent(GameObject _gameObject, EventTriggerType _type, UnityAction<BaseEventData> _action)
        {
            EventTrigger trigger = _gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = _type;
            eventTrigger.callback.AddListener(_action);
            trigger.triggers.Add(eventTrigger);
        }

        protected void OnSlotClick(GameObject _slotObject)
        {
            // Clicked on an empty slot
            if (slotsOnInterface[_slotObject].item.Id <= -1) return;

            float currentTime = Time.time;
            float timeSinceLastClick = currentTime - lastClickTime;

            if (lastClickedSlot == _slotObject && timeSinceLastClick <= doubleClickThreshold)
            {
                // Double-click logic here
                if (inventorySlotInfo != null)
                {
                    switch (inventory.type)
                    {
                        case InterfaceType.Inventory:
                            switch (StateManager.Instance.CurrentFlowState)
                            {
                                case AppStates.FlowStates.INVENTORY:
                                    inventorySlotInfo.inventorySlotOptionPanel.EquipItem();
                                    break;
                                case AppStates.FlowStates.STORAGEBOX:
                                    inventorySlotInfo.inventorySlotOptionPanel.StoreItem();
                                    break;
                            }

                            break;
                        case InterfaceType.Equipment:
                        {
                            if (StateManager.Instance.CurrentFlowState == AppStates.FlowStates.INVENTORY)
                            {
                                inventorySlotInfo.inventorySlotOptionPanel.UnEquipItem();
                            }

                            break;
                        }
                        case InterfaceType.StorageBox:
                            switch (StateManager.Instance.CurrentFlowState)
                            {
                                case AppStates.FlowStates.STORAGEBOX:
                                    inventorySlotInfo.inventorySlotOptionPanel.RetrieveItem();
                                    break;
                            }

                            break;
                    }
                }
            }
            else
            {
                // Single-click logic here
                if (inventorySlotInfo != null)
                {
                    inventorySlotInfo.Initialize(slotsOnInterface[_slotObject].ItemObject,
                        slotsOnInterface[_slotObject]);
                }
            }

            // Update last clicked slot and lastClickTime
            lastClickedSlot = _slotObject;
            lastClickTime = currentTime;
        }

        protected void OnEnter(GameObject _slotObject)
        {
            MouseData.slotHoveredOver = _slotObject;
        }

        protected void OnExit(GameObject _slotObject)
        {
            MouseData.slotHoveredOver = null;
        }

        private void OnExitInterface(GameObject _slotObject)
        {
            MouseData.interfaceMouseIsOver = null;
            MouseData.interfaceMouseIsOverAtBeginDrag = null;
            MouseData.interfaceMouseIsOverAtEndDrag = null;
        }

        private void OnEnterInterface(GameObject _slotObject)
        {
            MouseData.interfaceMouseIsOver = _slotObject.GetComponent<UserInterface>();
        }

        protected void OnDragStart(GameObject _slotObject)
        {
            MouseData.tempItemBeingDragged = CreateTempItem(_slotObject);
            MouseData.interfaceMouseIsOverAtBeginDrag = _slotObject.GetComponent<UserInterface>();
            inventorySlotInfo.ReleaseInventorySlotInfo();
        }

        private GameObject CreateTempItem(GameObject _slotObject)
        {
            GameObject tempItem;
            if (slotsOnInterface[_slotObject].item.Id >= 0)
            {
                tempItem = new GameObject();
                RectTransform rt = tempItem.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(50, 50);
                tempItem.transform.SetParent(transform.parent);
                Image image = tempItem.AddComponent<Image>();
                image.sprite = slotsOnInterface[_slotObject].ItemObject.icon;
                image.raycastTarget = false;
                return tempItem;
            }

            return null;
        }


        /// <summary>
        /// Handles the event when an item is not being dragged anymore
        /// </summary>
        /// <param name="_slotObject"> The items GameObject which was dragged </param>
        protected void OnDragEnd(GameObject _slotObject)
        {
            bool preventEmptySlotDrag = MouseData.tempItemBeingDragged == null;
            Destroy(MouseData.tempItemBeingDragged);

            if (preventEmptySlotDrag) return;

            if (MouseData.interfaceMouseIsOver == null)
            {
                // Remove the Item from the Inventory when it is dropped outside the inventory ui
                ConfirmRemoveItemPopUp(slotsOnInterface[_slotObject]);
                return;
            }

            if (MouseData.slotHoveredOver)
            {
                // Check if there is another item in the slot where it should be dropped
                // Swap items even though it is an empty slot
                InventorySlot mouseHoverSlotData =
                    MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
                inventory.SwapItems(slotsOnInterface[_slotObject], mouseHoverSlotData);
            }
        }

        // Spawns in the confirm popup to delete an item
        public void ConfirmRemoveItemPopUp(InventorySlot _inventorySlot, InventorySlotInfo _inventorySlotInfo = null)
        {
            confirmRemoveItemPopUp.GetItemData(_inventorySlot, _inventorySlotInfo);
        }

        protected void OnDrag(GameObject _slotObject)
        {
            if (MouseData.tempItemBeingDragged != null)
            {
                Vector2 mousePos2d = Mouse.current.position.ReadValue();
                Vector3 mousePos3d = new Vector3(mousePos2d.x, mousePos2d.y, 0);
                MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = mousePos3d;
            }
        }
    }

    public static class MouseData
    {
        public static UserInterface interfaceMouseIsOver;
        public static UserInterface interfaceMouseIsOverAtBeginDrag;
        public static UserInterface interfaceMouseIsOverAtEndDrag;
        public static GameObject tempItemBeingDragged;
        public static GameObject slotHoveredOver;
    }


    public static class ExtensionMethods
    {
        public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
        {
            foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
            {
                if (_slot.Value.item.Id >= 0)
                {
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite =
                        _slot.Value.ItemObject.icon;
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                    _slot.Key.GetComponentInChildren<TMP_Text>().text =
                        _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
                }
                else
                {
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                    _slot.Key.GetComponentInChildren<TMP_Text>().text = "";
                }
            }
        }
    }
}