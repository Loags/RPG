using System;
using Events;
using UnityEngine;
using UnityEngine.Events;

namespace EventListener
{
	
	[Serializable]
	public class UnityStringEvent : UnityEvent<string>{}
	
	[ExecuteInEditMode]
	public class StringEventListener : MonoBehaviour
	{

		public StringEvent Event;
		public UnityStringEvent Response;

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


		public void OnEventRaised(string s)
		{
			Response.Invoke(s);
		}
	}
}
