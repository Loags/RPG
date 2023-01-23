using Events;
using UnityEngine;
using UnityEngine.Events;

namespace EventListener
{
    [ExecuteInEditMode]
    public class ApplicationEventListener : MonoBehaviour
    {
        public ApplicationEvent Event;
        public UnityEvent Response;


        private void OnEnable()
        {
            if (Event != null) Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (Event != null) Event.UnregisterListener(this);
        }

        void OnDestroy()
        {
            if (Event != null) Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}