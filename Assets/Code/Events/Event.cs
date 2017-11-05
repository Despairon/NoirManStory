using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum EventID
{
    NONE,
    PLAYER_INTERNAL_EVENT,
    INTERACTABLE_OBJECT_CLICKED,
	PLAYER_TO_INTER_OBJ_IS_PLAYER_IN_RANGE,
	INTER_OBJ_TO_PLAYER_IN_RANGE,
	INTER_OBJ_TO_PLAYER_OUT_OF_RANGE
};

public abstract class EventData
{
    public EventData() { }
}

public class Event
{
    #region public_members

    public Event(EventID eventID, EventData eventData)
    {
        this.eventID   = eventID;
        this.eventData = eventData;
    }

    public readonly EventID   eventID;
    public readonly EventData eventData;

    #endregion
}
