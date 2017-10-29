using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

    private class EventProcessor : MonoBehaviour { }

    private class EventQueueElement
    {
        public EventQueueElement(Event evt, EventBroadcastType broadcastType, string searchPattern)
        {
            this.evt           = evt;
            this.broadcastType = broadcastType;
            this.searchPattern = searchPattern;
        }

        public readonly Event              evt;
        public readonly EventBroadcastType broadcastType;
        public readonly string             searchPattern;
    }

    private Queue<EventQueueElement> _eventQueue;
    private static EventsManager     _instance;
    private static object            _mutex = new object();

    private IEnumerator eventProcessorTask()
    {
        while (true)
        {
            while (_eventQueue.Count > 0)
            {
                var queueElement = _eventQueue.Dequeue();

                if ( (queueElement != null) && (queueElement.evt.eventID != EventID.NONE) )
                    switch (queueElement.broadcastType)
                    {
                        case EventBroadcastType.SEND_EVENT_BY_OBJECT_NAME:
                            {
                                var obj = GameObject.Find(queueElement.searchPattern);
                                if (obj != null)
                                {
                                    var eventReceiver = obj.GetComponent<IEventReceiver>();
                                    if (eventReceiver != null)
                                        eventReceiver.receiveEvent(queueElement.evt);
                                }
                            }
                            break;

                        case EventBroadcastType.BROADCAST_BY_TAG:
                            {
                                var objs = GameObject.FindGameObjectsWithTag(queueElement.searchPattern);
                                if (objs != null)
                                {
                                    foreach (var obj in objs)
                                    {
                                        if (obj != null)
                                        {
                                            var eventReceiver = obj.GetComponent<IEventReceiver>();
                                            if (eventReceiver != null)
                                                eventReceiver.receiveEvent(queueElement.evt);
                                        }
                                    }
                                }
                            }
                            break;
                        case EventBroadcastType.BROADCAST_ALL:
                            {
                                var objs = GameObject.FindObjectsOfType(typeof(GameObject));
                                if (objs != null)
                                {
                                    foreach (var obj in objs)
                                    {
                                        if (obj != null)
                                        {
                                            var eventReceiver = (obj as GameObject).GetComponent<IEventReceiver>();
                                            if (eventReceiver != null)
                                                eventReceiver.receiveEvent(queueElement.evt);
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
            }
            yield return new WaitForFixedUpdate();
            // TODO: fix for this frame events and previous frame events needed
        }
    }

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

    public EventsManager()
    {
        _eventQueue = new Queue<EventQueueElement>();

        var eventProcessor = new GameObject("eventProcessor");
        eventProcessor.AddComponent<EventProcessor>();
        eventProcessor.GetComponent<EventProcessor>().StartCoroutine(eventProcessorTask());
    }

    public void sendEventToObject(string objectName, EventID eventID, EventData eventData)
    {
        var evt = new Event(eventID, eventData);
        var queueElement = new EventQueueElement(evt, EventBroadcastType.SEND_EVENT_BY_OBJECT_NAME, objectName);
        _eventQueue.Enqueue(queueElement);
    }

    public void broadcastEventByTag(string tag, EventID eventID, EventData eventData)
    {
        var evt = new Event(eventID, eventData);
        var queueElement = new EventQueueElement(evt, EventBroadcastType.BROADCAST_BY_TAG, tag);
        _eventQueue.Enqueue(queueElement);
    }

    public void broadcastEvent(EventID eventID, EventData eventData)
    {
        var evt = new Event(eventID, eventData);
        var queueElement = new EventQueueElement(evt, EventBroadcastType.BROADCAST_ALL, string.Empty);
        _eventQueue.Enqueue(queueElement);
    }

    #endregion
}
