using UnityEngine;
using UnityEngine.EventSystems;

namespace LB.Inventory
{
    public class DynamicInterface : UserInterface
    {
        public GameObject inventoryPrefab;
        [SerializeField] private Transform slotsAnchor;

        protected override void CreateSlots()
        {
            slotsOnInterface = new();
            for (int i = 0; i < inventory.GetSlots.Length; i++)
            {
                GameObject obj = Instantiate(inventoryPrefab, slotsAnchor);

                AddEvent(obj, EventTriggerType.PointerClick, delegate { OnSlotClick(obj); });
                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
                AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

                inventory.GetSlots[i].slotDisplay = obj;

                slotsOnInterface.Add(obj, inventory.GetSlots[i]);
            }
        }
    }
}