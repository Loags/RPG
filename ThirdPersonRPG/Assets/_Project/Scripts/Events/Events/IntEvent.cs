using EventListener;
using UnityEngine;

namespace  Events
{
	[CreateAssetMenu(menuName = "Custom Events/Int Event")]
	public class IntEvent : BaseEvent<IntEventListener>
	{
		public void Raise(int value)
		{
			for (int i = Listeners.Count - 1; i >= 0; i--)
				Listeners[i].OnEventRaised(value);
		}

	}
}
