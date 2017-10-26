using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EventsManager
{
    #region private_members

    private enum EventBroadcastType
    {
        NONE,
        SEND_EVENT_BY_OBJECT_NAME,
        BROADCAST_BY_TAG,
        BROADCAST_ALL
    };

    private Queue<object>        _eventQueue;

    private static EventsManager _instance;
    private static object        _mutex = new object();

    #endregion

    #region public_members

    public static EventsManager instance
    {
        get
        {
            lock (_mutex)
            {
                if (_instance == null)
                    _instance = new EventsManager();
            }

            return _instance;
        }
    }

    #endregion

    public EventsManager()
    {
        _eventQueue = new Queue<object>();
    }

    public void sendEventToObject(string objectName, EventID eventID, EventData eventData)
    {
        // TODO: sendEventToObject: implement
    }

    public void broadcastEventByTag(string tag, EventID eventID, EventData eventData)
    {
        // TODO: broadcastEventByTag: implement
    }

    public void broadcastEvent(EventID eventID, EventData eventData)
    {
        // TODO: broadcastEvent: implement
    }


}
