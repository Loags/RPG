using EventListener;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Custom Events/Bool Event", order = 1)]
    public class BoolEvent : BaseEvent<BoolEventListener>
    {

        public void Raise(bool b)
        {
            for (int i = Listeners.Count - 1; i >= 0; i--)
                Listeners[i].OnEventRaised(b);
        }

    }

}
