using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot EquipmentSlot;

    public int DamageModifier;
    public int ArmorModifier;
    public int HealthModifier;

    public override void Use()
    {
        base.Use();

        EquipmentManager.instance.Equip(this);
        RemoveFromInventory();
    }
}

public enum EquipmentSlot { Helmet, Chestplate, Gloves, Legs, Boots, Amulet, Earring, Ring, Weapon, Shield }