using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LB.Interactable
{
    /// <summary>
    /// Manages item pickups, allowing them to merge when within a certain range.
    /// </summary>
    public class ItemPickUpManager : Singleton<ItemPickUpManager>
    {
        /// <summary>
        /// The maximum distance at which item pickups can merge.
        /// </summary>
        [SerializeField] private float combineRange = 5f;

        /// <summary>
        /// Attempts to merge nearby item pickups. The item with the most neighbors merges first.
        /// </summary>
        public void CombineClosestItemPickUps()
        {
            object poolObject = ObjectPoolerManager.Instance.GetPoolByID(ObjectPoolIDs.INTERACTABLE_ITEM_PICKUP);
            if (poolObject == null)
            {
                Debug.LogError("ItemPickup Pool not found!");
                return;
            }

            Pool<ItemPickup> itemPickupPool = poolObject as Pool<ItemPickup>;
            if (itemPickupPool == null)
            {
                Debug.LogError("ItemPickup Pool is not of type Pool<ItemPickup>!");
                return;
            }

            List<ItemPickup> itemPickUps = itemPickupPool.GetActiveObjects();

            if (itemPickUps.Count == 0) return;

            HashSet<ItemPickup> itemsToCombine = new HashSet<ItemPickup>();

            foreach (ItemPickup item in itemPickUps)
            {
                if (itemsToCombine.Contains(item)) continue;

                List<ItemPickup> neighbors = itemPickUps
                    .Where(other => other != item && Vector3.Distance(item.transform.position, other.transform.position) <= combineRange)
                    .ToList();

                if (neighbors.Count > 0)
                {
                    itemsToCombine.Add(item);
                    itemsToCombine.UnionWith(neighbors);
                }
            }

            if (itemsToCombine.Count == 0) return;


            HashSet<ItemPickup> mergedItems = new HashSet<ItemPickup>();

            foreach (var entry in itemsToCombine
                .Select(item => new
                {
                    Item = item,
                    NeighborCount = itemPickUps.Count(other => other != item && Vector3.Distance(item.transform.position, other.transform.position) <= combineRange)
                })
                .OrderByDescending(x => x.NeighborCount)
                .ToList())
            {
                ItemPickup mainItem = entry.Item;

                if (mergedItems.Contains(mainItem)) continue;

                List<ItemPickup> nearbyItems = itemPickUps
                    .Where(other => other != mainItem && Vector3.Distance(mainItem.transform.position, other.transform.position) <= combineRange)
                    .ToList();

                foreach (ItemPickup item in nearbyItems.Where(item => !mergedItems.Contains(item)))
                {
                    mainItem.CombineWith(item);
                    ObjectPoolerManager.Instance.ReturnToPool(ObjectPoolIDs.INTERACTABLE_ITEM_PICKUP, item);
                    mergedItems.Add(item);
                }
            }
        }

    }
}
