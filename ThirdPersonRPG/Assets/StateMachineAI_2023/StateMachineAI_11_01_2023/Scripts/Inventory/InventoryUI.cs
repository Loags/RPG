using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform ItemsParent;
    public InventorySlot[] slots;


    private void OnEnable()
    {
        Inventory.instance.onItemChangedCallback += UpdateUI;
        slots = new InventorySlot[Inventory.instance.inventorySpace];
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.instance.items.Count)
            {
                slots[i].AddItem(Inventory.instance.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void AssignInventorySlot(int _index, InventorySlot _inventorySlot)
    {
        slots[_index] = _inventorySlot;
    }

    private void OnDestroy()
    {
        Inventory.instance.onItemChangedCallback -= UpdateUI;
    }
}
