﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableObjectType
{
    INVALID = 0,
    FLOOR,
    WALL
    // add types here
}

public static class InteractableObjectsManager
{
    #region private_members

    private static Dictionary<string, InteractableObjectType> interactableObjectsMap = new Dictionary<string, InteractableObjectType>()
    {
        { "Floor", InteractableObjectType.FLOOR },
        { "Wall",  InteractableObjectType.WALL  }
    };

    #endregion

    #region public_members

    public static bool isObjectInteractable(GameObject obj)
    {
        try
        {
            var objType = interactableObjectsMap[obj.tag];

            return objType == InteractableObjectType.INVALID ? false : true;
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
