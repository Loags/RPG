using System;
using  Events;
using UnityEngine;
using UnityEngine.Events;

namespace EventListener
{
	
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>{}
	
	[ExecuteInEditMode]
	public class BoolEventListener : MonoBehaviour
	{

		public BoolEvent Event;
		public UnityBoolEvent Response;

		private void OnEnable()
		{
			Event.RegisterListener(this);
		}

		private void OnDisable()
		{
			Event.UnregisterListener(this);
		}

		private void OnDestroy()
		{
			Event.UnregisterListener(this);
		}
		
		public void OnEventRaised(bool b)
		{
			Response.Invoke(b);
		}
	}
}
