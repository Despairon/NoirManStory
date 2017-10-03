using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{

#region private_members   

    private bool                       doubleClicked;
    private GameObject                 clickVolumeObjPrev;
    private GameObject                 clickVolumeObjNext;
    private PlayerInteractionsManager  interactionsManager;
    private NavMeshAgent               navigator;
    private StateMachine               stateMachine;

    private void setDefaultValues()
    {
        doubleClicked         = false;

        clickVolumeObjPrev    = clickVolumeObjNext = null;

        interactionsManager   = new PlayerInteractionsManager(this);

        navigator             = GetComponent<NavMeshAgent>();

        gameObject.AddComponent<StateMachine>();

        stateMachine          = GetComponent<StateMachine>();
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
        clickVolumeObj.transform.localScale = new Vector3(50f, 50f, 50f);
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

    private void fillStateMachineTransitions()
    {
        // TODO: fill logic -> replace null's with methods!!!

        // TODO: rework state machine implementation

        // ordinary transitions table
        stateMachine.addNode(FSM_TransitionState.IDLE,           null, FSM_TransitionState.PLAYER_TURNING, null);
        stateMachine.addNode(FSM_TransitionState.PLAYER_TURNING, null, FSM_TransitionState.PLAYER_MOVING,  null);
        stateMachine.addNode(FSM_TransitionState.PLAYER_MOVING,  null, FSM_TransitionState.PLAYER_USING,   null);
        stateMachine.addNode(FSM_TransitionState.PLAYER_USING,   null, FSM_TransitionState.IDLE,           null);

        // transitions to IDLE
        stateMachine.addNode(FSM_TransitionState.PLAYER_TURNING, null, FSM_TransitionState.IDLE, null);
        stateMachine.addNode(FSM_TransitionState.PLAYER_MOVING,  null, FSM_TransitionState.IDLE, null);
    }

#endregion

#region public members

    public void lookAt(PlayerInteractionParams interactionParams)
    {
        
    }

    public void moveTo(PlayerInteractionParams interactionParams)
    {
        if (doubleClicked)
        {
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

    void Start ()
    {
        setDefaultValues();
        attachInteractions();
        fillStateMachineTransitions();
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        if (InteractableObjectsManager.isObjectInteractable(collider.gameObject))
        {
            // TODO: action on reaching destination

        }
    }

    #endregion

}
