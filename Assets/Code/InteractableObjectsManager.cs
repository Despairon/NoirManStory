using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableObjectType
{
    INVALID,
    FLOOR,
    WALL
    // add types here
}

public static class InteractableObjectsManager
{
#region private_members

    private sealed class InteractableObjectTagMap
    {
        public InteractableObjectTagMap(InteractableObjectType type, string tagName)
        {
            this.type    = type;
            this.tagName = tagName;
        }

        public readonly InteractableObjectType type;
        public readonly string tagName;
    }

    private static List<InteractableObjectTagMap> interactableObjectsMap = new List<InteractableObjectTagMap>()
    {
        new InteractableObjectTagMap(InteractableObjectType.FLOOR, "Floor"),
        new InteractableObjectTagMap(InteractableObjectType.WALL,  "Wall")

        // add mappings of interactable object types on tags here
    };

#endregion

#region public_members

    public static bool isObjectInteractable(GameObject obj)
    {
        if (interactableObjectsMap.Find(item => item.tagName == obj.tag) != null)
            return true;
        else
            return false;
    }

    public static InteractableObjectType getInteractionType(GameObject obj)
    {
        var item = interactableObjectsMap.Find(it => it.tagName == obj.tag);
        if (item != null)
            return item.type;
        else
            return InteractableObjectType.INVALID;
    }

#endregion
}
