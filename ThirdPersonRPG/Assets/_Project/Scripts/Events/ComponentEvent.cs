using EventListener;
using UnityEngine;

namespace Events
{
	[CreateAssetMenu(menuName = "Custom Events/Component Event", order = 3)]
	public class ComponentEvent : BaseEvent<ComponentEventListener>
	{
		public void Raise(Component component)
		{
			Debug.Log("Raised " + name);
			for (int i = Listeners.Count - 1; i >= 0; i--)
				Listeners[i].OnEventRaised(component);
		}
	}
}
