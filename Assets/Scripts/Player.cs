using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

#region private_members

    private bool _isMoving;
    private bool _isRotating;

    private bool _doubleClicked;

    private GameObject clickVolumeObjPrev, clickVolumeObjNext;

    private PlayerInteractionsManager interactionsManager;

    private void setDefaultValues()
    {
        isMoving   = false;
        isRotating = false;

        _doubleClicked = false;

        clickVolumeObjPrev = clickVolumeObjNext = null;

        interactionsManager = new PlayerInteractionsManager(this);
    }

    private bool checkForDoubleClick(Vector3 point)
    {
        bool result = false;

        clickVolumeObjNext = createClickVolumeAt(point);
        if (clickVolumeObjPrev != null)
        {
            if (clickVolumeObjNext.GetComponent<BoxCollider>().bounds.Intersects(clickVolumeObjPrev.GetComponent<BoxCollider>().bounds))
                result = true;
            else
                result = false;

            // removes game object from displaying, not class object
            Destroy(clickVolumeObjPrev);
        }
        else
            result = false;

        clickVolumeObjPrev = clickVolumeObjNext;

        return result;
    }

    private GameObject createClickVolumeAt(Vector3 point)
    {
        var clickVolumeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        clickVolumeObj.transform.position = point;
        clickVolumeObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        clickVolumeObj.AddComponent<BoxCollider>();
        clickVolumeObj.GetComponent<BoxCollider>().center = Vector3.zero;
        clickVolumeObj.GetComponent<Renderer>().enabled = false; // TODO: change to 'true' for debug cubes to begin being visible


        return clickVolumeObj;
    }

    private void attachInteractions()
    {
        interactionsManager.addInteraction(InteractableObjectType.WALL,  new PlayerInteractionsManager.PlayerInteraction(lookAt));
        interactionsManager.addInteraction(InteractableObjectType.FLOOR, new PlayerInteractionsManager.PlayerInteraction(lookAt));
        interactionsManager.addInteraction(InteractableObjectType.FLOOR, new PlayerInteractionsManager.PlayerInteraction(moveTo));
        // add interactions here...
    }

#endregion

#region public members

    public bool isMoving
    {
        get         { return _isMoving;  }
        private set { _isMoving = value; }
    }

    public bool isRotating
    {
        get         { return _isRotating;  }
        private set { _isRotating = value; }
    }

    public void moveTo(GameObject obj, Vector3 interactionPoint)
    {
        if (_doubleClicked)
        {
            // TODO: implement
            lookAt(obj, interactionPoint);

            _doubleClicked = false;
        }
    }

    public void lookAt(GameObject obj, Vector3 interactionPoint)
    {
        // TODO: implement
        var playerToObject = interactionPoint - transform.position;
        playerToObject.y = 0f;

        Quaternion newRotation = Quaternion.LookRotation(playerToObject);
        GetComponent<Rigidbody>().rotation = newRotation;
    }

    public void onInteractableObjectClick(GameObject obj, Vector3 interactionPoint)
    {
        _doubleClicked = checkForDoubleClick(interactionPoint);

        interactionsManager.interactWith(obj, interactionPoint);
    }

#endregion

#region unity_defined_methods

    void Start ()
    {
        setDefaultValues();
        attachInteractions();
	}
	
	void Update ()
    {

    }

    private void FixedUpdate()
    {

    }

    #endregion

}
