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

        Destroy(targetObject);
        targetObject = null;

        navigator.destination = transform.position;
        navigator.isStopped = true;
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
        const float ROTATION_COMPLETE_THRESHOLD = 5; // angles

        Vector3 toRotation   = (target.transform.position - transform.position).normalized;
        Vector3 fromRotation = transform.forward;

        var angleToTarget = Vector3.Angle(fromRotation, toRotation);

        if (angleToTarget <= ROTATION_COMPLETE_THRESHOLD)
            return true;
        else
            return false;
    }

    private bool checkIfTargetIsReached(GameObject target)
    {
        const float dist_threshold = 10f;

        if (!navigator.pathPending && (navigator.remainingDistance <= dist_threshold))
        {
            return true;
        }
        else
            return false;
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

            // TODO: discuss these two implementations and find out which is better

            //Vector3 toRotation = (target.transform.position - transform.position).normalized;
            //Vector3 fromRotation = transform.forward;

            //var angleToTarget = Vector3.SignedAngle(fromRotation, toRotation, transform.up);

            /*if (angleToTarget >= 0)
            {
                // rotate right
                transform.Rotate(Vector3.up, ROTATION_SPEED * Time.fixedDeltaTime);
            }
            else
            {
                // rotate left
                transform.Rotate(Vector3.up, ROTATION_SPEED * Time.fixedDeltaTime * -1.0f);
            }*/
        }
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
