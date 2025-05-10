using System.Collections.Generic;
using UnityEngine;

namespace LB.Inventory
{
    [CreateAssetMenu(fileName = "New Weapon Object", menuName = "Inventory System/Items/Weapon")]
    public class WeaponObject : EquipmentObject
    {
        public Weapon weaponType;

        protected override void OnValidate()
        {
            base.OnValidate();
            equipmentType = Equipment.Weapon;
        }

        protected override void GenerateWeaponBuffs(List<ItemBuff> buffList, ItemTier _tier)
        {
            if(weaponType == Weapon.MeleeWeapon)
            {
                 ApplyBuffs(BuffRanges.MeleeWeaponBuffs, buffList, _tier);
            }
            else if (weaponType != Weapon.Unarmed && weaponType != Weapon.NONE)
            {
                Debug.LogWarning($"GenerateWeaponBuffs called with unexpected weaponType: {weaponType} in {this.name}");
            }
        }
    }
}