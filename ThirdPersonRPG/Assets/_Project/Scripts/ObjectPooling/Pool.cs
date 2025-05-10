using LB.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace LB
{
    [System.Serializable]
    public enum ObjectPoolIDs
    {
        NONE,
        INTERACTABLE_ITEM_PICKUP
    }

    [System.Serializable]
    public class PoolDefinition
    {
        /// <summary>
        /// The unique identifier for the pool.
        /// </summary>
        public ObjectPoolIDs tag;

        /// <summary>
        /// The prefab that will be instantiated and pooled.
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// The initial size of the pool.
        /// </summary>
        public int size;

        /// <summary>
        /// The name of the required script component (must match the class name exactly).
        /// </summary>
        public string scriptTypeName;

    }

    /// <summary>
    /// A generic object pool that manages reusable instances of a specified MonoBehaviour type.
    /// </summary>
    /// <typeparam name="T">The type of MonoBehaviour to pool.</typeparam>
    public class Pool<T> where T : MonoBehaviour
    {
        /// <summary>
        /// The queue holding pooled objects.
        /// </summary>
        private Queue<T> objectQueue;

        /// <summary>
        /// The prefab used to instantiate new objects.
        /// </summary>
        private GameObject prefab;

        /// <summary>
        /// The parent transform under which pooled objects are stored.
        /// </summary>
        private Transform parent;

        /// <summary>
        /// The original size of the pool before any expansions.
        /// </summary>
        private int originalSize;

        /// <summary>
        /// The current size of the pool.
        /// </summary>
        private int size;

        /// <summary>
        /// The list of currently active objects.
        /// </summary>
        private List<T> activeObjects = new List<T>();


        /// <summary>
        /// Initializes a new instance of the pool with a specified prefab and size.
        /// </summary>
        /// <param name="prefab">The prefab to pool.</param>
        /// <param name="initialSize">The initial number of objects in the pool.</param>
        /// <param name="parent">The parent transform for organization in the hierarchy.</param>
        public Pool(GameObject prefab, int initialSize, Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError($"Pool<{typeof(T)}> received a null prefab! Check the ObjectPooler setup.");
                return;
            }

            this.prefab = prefab;
            this.size = initialSize;
            this.originalSize = initialSize;
            this.parent = parent;

            objectQueue = new Queue<T>();

            for (int i = 0; i < initialSize; i++)
            {
                AddObjectToPool();
            }
        }

        /// <summary>
        /// Instantiates a new object, verifies it has the required component, and adds it to the pool.
        /// </summary>
        private void AddObjectToPool()
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            if (parent != null)
                obj.transform.SetParent(parent);

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Prefab {prefab.name} does not contain a required component of type {typeof(T)}!");
                Object.Destroy(obj);
                return;
            }

            objectQueue.Enqueue(component);
        }

        /// <summary>
        /// Retrieves an active object from the pool, activating and positioning it.
        /// </summary>
        /// <param name="position">The position to place the object.</param>
        /// <param name="rotation">The rotation to apply to the object.</param>
        /// <returns>A pooled object of type T.</returns>
        public T GetObject(Vector3 position, Quaternion rotation)
        {
            if (objectQueue.Count == 0)
            {
                ExpandPool();
            }

            T obj = objectQueue.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.gameObject.SetActive(true);

            activeObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Returns an object to the pool, deactivating and removing it from the active list.
        /// </summary>
        /// <param name="obj">The object to return.</param>
        public void ReturnObject(T obj)
        {
            obj.gameObject.SetActive(false);
            objectQueue.Enqueue(obj);

            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
            }
        }

        /// <summary>
        /// Returns a list of all currently active objects.
        /// </summary>
        public List<T> GetActiveObjects()
        {
            return new List<T>(activeObjects);
        }

        /// <summary>
        /// Expands the pool by doubling its current size.
        /// </summary>
        private void ExpandPool()
        {
            int expandAmount = size;
            size *= 2;
            for (int i = 0; i < expandAmount; i++)
            {
                AddObjectToPool();
            }
        }
    }
}
