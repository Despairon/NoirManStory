using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : MonoBehaviour
{
    #region private_members

    private PlayerStateMachine playerTurningFSM;
    private PlayerStateMachine playerMovingFSM;
    private PlayerStateMachine playerUsingFSM;

    private Dictionary<State, PlayerStateMachine> stateMachineMap;

    private void resetStateMachines()
    {
        playerTurningFSM.reset();
        playerMovingFSM.reset();
        playerUsingFSM.reset();
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
        stateMachineMap.Add(State.TURNING, playerTurningFSM);
        stateMachineMap.Add(State.MOVING,  playerMovingFSM);
        stateMachineMap.Add(State.USING,   playerUsingFSM);
    }

    #region player_fsm_transition_rules

    private bool emptyTransitionRule(GameObject target)
    {
        // do nothing
        return true;
    }

    private bool checkRotation(GameObject target)
    {
        // TODO: checkRotation: implement
        Vector3 toRotation   = (target.transform.position - transform.position).normalized;
        Vector3 fromRotation = transform.forward;

        Debug.Log(Vector3.Angle(fromRotation, toRotation)); 

        return true;
    }

    private bool checkIfTargetIsReached(GameObject target)
    {
        //TODO: checkIfTargetIsReached: implement
        return true;
    }

    private bool isUsingEnded(GameObject target)
    {
        //TODO: isUsingEnded: implement
        return true;
    }

    #endregion

    #region player_fsm_state_actions

    private void emptyAction(GameObject target)
    {
        // do nothing
    }

    private void setIdleAnimation(GameObject target)
    {
        // TODO: setIdleAnimation
    }

    private void setMovingAnimation(GameObject target)
    {
        // TODO: setMovingAnimation: implement
    }

    private void setUsingAnimation(GameObject target)
    {
        // TODO: setUsingAnimation: implement
    }

    private void startMovingToTarget(GameObject target)
    {
        // TODO: startMovingToTarget: implement
    }

    private void rotateToTarget(GameObject target)
    {
        // TODO: rotateToTarget: implement
    }

    private void startUsingTarget(GameObject target)
    {
        // TODO: startUsingTarget: implement
    }

    private void usingTarget(GameObject target)
    {
        // TODO: usingTarget: implement
    }

    private void setStateIdle(GameObject target)
    {
        state = State.IDLE;

        Destroy(targetObject);
        targetObject = null;
    }

    #endregion

    private void fillStateMachinesTransitions()
    {
        // TODO: complete state machines -> replace null's with callbacks!!!

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

        // player using state machine transitions table section
        playerUsingFSM.addTransition(PlayerStateMachine.State.IDLE,                 emptyTransitionRule,    PlayerStateMachine.State.TURNING,              emptyAction);
        playerUsingFSM.addTransition(PlayerStateMachine.State.TURNING,              checkRotation,          PlayerStateMachine.State.SET_MOVING_ANIMATION, rotateToTarget);
        playerUsingFSM.addTransition(PlayerStateMachine.State.SET_MOVING_ANIMATION, emptyTransitionRule,    PlayerStateMachine.State.START_MOVING,         setMovingAnimation);
        playerUsingFSM.addTransition(PlayerStateMachine.State.START_MOVING,         emptyTransitionRule,    PlayerStateMachine.State.MOVING,               startMovingToTarget);
        playerUsingFSM.addTransition(PlayerStateMachine.State.MOVING,               checkIfTargetIsReached, PlayerStateMachine.State.SET_USING_ANIMATION,  emptyAction);
        playerUsingFSM.addTransition(PlayerStateMachine.State.SET_USING_ANIMATION,  emptyTransitionRule,    PlayerStateMachine.State.START_USING,          setUsingAnimation);
        playerUsingFSM.addTransition(PlayerStateMachine.State.START_USING,          emptyTransitionRule,    PlayerStateMachine.State.USING,                startUsingTarget);
        playerUsingFSM.addTransition(PlayerStateMachine.State.USING,                isUsingEnded,           PlayerStateMachine.State.SET_IDLE_ANIMATION,   usingTarget);
        playerUsingFSM.addTransition(PlayerStateMachine.State.SET_IDLE_ANIMATION,   emptyTransitionRule,    PlayerStateMachine.State.END_USING,            setIdleAnimation);
        playerUsingFSM.addTransition(PlayerStateMachine.State.END_USING,            emptyTransitionRule,    PlayerStateMachine.State.IDLE,                 setStateIdle);
    }

    #endregion


    #region public_members

    #endregion
}
