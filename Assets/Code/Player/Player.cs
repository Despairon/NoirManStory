using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : MonoBehaviour
{
    #region private_members   

    private State                      _state;
    private bool                       doubleClicked;
    private GameObject                 clickVolumeObjPrev;
    private GameObject                 clickVolumeObjNext;
    private PlayerInteractionsManager  interactionsManager;
    private NavMeshAgent               navigator;
    private GameObject                 targetObject; // TODO: set target object after approppriate interaction!!!

    private void initializeValues()
    {
        state                 = State.IDLE;

        doubleClicked         = false;

        clickVolumeObjPrev    = clickVolumeObjNext = null;

        interactionsManager   = new PlayerInteractionsManager(this);

        navigator             = GetComponent<NavMeshAgent>();

        targetObject          = null;

        playerTurningFSM      = new PlayerStateMachine(PlayerStateMachine.State.IDLE);
        playerMovingFSM       = new PlayerStateMachine(PlayerStateMachine.State.IDLE);
        playerUsingFSM        = new PlayerStateMachine(PlayerStateMachine.State.IDLE);

        stateMachineMap       = new Dictionary<State, PlayerStateMachine>();
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
        clickVolumeObj.transform.localScale = new Vector3(50f, 50f, 50f);
        clickVolumeObj.GetComponent<BoxCollider>().center = Vector3.zero;
        clickVolumeObj.GetComponent<BoxCollider>().isTrigger = true;
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

    public enum State
    {
        IDLE,
        TURNING,
        MOVING,
        USING
    }

    public State state
    {
        get         { return _state;  }
        private set { _state = value; }
    }

    public void lookAt(PlayerInteractionParams interactionParams)
    {
        state = State.TURNING;

        targetObject = new GameObject("playerRotationTarget");
        targetObject.transform.position = interactionParams.interactionPoint;
    }

    public void moveTo(PlayerInteractionParams interactionParams)
    {
        if (doubleClicked)
        {
            state = State.MOVING;

            // TODO: use these 
            //navigator.SetDestination(interactionParams.interactionPoint);
            //const float dist_threshold = 10f;
            //if (navigator.remainingDistance <= dist_threshold)
            //    animationStateMachine.SetTrigger("movingEnded");
        }
    }

    public void useIt(PlayerInteractionParams interactionParams)
    {
        if (doubleClicked)
        {
            state = State.USING;
        }
    }

    public void onInteractableObjectClick(GameObject obj, Vector3 interactionPoint)
    {
        doubleClicked = checkForDoubleClick(interactionPoint);

        interactionsManager.interactWith(new PlayerInteractionParams(obj, interactionPoint));

        doubleClicked = false;
    }

    #endregion

    #region unity_defined_methods

    void Start()
    {
        initializeValues();
        attachInteractions();
        fillStateMachineMap();
        fillStateMachinesTransitions();
    }

    private void FixedUpdate()
    {
        if (state != State.IDLE)
            executeFsmForState(state);
        else
            resetStateMachines();
    }

    private void Update()
    {
        // TODO: ...
    }

    #endregion
}
