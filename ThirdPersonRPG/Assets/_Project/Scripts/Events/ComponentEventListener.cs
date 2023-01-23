using System;
using Events;
using UnityEngine;
using UnityEngine.Events;


namespace EventListener
{
	[Serializable]
	public class UnityComponentEvent : UnityEvent<Component>{}
	
	[ExecuteInEditMode]
	public class ComponentEventListener : MonoBehaviour
	{

		public ComponentEvent Event;
		
		public UnityComponentEvent Response;

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

		public void OnEventRaised(Component component)
		{
			Response.Invoke(component);
		}
	}
}
