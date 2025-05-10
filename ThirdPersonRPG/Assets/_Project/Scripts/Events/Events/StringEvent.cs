using EventListener;
using UnityEngine;

namespace  Events
{
	[CreateAssetMenu(menuName = "Custom Events/String Event")]
	public class StringEvent : BaseEvent<StringEventListener>
	{
		public void Raise(string value)
		{
			for (int i = Listeners.Count - 1; i >= 0; i--)
				Listeners[i].OnEventRaised(value);
		}

	}
}
