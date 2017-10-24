using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : MonoBehaviour
{
    #region private_members

    private PlayerStateMachine playerTurningFSM;
    private PlayerStateMachine playerMovingFSM;
    private PlayerStateMachine playerInteractiveSearchFSM;

    private Dictionary<State, PlayerStateMachine> stateMachineMap;

    private void resetStateMachines()
    {
        playerTurningFSM.reset();
        playerMovingFSM.reset();
        playerInteractiveSearchFSM.reset();

        Destroy(targetObject);
        targetObject = null;
        
        navigator.destination = transform.position;
        navigator.isStopped = true;

        setIdleAnimation(null);
    }

    private void executeFsmForState(State state)
    {
        try
        {
            var stateMachine = stateMachineMap[state];

            if (stateMachine != null)
            {
                stateMachine.execute(targetObject);
            }
        }
        catch (KeyNotFoundException)
        {
            // do nothing
        }
    }

    private void fillStateMachineMap()
    {
        stateMachineMap.Add(State.TURNING,                 playerTurningFSM);
        stateMachineMap.Add(State.MOVING,                  playerMovingFSM);
        stateMachineMap.Add(State.INTERACTIVE_SEARCHING,   playerInteractiveSearchFSM);
    }

    #region player_fsm_transition_rules

    private bool emptyTransitionRule(GameObject target)
    {
        // do nothing
        return true;
    }

    private bool checkRotation(GameObject target)
    {
        const float ROTATION_COMPLETE_THRESHOLD = 5; // angles

        Vector3 toRotation   = (target.transform.position - transform.position).normalized;
        toRotation.y = 0;
        Vector3 fromRotation = transform.forward;

        var angleToTarget = Vector3.Angle(fromRotation, toRotation);

        if (angleToTarget <= ROTATION_COMPLETE_THRESHOLD)
            return true;
        else
            return false;
    }

    private bool checkIfTargetIsReached(GameObject target)
    {
        const float dist_threshold = 0.5f;

        if (!navigator.pathPending && (navigator.remainingDistance <= dist_threshold))
        {
            return true;
        }
        else
            return false;
    }

    private bool isUsingEnded(GameObject target)
    {
        //TODO: isUsingEnded: implement and remove temporary implementation!!!

        if (Input.GetKeyDown(KeyCode.Space))
            return true;
        else
            return false;
    }

    #endregion

    #region player_fsm_state_actions

    private void emptyAction(GameObject target)
    {
        // do nothing
    }

    private void setIdleAnimation(GameObject target)
    {
        playAnim(PlayerAnimation.IDLE);
    }

    private void setMovingAnimation(GameObject target)
    {
        playAnim(PlayerAnimation.WALKING);
    }

    private void setUsingAnimation(GameObject target)
    {
        playAnim(PlayerAnimation.USING);
    }

    private void startMovingToTarget(GameObject target)
    {
        navigator.destination = target.transform.position;
        navigator.isStopped = false;
    }

    private void rotateToTarget(GameObject target)
    {
        const float ROTATION_SPEED = 5.0f;

        if (target != null)
        {
            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATION_SPEED * Time.fixedDeltaTime);
        }
    }

    private void startInteractiveSearch(GameObject target)
    {
        // TODO: startUsingTarget: implement
    }

    private void setStateIdle(GameObject target)
    {
        state = State.IDLE;
    }

    #endregion

    private void fillStateMachinesTransitions()
    {
        // player turning state machine transitions table section
        playerTurningFSM.addTransition(PlayerStateMachine.State.IDLE,                emptyTransitionRule,    PlayerStateMachine.State.TURNING,              emptyAction);
        playerTurningFSM.addTransition(PlayerStateMachine.State.TURNING,             checkRotation,          PlayerStateMachine.State.END_TURNING,          rotateToTarget);
        playerTurningFSM.addTransition(PlayerStateMachine.State.END_TURNING,         emptyTransitionRule,    PlayerStateMachine.State.IDLE,                 setStateIdle);

        // player moving state machine transitions table section
        playerMovingFSM.addTransition(PlayerStateMachine.State.IDLE,                 emptyTransitionRule,    PlayerStateMachine.State.TURNING,              emptyAction);
        playerMovingFSM.addTransition(PlayerStateMachine.State.TURNING,              checkRotation,          PlayerStateMachine.State.SET_MOVING_ANIMATION, rotateToTarget);
        playerMovingFSM.addTransition(PlayerStateMachine.State.SET_MOVING_ANIMATION, emptyTransitionRule,    PlayerStateMachine.State.START_MOVING,         setMovingAnimation);
        playerMovingFSM.addTransition(PlayerStateMachine.State.START_MOVING,         emptyTransitionRule,    PlayerStateMachine.State.MOVING,               startMovingToTarget);
        playerMovingFSM.addTransition(PlayerStateMachine.State.MOVING,               checkIfTargetIsReached, PlayerStateMachine.State.SET_IDLE_ANIMATION,   emptyAction);
        playerMovingFSM.addTransition(PlayerStateMachine.State.SET_IDLE_ANIMATION,   emptyTransitionRule,    PlayerStateMachine.State.END_MOVING,           setIdleAnimation);
        playerMovingFSM.addTransition(PlayerStateMachine.State.END_MOVING,           emptyTransitionRule,    PlayerStateMachine.State.IDLE,                 setStateIdle);

        // player interactive search state machine transitions table section
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.IDLE,                 emptyTransitionRule,    PlayerStateMachine.State.TURNING,              emptyAction);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.TURNING,              checkRotation,          PlayerStateMachine.State.SET_MOVING_ANIMATION, rotateToTarget);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.SET_MOVING_ANIMATION, emptyTransitionRule,    PlayerStateMachine.State.START_MOVING,         setMovingAnimation);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.START_MOVING,         emptyTransitionRule,    PlayerStateMachine.State.MOVING,               startMovingToTarget);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.MOVING,               checkIfTargetIsReached, PlayerStateMachine.State.SET_USING_ANIMATION,  emptyAction);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.SET_USING_ANIMATION,  emptyTransitionRule,    PlayerStateMachine.State.START_USING,          setUsingAnimation);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.START_USING,          emptyTransitionRule,    PlayerStateMachine.State.USING,                startInteractiveSearch);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.USING,                isUsingEnded,           PlayerStateMachine.State.SET_IDLE_ANIMATION,   emptyAction);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.SET_IDLE_ANIMATION,   emptyTransitionRule,    PlayerStateMachine.State.END_USING,            setIdleAnimation);
        playerInteractiveSearchFSM.addTransition(PlayerStateMachine.State.END_USING,            emptyTransitionRule,    PlayerStateMachine.State.IDLE,                 setStateIdle);
    }

    #endregion
}
