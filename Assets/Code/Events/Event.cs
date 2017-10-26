using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum EventID
{
    NONE,
};

public class EventData
{
    // TODO: ...
}

public delegate void EventHandlers(Enum eventID, EventData eventData);

public class Event
{
    #region private_members

    private Enum          _eventID;
    private EventData     _eventData;
    private EventHandlers _eventHandlers;

    #endregion

    #region public_members

    public void GC()
    {
       
    }

    #endregion
}
