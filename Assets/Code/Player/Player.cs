using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : MonoBehaviour, IEventReceiver
{
    #region private_members   

    private bool                       doubleClicked;
    private GameObject                 clickVolumeObjPrev;
    private GameObject                 clickVolumeObjNext;
    private PlayerInteractionsManager  interactionsManager;
    private NavMeshAgent               navigator;
    private Animator                   animator;
    private GameObject                 targetObject;

    private void initializeValues()
    {
        doubleClicked         = false;

        clickVolumeObjPrev    = clickVolumeObjNext = null;

        interactionsManager   = new PlayerInteractionsManager(this);

        navigator             = GetComponent<NavMeshAgent>();

        targetObject          = null;

        animator              = GetComponent<Animator>();

        playerFSM             = new PlayerStateMachine(PlayerStateMachine.State.IDLE);

        currentAnimation      = PlayerAnimation.NONE;
        playerAnimationMap    = new Dictionary<PlayerAnimation, string>();
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
        clickVolumeObj.GetComponent<BoxCollider>().center = Vector3.zero;
        clickVolumeObj.GetComponent<BoxCollider>().isTrigger = true;
        clickVolumeObj.GetComponent<Renderer>().enabled = false; // TODO: change to 'true' for debug cubes to begin being visible

        return clickVolumeObj;
    }

    private void attachInteractions()
    {
        interactionsManager.addInteraction(InteractableObjectType.WALL,                    lookAt,            InputAction.SINGLE_TAP);
		interactionsManager.addInteraction(InteractableObjectType.FLOOR,                   lookAt,            InputAction.SINGLE_TAP);
        interactionsManager.addInteraction(InteractableObjectType.INTERACTIVE_SEARCHABLE,  lookAt,            InputAction.SINGLE_TAP);
        interactionsManager.addInteraction(InteractableObjectType.FLOOR,                   moveTo,            InputAction.DOUBLE_TAP);
		interactionsManager.addInteraction(InteractableObjectType.INTERACTIVE_SEARCHABLE,  interactiveSearch, InputAction.DOUBLE_TAP);
        // add interactions here...
    }

    #endregion

    #region public members

    public void lookAt(PlayerInteractionParams interactionParams)
    {
        targetObject = new GameObject("playerRotationTarget");
        targetObject.transform.position = interactionParams.interactionPoint;

        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_STARTED_TURNING, targetObject));
    }

    public void moveTo(PlayerInteractionParams interactionParams)
    {
        targetObject = new GameObject("playerMovementTarget");
        targetObject.transform.position = interactionParams.interactionPoint;

        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_STARTED_MOVING, targetObject));
    }

    public void interactiveSearch(PlayerInteractionParams interactionParams)
    {
        targetObject = Instantiate(interactionParams.obj);

        targetObject.transform.position = interactionParams.interactionPoint; // TODO: REMOVE HACK! need to use original object position, but its all zeros now!

        targetObject.GetComponent<Renderer>().enabled = true;

        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_STARTED_INTERACTIVE_SEARCH, targetObject));
    }

    private void resetTarget()
    {
        Destroy(targetObject);
        targetObject = null;
    }

    private void sendEventToSelf(EventData eventData)
    {
        EventsManager.instance.sendEventToObject(name, EventID.PLAYER_INTERNAL_EVENT, eventData);
    }

    public void onInteractableObjectClick(GameObject obj, Vector3 interactionPoint)
    {
        doubleClicked = checkForDoubleClick(interactionPoint);

        resetTarget();

        interactionsManager.interactWith(new PlayerInteractionParams(obj, interactionPoint), doubleClicked ? InputAction.DOUBLE_TAP : InputAction.SINGLE_TAP);

        doubleClicked = false;
    }

    public void receiveEvent(Event evt)
    {
        switch (evt.eventID)
        {
            case EventID.PLAYER_INTERNAL_EVENT:
                if (evt.eventData != null)
                    playerFSM.execute(evt.eventData as PlayerFsmExecData);
                break;

            default:
                break;
        }
    }

    #endregion

    #region unity_defined_methods

    void Start()
    {
        initializeValues();
        attachInteractions();
        fillAnimationMap();
        fillStateMachinesTransitions();

        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_STARTED_IDLING, targetObject));
    }

    #endregion
}
