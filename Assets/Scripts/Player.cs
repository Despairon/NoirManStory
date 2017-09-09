using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerState
{
    DEAD,
    IDLE,
    TURNING,
    MOVING,
    USING
}

public class Player : MonoBehaviour
{

#region private_members   
    private PlayerState                playerState;
    private bool                       doubleClicked;
    private GameObject                 clickVolumeObjPrev;
    private GameObject                 clickVolumeObjNext;
    private PlayerInteractionsManager  interactionsManager;
    private NavMeshAgent               navigator;
    private Animator                   animationStateMachine;

    private void setDefaultValues()
    {
        playerState           = PlayerState.IDLE;

        doubleClicked         = false;

        clickVolumeObjPrev    = clickVolumeObjNext = null;

        interactionsManager   = new PlayerInteractionsManager(this);

        navigator             = GetComponent<NavMeshAgent>();

        animationStateMachine          = GetComponent<Animator>();
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

    public void lookAt(PlayerInteractionParams interactionParams)
    {   
        playerState = PlayerState.TURNING;
        navigator.SetDestination(transform.position);
    }

    public void moveTo(PlayerInteractionParams interactionParams)
    {
        if (doubleClicked)
        {
            playerState = PlayerState.MOVING;

            doubleClicked = false;

            navigator.SetDestination(interactionParams.interactionPoint);
        }
    }

    public void useIt(PlayerInteractionParams interactionParams)
    {
        if (doubleClicked)
        {
            playerState = PlayerState.USING;

            doubleClicked = false;
        }
    }

    public void kill()
    {
        playerState = PlayerState.DEAD;

        // TODO: ...
    }

    public void onInteractableObjectClick(GameObject obj, Vector3 interactionPoint)
    {
        if (playerState != PlayerState.DEAD)
        {
            doubleClicked = checkForDoubleClick(interactionPoint);

            interactionsManager.interactWith(new PlayerInteractionParams(obj, interactionPoint));
        }
    }

    public PlayerState getState()
    {
        return playerState;
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
        if (playerState != PlayerState.DEAD)
        {
            // TODO: ...
        }
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        if (InteractableObjectsManager.isObjectInteractable(collider.gameObject))
        {
            Debug.Log("we hit " + collider.gameObject.name);
            playerState = PlayerState.IDLE;
        }
    }

    private void FixedUpdate()
    {
        if (playerState != PlayerState.DEAD)
        {

        }
    }

    #endregion

}
