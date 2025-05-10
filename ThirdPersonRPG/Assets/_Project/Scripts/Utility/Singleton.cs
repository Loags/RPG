using UnityEngine;

namespace LB
{
    /// <summary>
    /// A generic non-persistent singleton. Inherit from this class for a static Instance that is only valid within the current scene.
    /// </summary>
    /// <typeparam name="T">The type inheriting from Singleton.</typeparam>
    [DefaultExecutionOrder(-50)]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"[Singleton<{typeof(T)}>]: Duplicate instance found. Destroying the new one.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            // Only clear the instance if this is the current instance.
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
