using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LB
{
    /// <summary>
    /// A centralized object pooling system that manages different object pools.
    /// </summary>
    public class ObjectPoolerManager : Singleton<ObjectPoolerManager>
    {
        /// <summary>
        /// List of pool definitions configured in the Unity Inspector.
        /// </summary>
        [SerializeField] private List<PoolDefinition> pools;

        /// <summary>
        /// Dictionary storing the different pools by ObjectPoolIDs.
        /// </summary>
        private Dictionary<ObjectPoolIDs, object> poolDictionary;

        /// <summary>
        /// Initializes the object pools at the start of the game.
        /// </summary>
        void Start()
        {
            poolDictionary = new Dictionary<ObjectPoolIDs, object>();

            foreach (PoolDefinition pool in pools)
            {
                CreatePool(pool);
            }
        }

        /// <summary>
        /// Creates a pool using the prefab and script name from the pool definition.
        /// </summary>
        /// <param name="pool">The pool definition containing prefab, size, and script name.</param>
        private void CreatePool(PoolDefinition pool)
        {
            if (pool.prefab == null)
            {
                Debug.LogError($"Pool {pool.tag} has a null prefab! Check ObjectPooler Inspector.");
                return;
            }

            if (string.IsNullOrEmpty(pool.scriptTypeName))
            {
                Debug.LogError($"Pool {pool.tag} is missing a script type name! Set the correct class name in the Inspector.");
                return;
            }

            Type scriptType = FindTypeInAssemblies(pool.scriptTypeName);
            if (scriptType == null || !typeof(MonoBehaviour).IsAssignableFrom(scriptType))
            {
                Debug.LogError($"Invalid script type '{pool.scriptTypeName}' for pool {pool.tag}. Ensure the name is correct and the script inherits from MonoBehaviour.");
                return;
            }

            if (pool.prefab.GetComponent(scriptType) == null)
            {
                Debug.LogError($"Prefab for pool {pool.tag} does not have the required script component: {scriptType.Name}");
                return;
            }

            Transform parentObject = new GameObject(pool.tag.ToString() + " Pool").transform;

            Type poolType = typeof(Pool<>).MakeGenericType(scriptType);
            object newPool = Activator.CreateInstance(poolType, pool.prefab, pool.size, parentObject);

            poolDictionary.Add(pool.tag, newPool);
        }

        /// <summary>
        /// Retrieves an object from the specified pool.
        /// </summary>
        /// <typeparam name="T">The expected component type.</typeparam>
        /// <param name="tag">The pool tag.</param>
        /// <param name="position">The spawn position.</param>
        /// <param name="rotation">The spawn rotation.</param>
        /// <returns>A pooled object of type T.</returns>
        public T SpawnFromPool<T>(ObjectPoolIDs tag, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        {
            if (poolDictionary.TryGetValue(tag, out object poolObject))
            {
                Pool<T> pool = poolObject as Pool<T>;
                if (pool == null)
                {
                    Debug.LogError($"Pool {tag} does not contain objects of type {typeof(T)}!");
                    return null;
                }
                return pool.GetObject(position, rotation);
            }

            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        /// <summary>
        /// Returns an object back to its pool.
        /// </summary>
        /// <typeparam name="T">The type of object being returned.</typeparam>
        /// <param name="tag">The pool tag.</param>
        /// <param name="obj">The object to return.</param>
        public void ReturnToPool<T>(ObjectPoolIDs tag, T obj) where T : MonoBehaviour
        {
            if (poolDictionary.TryGetValue(tag, out object poolObject))
            {
                Pool<T> pool = poolObject as Pool<T>;
                if (pool == null)
                {
                    Debug.LogError($"Pool {tag} does not contain objects of type {typeof(T)}!");
                    return;
                }
                pool.ReturnObject(obj);
            }
            else
            {
                Debug.LogWarning($"Pool with tag {tag} not found.");
            }
        }

        /// <summary>
        /// Retrieves a pool by its ID.
        /// </summary>
        /// <param name="id">The ID of the pool to retrieve.</param>
        /// <returns>The pool with the specified ID, or null if not found.</returns>
        public object GetPoolByID(ObjectPoolIDs id)
        {
            if (poolDictionary.TryGetValue(id, out object pool))
            {
                return pool;
            }

            Debug.LogWarning($"Pool with ID {id} not found.");
            return null;
        }

        /// <summary>
        /// Finds a Type by name across all assemblies, ensuring inherited types are found.
        /// </summary>
        /// <param name="typeName">The name of the class to find.</param>
        /// <returns>The Type if found, otherwise null.</returns>
        private Type FindTypeInAssemblies(string typeName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(type => type.Name == typeName);
        }
    }
}
