using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableObjectType
{
    INVALID = 0,
    FLOOR,
    WALL,
	DOOR,
    INTERACTIVE_SEARCHABLE,
    PICK_UPABLE,
    PERSON
    // add types here
}

public static class InteractableObjectsManager
{
    #region private_members

    private static Dictionary<string, InteractableObjectType> interactableObjectsMap = new Dictionary<string, InteractableObjectType>()
    {
        { "Floor",                 InteractableObjectType.FLOOR                  },
        { "Wall",                  InteractableObjectType.WALL                   },
		{ "Door",                  InteractableObjectType.DOOR                   },
        { "InteractiveSearchable", InteractableObjectType.INTERACTIVE_SEARCHABLE },
        { "PickUpable",            InteractableObjectType.PICK_UPABLE            },
        { "Person",                InteractableObjectType.PERSON                 }
    };

    #endregion

    #region public_members

    public static bool isObjectInteractable(GameObject obj)
    {
        try
        {
            var objType = interactableObjectsMap[obj.tag];

            Enum.IsDefined(typeof(InteractableObjectType), objType);

            if (Enum.IsDefined(typeof(InteractableObjectType), objType) && (objType != InteractableObjectType.INVALID))
                return true;
            else
                return false;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public static InteractableObjectType getInteractionType(GameObject obj)
    {
        try
        {
            var objType = interactableObjectsMap[obj.tag];

            return objType;
        }
        catch (KeyNotFoundException)
        {
            return InteractableObjectType.INVALID;
        }
    }

    #endregion
}
