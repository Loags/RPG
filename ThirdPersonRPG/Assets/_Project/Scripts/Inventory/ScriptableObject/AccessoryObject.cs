using System.Collections.Generic;
using UnityEngine;

namespace LB.Inventory
{
    [CreateAssetMenu(fileName = "New Accessory Object", menuName = "Inventory System/Items/Accessory")]
    public class AccessoryObject : EquipmentObject
    {
        public Accessory accessoryType;

        protected override void OnValidate()
        {
            base.OnValidate();
            equipmentType = Equipment.Accessory;
        }

        protected override void GenerateAccessoryBuffs(List<ItemBuff> buffList, ItemTier _tier)
        {
            switch (accessoryType)
            {
                case Accessory.Ring:
                    ApplyBuffs(BuffRanges.RingBuffs, buffList, _tier);
                    break;
                case Accessory.Earring:
                    ApplyBuffs(BuffRanges.EarringBuffs, buffList, _tier);
                    break;
                case Accessory.Necklace:
                    ApplyBuffs(BuffRanges.NecklaceBuffs, buffList, _tier);
                    break;
            }
        }
    }
}