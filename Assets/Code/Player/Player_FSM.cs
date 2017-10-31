using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : MonoBehaviour
{
    #region private_members

    private PlayerStateMachine playerFSM;

    #region player_fsm_state_actions

    private void emptyAction(PlayerFsmExecData execData)
    {
        // do nothing
    }

    private void setPlayerIdle(PlayerFsmExecData execData)
    {
        playerFSM.reset();

        Destroy(targetObject);
        targetObject = null;

        navigator.destination = transform.position;
        navigator.isStopped   = true;

        setIdleAnimation(execData);
    }

    private void checkRotation(PlayerFsmExecData execData)
    {
        const float ROTATION_COMPLETE_THRESHOLD = 5.0f; // angles

        if (execData.target != null)
        {
            Vector3 toRotation = (execData.target.transform.position - transform.position).normalized;
            toRotation.y = 0;
            Vector3 fromRotation = transform.forward;

            var angleToTarget = Vector3.Angle(fromRotation, toRotation);

            if (angleToTarget <= ROTATION_COMPLETE_THRESHOLD)
                sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING, execData.target));
            else
                sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_ROTATION_INCOMPLETE, execData.target));
        }
        else
        {
            // error occured - no target to rotate
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING, execData.target));
        }
    }

    private void checkIfTargetIsReached(PlayerFsmExecData execData)
    {
        const float dist_threshold = 0.5f;

        if (execData.target != null)
        {
            if (!navigator.pathPending && (navigator.remainingDistance <= dist_threshold))
                sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_REACHED_TARGET, execData.target));
            else
                sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_NOT_REACHED_TARGET, execData.target));
        }
        else
        {
            // error occured - no target to reach
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_REACHED_TARGET, execData.target));
        }
    }

    private void isUsingEnded(PlayerFsmExecData execData)
    {
        //TODO: isUsingEnded: implement and remove temporary implementation!!!

        if (Input.GetKeyDown(KeyCode.Space))
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_USING_ENDED, execData.target));
        else
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_USING_NOT_ENDED, execData.target));
    }

    private void setIdleAnimation(PlayerFsmExecData execData)
    {
        playAnim(PlayerAnimation.IDLE);
        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_IDLE_ANIMATION_SET, execData.target));
    }

    private void setMovingAnimation(PlayerFsmExecData execData)
    {
        playAnim(PlayerAnimation.WALKING);
        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_WALKING_ANIMATION_SET, execData.target));
    }

    private void setUsingAnimation(PlayerFsmExecData execData)
    {
        playAnim(PlayerAnimation.USING);
        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_USING_ANIMATION_SET, execData.target));
    }

    private void startMovingToTarget(PlayerFsmExecData execData)
    {
        if (execData.target != null)
        {
            navigator.destination = execData.target.transform.position;
            navigator.isStopped = false;

            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_DESTINATION_SET, execData.target));
        }
        else
        {
            // error occured - no target to reach
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_REACHED_TARGET, execData.target));
        }
    }

    private void rotateToTarget(PlayerFsmExecData execData)
    {
        const float ROTATION_SPEED = 5.0f;

        if (execData.target != null)
        {
            var targetRotation = Quaternion.LookRotation(execData.target.transform.position - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATION_SPEED * Time.fixedDeltaTime);

            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_ROTATION_PROCEEDED, execData.target));
        }
        else
        {
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING, execData.target));
        }
    }

    private void startUsingTarget(PlayerFsmExecData execData)
    {
        // TODO: startUsingTarget: implement
    }

    #endregion

    private void fillStateMachinesTransitions()
    {
        // TODO: complete!!!

        /*                              current state                                      event                                           next state                           action */

        // IDLE transitions set
        playerFSM.addTransition(PlayerStateMachine.State.IDLE,       PlayerStateMachine.Event.PLAYER_STARTED_TURNING,            PlayerStateMachine.State.TURNING,            checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.IDLE,       PlayerStateMachine.Event.PLAYER_STARTED_MOVING,             PlayerStateMachine.State.MOVING,             checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.IDLE,       PlayerStateMachine.Event.PLAYER_STARTED_INTERACTIVE_SEARCH, PlayerStateMachine.State.INTERACTIVE_SEARCH, checkRotation);

        // TURNING transitions set
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,    PlayerStateMachine.Event.PLAYER_ROTATION_INCOMPLETE,        PlayerStateMachine.State.TURNING,            rotateToTarget);
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,    PlayerStateMachine.Event.PLAYER_ROTATION_PROCEEDED,         PlayerStateMachine.State.TURNING,            checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,    PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING,         PlayerStateMachine.State.IDLE,               setPlayerIdle);

        // WALKING transitions set
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,     PlayerStateMachine.Event.PLAYER_ROTATION_INCOMPLETE,        PlayerStateMachine.State.MOVING,             rotateToTarget);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,     PlayerStateMachine.Event.PLAYER_ROTATION_PROCEEDED,         PlayerStateMachine.State.MOVING,             checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,     PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING,         PlayerStateMachine.State.MOVING,             setMovingAnimation);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,     PlayerStateMachine.Event.PLAYER_WALKING_ANIMATION_SET,      PlayerStateMachine.State.MOVING,             startMovingToTarget);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,     PlayerStateMachine.Event.PLAYER_DESTINATION_SET,            PlayerStateMachine.State.MOVING,             checkIfTargetIsReached);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,     PlayerStateMachine.Event.PLAYER_NOT_REACHED_TARGET,         PlayerStateMachine.State.MOVING,             checkIfTargetIsReached);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,     PlayerStateMachine.Event.PLAYER_REACHED_TARGET,             PlayerStateMachine.State.IDLE,               setPlayerIdle);

        // INTERACTIVE_SEARCH transitions set

        // ANY_STATE transitions set

    }

    #endregion
}
