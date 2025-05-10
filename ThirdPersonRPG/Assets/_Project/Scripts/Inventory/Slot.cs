using UnityEngine;
using UnityEngine.UI;

namespace LB.Inventory
{
    public class Slot<T> : MonoBehaviour
    {

        public Image icon;
        public Button removeButton;
        public Button itemButton;

        [HideInInspector] public T item;

        public virtual void AddItem(T _newItem)
        {
            //item = _newItem;

            //icon.sprite = _newItem.icon;
            icon.enabled = true;
            removeButton.interactable = true;
            itemButton.interactable = true;
        }

        public virtual void ClearSlot()
        {
            //item = null;

            icon.sprite = null;
            icon.enabled = false;
            removeButton.interactable = false;
            itemButton.interactable = false;
        }

        public virtual void UseItem() // On Button Clicked Event --> OptionsPanel
        {
            if (item == null) return;

            //if (item != null)
            //{
            //    item.Use();
            //    DeselectItem();
            //}
        }

        public virtual void SelectItem() // On Button Clicked Event
        {
            DeselectItem();

            if (item == null) return;

            //if (item != null)
            //{
            //    item.Select();
            //}
        }

        public virtual void DeselectItem()
        {
            if (item == null) return;

            //if (item != null)
            //{
            //    item.Deselect();
            //}
        }
    }
}