using UnityEngine;

namespace LB.Inventory
{
    [CreateAssetMenu(fileName = "New Consumable Object", menuName = "Inventory System/Items/Consumable")]
    public class ConsumableObject : ItemObject
    {
        public Consumable consumableType;

        protected override void OnValidate()
        {
            base.OnValidate();
            type = ItemType.Consumable;
        }
    }
}