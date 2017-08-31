using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

#region private_members

    private bool _isMoving;
    private bool _isRotating;

    private PlayerInteractionsManager interactionsManager;

    private void setDefaultValues()
    {
        isMoving   = false;
        isRotating = false;

        interactionsManager = new PlayerInteractionsManager(this);
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
        // TODO: implement
        lookAt(obj, interactionPoint);
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
