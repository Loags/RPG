using System;
using  Events;
using UnityEngine;
using UnityEngine.Events;

namespace EventListener
{
	
	[Serializable]
	public class UnityIntEvent : UnityEvent<int>{}
	
	[ExecuteInEditMode]
	public class IntEventListener : MonoBehaviour
	{

		public IntEvent Event;
		public UnityIntEvent Response;

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


		public void OnEventRaised(int i)
		{
			Response.Invoke(i);
		}
	}
}
