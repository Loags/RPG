using System.Collections.Generic;

namespace LB.Inventory
{
    public class EquipmentObject : ItemObject
    {
        public Equipment equipmentType;

        protected override void OnValidate()
        {
            base.OnValidate();
            type = ItemType.Equipment;
        }

        protected virtual void GenerateWeaponBuffs(List<ItemBuff> buffList, ItemTier _tier)
        {
        }

        protected virtual void GenerateArmorBuffs(List<ItemBuff> buffList, ItemTier _tier)
        {
        }

        protected virtual void GenerateAccessoryBuffs(List<ItemBuff> buffList, ItemTier _tier)
        {
        }

        public override List<ItemBuff> GeneratePredefinedBuffs(ItemTier _tier)
        {
            List<ItemBuff> buffList = new List<ItemBuff>();

            switch (equipmentType)
            {
                case Equipment.Weapon:
                    GenerateWeaponBuffs(buffList, _tier);
                    break;
                case Equipment.Armor:
                    GenerateArmorBuffs(buffList, _tier);
                    break;
                case Equipment.Accessory:
                    GenerateAccessoryBuffs(buffList, _tier);
                    break;
            }

            return buffList;
        }

        protected void ApplyBuffs(Dictionary<ItemTier, Dictionary<Attributes, (int Min, int Max)>> _buffRanges,
            List<ItemBuff> _buffList, ItemTier _tier)
        {
            foreach (KeyValuePair<Attributes, (int Min, int Max)> attribute in _buffRanges[_tier])
            {
                _buffList.Add(new ItemBuff(attribute.Value.Min, attribute.Value.Max)
                {
                    attribute = attribute.Key
                });
            }
        }
    }
}