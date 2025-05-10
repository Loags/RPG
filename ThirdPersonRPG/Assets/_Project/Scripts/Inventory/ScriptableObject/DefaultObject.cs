using UnityEngine;

namespace LB.Inventory
{
    [CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]
    public class DefaultObject : ItemObject
    {
        protected override void OnValidate()
        {
            base.OnValidate();
            type = ItemType.Default;
        }
    }
}