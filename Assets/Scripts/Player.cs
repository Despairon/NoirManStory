using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private delegate void PlayerStateAction(PlayerInteractionParams interactionParams);

    private sealed class PlayerStateActionMap
    {
        public readonly PlayerState       state;
        public readonly PlayerStateAction action;

        public PlayerStateActionMap(PlayerState state, PlayerStateAction action)
        {
            this.state  = state;
            this.action = action;
        }
    }
   
    private PlayerState                playerState;
    private bool                       doubleClicked;
    private GameObject                 clickVolumeObjPrev;
    private GameObject                 clickVolumeObjNext;
    private PlayerInteractionsManager  interactionsManager;
    private PlayerInteractionParams    interactionParameters;
    private List<PlayerStateActionMap> playerStateActionMap;

    private void setDefaultValues()
    {
        playerState           = PlayerState.IDLE;

        doubleClicked         = false;

        clickVolumeObjPrev    = clickVolumeObjNext = null;

        interactionsManager   = new PlayerInteractionsManager(this);

        interactionParameters = new PlayerInteractionParams(null, Vector3.zero);

        playerStateActionMap  = new List<PlayerStateActionMap>();
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
        interactionsManager.addInteraction(InteractableObjectType.WALL,  new PlayerInteractionsManager.PlayerInteraction(startTurning));
        interactionsManager.addInteraction(InteractableObjectType.FLOOR, new PlayerInteractionsManager.PlayerInteraction(startTurning));
        interactionsManager.addInteraction(InteractableObjectType.FLOOR, new PlayerInteractionsManager.PlayerInteraction(startMoving));
        // add interactions here...
    }

    private void attachStateActions()
    {
        playerStateActionMap.Add(new PlayerStateActionMap(PlayerState.TURNING, lookAt));
        playerStateActionMap.Add(new PlayerStateActionMap(PlayerState.MOVING,  lookAt));
        playerStateActionMap.Add(new PlayerStateActionMap(PlayerState.MOVING,  moveTo));
        playerStateActionMap.Add(new PlayerStateActionMap(PlayerState.USING,   lookAt));
        playerStateActionMap.Add(new PlayerStateActionMap(PlayerState.USING,   useIt));
        // add actions here...
    }

#endregion

#region public members

    public void startTurning()
    {
        playerState = PlayerState.TURNING;
    }

    public void startMoving()
    {
        if (doubleClicked)
        {
            playerState = PlayerState.MOVING;

            doubleClicked = false;
        }
    }

    public void startUsing()
    {
        if (doubleClicked)
        {
            doubleClicked = false;

            playerState = PlayerState.USING;
        }
    }

    public void lookAt(PlayerInteractionParams interactionParams)
    {
        var rotationPoint = interactionParams.interactionPoint - transform.position;
		rotationPoint.y = 0f;

		float turningRate = 480f; // degrees

		var rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rotationPoint), turningRate * Time.deltaTime);

		GetComponent<Rigidbody>().MoveRotation(rotation);
    }

    public void moveTo(PlayerInteractionParams interactionParams)
    {		
		var distance = Vector3.Distance (interactionParams.interactionPoint, transform.position);

		if (distance <= transform.position.y + 0.5f) 
		{
			playerState = PlayerState.IDLE;
			// TODO: notify that player arrived
			return;
		}

        var movementPoint = interactionParams.interactionPoint - transform.position;
		movementPoint.y = 0f;

		float speed = 10f;

		movementPoint = movementPoint.normalized * speed * Time.deltaTime;
		GetComponent<Rigidbody>().MovePosition(transform.position + movementPoint);
    }

    public void useIt(PlayerInteractionParams interactionParams)
    {
        // TODO: implement
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

            interactionParameters = new PlayerInteractionParams(obj, interactionPoint);

            interactionsManager.interactWith(interactionParameters);
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
        attachStateActions();
	}
	
	void Update ()
    {
        if (playerState != PlayerState.DEAD)
        {
            // TODO: ...
        }
    }

    private void FixedUpdate()
    {
        if (playerState != PlayerState.DEAD)
        {
            foreach (var stateAction in playerStateActionMap.FindAll(item => item.state == playerState))
                if ((stateAction != null) && (stateAction.action != null))
                    stateAction.action(interactionParameters);
        }
    }

    #endregion

}
