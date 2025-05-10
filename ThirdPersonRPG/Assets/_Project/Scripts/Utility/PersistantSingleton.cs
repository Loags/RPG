using UnityEngine;

namespace LB
{
    /// <summary>
    /// A generic persistent singleton. Inherit from this class for a static Instance that persists across scene loads.
    /// </summary>
    /// <typeparam name="T">The type inheriting from PersistantSingleton.</typeparam>
    [DefaultExecutionOrder(-50)]
    public abstract class PersistantSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }
        private static bool applicationIsQuitting = false;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"[PersistantSingleton<{typeof(T)}>]: Duplicate instance found. Destroying the new one.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnApplicationQuit()
        {
            // This flag prevents creating a new instance during application shutdown.
            applicationIsQuitting = true;
        }
    }
}
