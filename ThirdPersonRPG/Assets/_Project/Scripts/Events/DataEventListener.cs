using System;
using Events;
using UnityEngine;
using UnityEngine.Events;


namespace EventListener
{
	[Serializable]
	public class UnityDataEvent : UnityEvent<ScriptableObject>{}

	[ExecuteInEditMode]
	public class DataEventListener : MonoBehaviour
	{

		
		public DataEvent Event;
		
		public UnityDataEvent Response;

		private void OnEnable()
		{
			Event.RegisterListener(this);
		}

		private void OnDisable()
		{
			Event.UnregisterListener(this);
		}

		public void OnEventRaised(ScriptableObject scriptableObject)
		{
			Response.Invoke(scriptableObject);
		}
		
		private void OnDestroy()
		{
			Event.UnregisterListener(this);
		}

		private void ListConfigOptions()
		{
		
			
		}
	}
}
