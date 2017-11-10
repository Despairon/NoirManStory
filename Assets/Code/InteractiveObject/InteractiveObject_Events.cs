using System;
using UnityEngine;

public partial class InteractiveObject
{
	void IEventReceiver.receiveEvent(Event evt)
	{
		if (evt != null) 
		{
			switch (evt.eventID) 
			{
				case EventID.PLAYER_TO_INTER_OBJ_IS_PLAYER_IN_RANGE:
				{
					var player = GameObject.FindWithTag("Player");
					if (player != null)
					{
						var eventToSend = isPlayerInRange ? EventID.INTER_OBJ_TO_PLAYER_IN_RANGE : EventID.INTER_OBJ_TO_PLAYER_OUT_OF_RANGE;
						EventsManager.instance.sendEventToObject (player.name, eventToSend, evt.eventData);
					}
				}
					break;
				default:
					break;
			}
		}
	}
}

