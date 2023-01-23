using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
    }

    private void OnEquipmentChanged(Equipment _newItem, Equipment _oldItem)
    {
        if (_newItem != null)
        {
            Armor.AddModifier(_newItem.ArmorModifier);
            Damage.AddModifier(_newItem.DamageModifier);
        }

        if (_oldItem != null)
        {
            Armor.RemoveModifier(_oldItem.ArmorModifier);
            Damage.RemoveModifier(_oldItem.DamageModifier);
        }
    }
}
