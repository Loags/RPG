using LB.Inventory;
using LB.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LB.Interactable
{
    public class ItemPickup : Interactable, IPooledObject
    {
        /// <summary>
        /// Whether this item was placed manually in the scene
        /// </summary>
        [SerializeField] private bool placedByHand;
        
        /// <summary>
        /// List of item objects that this pickup contains
        /// </summary>
        [SerializeField] private List<ItemObject> itemObjects;
        
        /// <summary>
        /// Priority settings for this item pickup
        /// </summary>
        [Header("Interaction Settings")]
        [SerializeField] [Tooltip("Lower priority than storage boxes but higher than basic interactables")] 
        private int itemPickupPriority = 5;

        /// <summary>
        /// Items contained in this pickup
        /// </summary>
        [SerializeField][ReadOnly] private List<Item> items = new();
        
        /// <summary>
        /// Gets the priority for this item pickup
        /// </summary>
        public override int InteractionPriority => itemPickupPriority;

        /// <summary>
        /// Handles player interaction with the item pickup
        /// </summary>
        /// <param name="_rpgCharacterController">Reference to the player's character controller</param>
        public override void Interact(RPGCharacterController _rpgCharacterController)
        {
            base.Interact(_rpgCharacterController);

            if (items.Count > 0)
            {
                List<Item> itemsToRemove = new List<Item>();

                foreach (Item item in items)
                {
                    if (_rpgCharacterController.rpgCharacterInventoryController.inventory.AddItem(item, 1))
                    {
                        itemsToRemove.Add(item);
                    }
                    else
                    {
                        Debug.Log("Inventory of the player was full. Cancel adding items on pickup!");
                    }
                }

                foreach (Item item in itemsToRemove)
                {
                    RemoveItem(item);
                }

                if (items.Count == 0)
                {
                    ObjectPoolerManager.Instance.ReturnToPool(ObjectPoolIDs.INTERACTABLE_ITEM_PICKUP, this);
                }
            }
        }

        /// <summary>
        /// Initializes the item pickup and loads its items
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            if (placedByHand)
            {
                items.AddRange(itemObjects.Select(x => x.CreateItemWithTier(ItemTier.Mythical)));
            }
        }

        /// <summary>
        /// Deactivates the item pickup and clears its items
        /// </summary>
        protected override void Deactivate()
        {
            base.Deactivate();
            items = null;
        }

        /// <summary>
        /// Called when the object is spawned from the pool
        /// </summary>
        public void OnObjectSpawnFromPool()
        {
            Initialize();
        }

        /// <summary>
        /// Called when the object is returned to the pool
        /// </summary>
        public void OnObjectReturnToPool()
        {
            Deactivate();
        }

        /// <summary>
        /// Adds an item to this pickup
        /// </summary>
        /// <param name="_item">Item to add</param>
        public void AddItem(Item _item)
        {
            items.Add(_item);
        }

        /// <summary>
        /// Removes an item from this pickup
        /// </summary>
        /// <param name="_item">Item to remove</param>
        private void RemoveItem(Item _item)
        {
            items.Remove(_item);
        }

        /// <summary>
        /// Combines the items from another ItemPickup into this one.
        /// </summary>
        /// <param name="otherPickup">The other ItemPickup to merge with.</param>
        public void CombineWith(ItemPickup otherPickup)
        {
            if (otherPickup == null) return;

            foreach (Item item in otherPickup.items)
            {
                AddItem(item);
            }

            ObjectPoolerManager.Instance.ReturnToPool(ObjectPoolIDs.INTERACTABLE_ITEM_PICKUP, otherPickup);
        }
    }
}