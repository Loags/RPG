using System;
using System.Collections.Generic;

namespace LB.Inventory
{
    [Serializable]
    public class Item
    {
        public string name;
        public int Id = -1;
        public List<ItemBuff> buffs = new();
        public ItemTier tier = ItemTier.NONE;

        public Item()
        {
            name = "";
            Id = -1;
        }

        public Item(ItemObject _item)
        {
            this.name = _item.name;
            this.Id = _item.data.Id;
        }
    }
}