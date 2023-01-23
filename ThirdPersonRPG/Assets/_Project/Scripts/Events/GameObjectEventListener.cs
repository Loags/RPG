using System;
using  Events;
using UnityEngine;
using UnityEngine.Events;


namespace EventListener
{
	[Serializable]
	public class UnityGameObjectEvent : UnityEvent<GameObject>{}
	
	[ExecuteInEditMode]
	public class GameObjectEventListener : MonoBehaviour
	{

		public GameObjectEvent Event;
		
		public UnityGameObjectEvent Response;

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
		
		public void OnEventRaised(GameObject go)
		{
			Response.Invoke(go);
		}
	}
}
