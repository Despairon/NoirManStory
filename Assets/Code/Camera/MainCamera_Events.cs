using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class MainCamera
{
    public sealed class InteractableObjectClickData : EventData
    {
        public InteractableObjectClickData(GameObject obj, Vector3 interactionPoint)
        {
            this.obj              = obj;
            this.interactionPoint = interactionPoint;
        }

        public readonly GameObject obj;
        public readonly Vector3    interactionPoint;
    }

    void IEventReceiver.receiveEvent(Event evt)
    {
        Debug.Log("camera received event! " + evt.eventID.ToString());
        // TODO: ...
    }
}
