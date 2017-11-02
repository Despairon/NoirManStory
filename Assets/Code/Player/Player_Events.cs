using System.Collections.Generic;

public partial class Player
{
    private Dictionary<EventID, EventHandler> eventHandlersMap;

    private void fillEventHandlersMap()
    {
        eventHandlersMap.Add(EventID.PLAYER_INTERNAL_EVENT,       handleInternalEvents);
        eventHandlersMap.Add(EventID.INTERACTABLE_OBJECT_CLICKED, handleInteractableObjectClick);
        
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

    public void handleInteractableObjectClick(Event evt)
    {
        var clickData        = (evt.eventData as MainCamera.InteractableObjectClickData);
        var obj              = clickData.obj;
        var interactionPoint = clickData.interactionPoint;

        doubleClicked = checkForDoubleClick(interactionPoint);

        setPlayerIdle(new PlayerFsmExecData(0, null));

        interactionsManager.interactWith(new PlayerInteractionParams(obj, interactionPoint), doubleClicked ? InputAction.DOUBLE_TAP : InputAction.SINGLE_TAP);

        doubleClicked = false;
    }

    #endregion
}
