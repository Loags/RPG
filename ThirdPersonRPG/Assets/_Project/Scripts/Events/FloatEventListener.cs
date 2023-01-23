using System;
using  Events;
using UnityEngine;
using UnityEngine.Events;

namespace EventListener
{
	
	[Serializable]
	public class UnityFloatEvent : UnityEvent<float>{}
	
	[ExecuteInEditMode]
	public class FloatEventListener : MonoBehaviour
	{

		public FloatEvent Event;
		public UnityFloatEvent Response;

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

		public void OnEventRaised(float f)
		{
			Response.Invoke(f);
		}
	}
}
