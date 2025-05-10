using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public abstract class BaseEvent<T> : ScriptableObject {

        public List<T> Listeners = new List<T>();

        void Awake()
        {
            Listeners.Clear();	
        }

        public void RegisterListener(T listener)
        {
            if(listener == null) return;
		
            if(!Listeners.Contains(listener)) Listeners.Add(listener);
        }
    
        public void UnregisterListener(T listener)
        {
            if(Listeners.Contains(listener)) Listeners.Remove(listener);
        }
    }


}