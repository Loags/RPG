using LB.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace LB
{
    public abstract class ItemObject : ScriptableObject
    {
        public Sprite icon;
        public GameObject characterDisplay;
        public bool stackable;
        public int stackAmount = 1;
        [TextArea(15, 20)] public string description;
        public Item data = new Item();
        public List<string> boneNames = new();
        public ItemType type;

        public Item CreateItem()
        {
            Item newItem = new Item(this);
            return newItem;
        }

        public Item CreateItemWithTier(ItemTier _tier)
        {
            Item newItem = new Item(this)
            {
                tier = _tier,
                buffs = GeneratePredefinedBuffs(_tier)
            };

            return newItem;
        }

        protected virtual void OnValidate()
        {
            ItemDatabaseObject.Instance.UpdateID();

            boneNames.Clear();
            if (characterDisplay == null)
                return;
            if (!characterDisplay.GetComponent<SkinnedMeshRenderer>())
                return;

            SkinnedMeshRenderer renderer = characterDisplay.GetComponent<SkinnedMeshRenderer>();
            Transform[] bones = renderer.bones;
            foreach (Transform transform in bones)
            {
                boneNames.Add(transform.name);
            }
        }

        public virtual List<ItemBuff> GeneratePredefinedBuffs(ItemTier _tier)
        {
            return new List<ItemBuff>();
        }
    }
}