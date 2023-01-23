using EventListener;
using UnityEngine;

namespace  Events
{
   
	[CreateAssetMenu(menuName = "Custom Events/Data Event", order = 5)]
	public class DataEvent : BaseEvent<DataEventListener>
	{
       
		public void Raise(ScriptableObject scriptableObject)
		{
			//Debug.Log("Raised " + name);
			for (int i = Listeners.Count - 1; i >= 0; i--)
				Listeners[i].OnEventRaised(scriptableObject);
		}
		
	}
}
