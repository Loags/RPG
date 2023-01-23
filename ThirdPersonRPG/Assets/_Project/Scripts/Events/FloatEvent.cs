using EventListener;
using UnityEngine;

namespace  Events
{
	[CreateAssetMenu(menuName = "Custom Events/Float Event", order = 2)]
	public class FloatEvent : BaseEvent<FloatEventListener>
	{
		public void Raise(float f)
		{
			for (int i = Listeners.Count - 1; i >= 0; i--)
				Listeners[i].OnEventRaised(f);
		}

	}
}
