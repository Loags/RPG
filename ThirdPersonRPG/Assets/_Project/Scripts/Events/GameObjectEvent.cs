using EventListener;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Custom Events/GameObject Event", order = 3)]
    public class GameObjectEvent : BaseEvent<GameObjectEventListener>
    {
        
        public void Raise(GameObject _go)
        {
            //Debug.Log("Raise " +  name);
            for (int i = Listeners.Count - 1; i >= 0; i--)
                Listeners[i].OnEventRaised(_go);
            
        }
    }
}
 