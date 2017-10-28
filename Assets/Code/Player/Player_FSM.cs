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
        resetTarget();

        setIdleAnimation(execData);

        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_STARTED_IDLING, execData.target));
    }

    private void checkRotation(PlayerFsmExecData execData)
    {
        const float ROTATION_COMPLETE_THRESHOLD = 5; // angles

        if (execData.target != null)
        {
            Vector3 toRotation = (execData.target.transform.position - transform.position).normalized;
            toRotation.y = 0;
            Vector3 fromRotation = transform.forward;

            var angleToTarget = Vector3.Angle(fromRotation, toRotation);

            if (angleToTarget <= ROTATION_COMPLETE_THRESHOLD)
                sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.ROTATION_COMPLETE, execData.target));
            else
                sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.ROTATION_INCOMPLETE, execData.target));
        }
        else
        {
            // error occured - no target to rotate
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.ROTATION_COMPLETE, execData.target));
        }
    }

    private void checkIfTargetIsReached(PlayerFsmExecData execData)
    {
        const float dist_threshold = 0.5f;

        if (!navigator.pathPending && (navigator.remainingDistance <= dist_threshold))
        {
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.TARGET_REACHED, execData.target));
        }
        else
            sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.TARGET_NOT_REACHED, execData.target));
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
        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.IDLE_ANIMATION_SET, execData.target));
    }

    private void setMovingAnimation(PlayerFsmExecData execData)
    {
        playAnim(PlayerAnimation.WALKING);
        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.WALKING_ANIMATION_SET, execData.target));
    }

    private void setUsingAnimation(PlayerFsmExecData execData)
    {
        playAnim(PlayerAnimation.USING);
        sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.USING_ANIMATION_SET, execData.target));
    }

    private void startMovingToTarget(PlayerFsmExecData execData)
    {
        navigator.destination = execData.target.transform.position;
        navigator.isStopped = false;
    }

    private void rotateToTarget(PlayerFsmExecData execData)
    {
        const float ROTATION_SPEED = 5.0f;

        if (execData.target != null)
        {
            var targetRotation = Quaternion.LookRotation(execData.target.transform.position - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATION_SPEED * Time.fixedDeltaTime);
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

        /*                              current state                                      event                                           next state                  action */

        // IDLE transitions set
        playerFSM.addTransition(PlayerStateMachine.State.IDLE,         PlayerStateMachine.Event.PLAYER_STARTED_TURNING,    PlayerStateMachine.State.TURNING,       checkRotation);

        // TURNING transitions set
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,      PlayerStateMachine.Event.ROTATION_COMPLETE,         PlayerStateMachine.State.IDLE,          setPlayerIdle);
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,      PlayerStateMachine.Event.ROTATION_INCOMPLETE,       PlayerStateMachine.State.TURNING,       rotateToTarget);
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,      PlayerStateMachine.Event.ROTATION_PROCEEDED,        PlayerStateMachine.State.TURNING,       checkRotation);

        // WALKING transitions set

        // INTERACTIVE_SEARCH transitions set

        // ANY_STATE transitions set
        playerFSM.addTransition(PlayerStateMachine.State.ANY_STATE,    PlayerStateMachine.Event.PLAYER_STARTED_IDLING,      PlayerStateMachine.State.IDLE,         emptyAction);
    }

    #endregion
}
