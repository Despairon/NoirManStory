using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private Dictionary<EventID, EventHandler> eventHandlersMap;

    private void fillEventHandlersMap()
    {
        eventHandlersMap.Add(EventID.PLAYER_INTERNAL_EVENT,            handleInternalEvents);
        eventHandlersMap.Add(EventID.INTERACTABLE_OBJECT_CLICKED,      handleInteractableObjectClick);
		eventHandlersMap.Add(EventID.INTER_OBJ_TO_PLAYER_IN_RANGE,     handlePlayerInRangeRequest);
		eventHandlersMap.Add(EventID.INTER_OBJ_TO_PLAYER_OUT_OF_RANGE, handlePlayerInRangeRequest);
        
        // add event handling mappings here...
    }

    private void sendEventToSelf(EventData eventData)
    {
        EventsManager.instance.sendEventToObject(name, EventID.PLAYER_INTERNAL_EVENT, eventData);
    }

    void IEventReceiver.receiveEvent(Event evt)
    {
        if (evt != null)
        {
            try
            {
                var eventHandler = eventHandlersMap[evt.eventID];

                if (eventHandler != null)
                    eventHandler(evt);
            }
            catch (KeyNotFoundException)
            {
                // do nothing if no handler found
            }
        }
    }

    #region event_handlers

    private void handleInternalEvents(Event evt)
    {
        if (evt.eventData != null)
            playerFSM.execute(evt.eventData as PlayerFsmExecData);
    }

    private void handleInteractableObjectClick(Event evt)
    {
        var clickData        = (evt.eventData as MainCamera.InteractableObjectClickData);
        var obj              = clickData.obj;
        var interactionPoint = clickData.interactionPoint;

        doubleClicked = checkForDoubleClick(interactionPoint);

        setPlayerIdle(new PlayerFsmExecData(0, null));

        interactionsManager.interactWith(new PlayerInteractionParams(obj, interactionPoint), doubleClicked ? InputAction.DOUBLE_TAP : InputAction.SINGLE_TAP);

        doubleClicked = false;
    }

	private void handlePlayerInRangeRequest(Event evt)
	{
		switch (evt.eventID) 
		{
			case EventID.INTER_OBJ_TO_PLAYER_IN_RANGE:
				sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_INTER_OBJECT_IN_RANGE, (evt.eventData as PlayerFsmExecData).interactionParams));
		    	break;
			case EventID.INTER_OBJ_TO_PLAYER_OUT_OF_RANGE:
				sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_INTER_OBJECT_OUT_OF_RANGE, (evt.eventData as PlayerFsmExecData).interactionParams));
		   	 	break;

			default:
				break;
		}
	}

    #endregion
}
