using EventListener;
using UnityEngine;

namespace Events
{
   
    [CreateAssetMenu(menuName = "Custom Events/Application Event", order = 4)]
    public class ApplicationEvent : BaseEvent<ApplicationEventListener>
    {
        
        public void Raise()
        {
            //Debug.Log("Raise " +  name);
            for (int i = Listeners.Count - 1; i >= 0; i--)
                Listeners[i].OnEventRaised();
            
        }
    }
}
 