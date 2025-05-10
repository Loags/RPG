using System.Collections.Generic;
using UnityEngine;

namespace LB.Inventory
{
    [CreateAssetMenu(fileName = "New Armor Object", menuName = "Inventory System/Items/Armor")]
    public class ArmorObject : EquipmentObject
    {
        public Armor armorType;

        protected override void OnValidate()
        {
            base.OnValidate();
            equipmentType = Equipment.Armor;
        }

        protected override void GenerateArmorBuffs(List<ItemBuff> buffList, ItemTier _tier)
        {
            switch (armorType)
            {
                case Armor.Helmet:
                    ApplyBuffs(BuffRanges.HelmetBuffs, buffList, _tier);
                    break;
                case Armor.Chest:
                    ApplyBuffs(BuffRanges.ChestBuffs, buffList, _tier);
                    break;
                case Armor.Gloves:
                    ApplyBuffs(BuffRanges.GlovesBuffs, buffList, _tier);
                    break;
                case Armor.Legs:
                    ApplyBuffs(BuffRanges.LegsBuffs, buffList, _tier);
                    break;
                case Armor.Boots:
                    ApplyBuffs(BuffRanges.BootsBuffs, buffList, _tier);
                    break;
            }
        }
    }
}