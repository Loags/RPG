using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{

    public static EquipmentManager instance;

    public delegate void OnEquipmentChanged(Equipment _newItem, Equipment _oldItem);
    public OnEquipmentChanged onEquipmentChanged;


    [Header("Equipment")]
    [SerializeField] private Equipment[] currentEquipment;


    private Inventory inventory;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
        inventory = Inventory.instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            UnequipAll();
    }    

    public void Equip(Equipment _newItem)
    {
        int slotIndex = (int)_newItem.EquipmentSlot;

        Equipment oldItem = null;

        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        if (onEquipmentChanged != null)
            onEquipmentChanged(_newItem, oldItem);

        currentEquipment[slotIndex] = _newItem;
    }

    public void Unequip(int _slotIndex)
    {
        if (currentEquipment[_slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[_slotIndex];
            inventory.Add(oldItem);

            if (onEquipmentChanged != null)
                onEquipmentChanged(null, oldItem);

            currentEquipment[_slotIndex] = null;
        }
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }
}
