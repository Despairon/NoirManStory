using UnityEngine;

public partial class Player
{
    #region private_members

    private PlayerStateMachine playerFSM;

    private bool checkExecDataForNulls(PlayerFsmExecData execData)
    {
        if (execData != null)
        {
            if (execData.interactionParams != null)
                if ((execData.interactionParams.interactionPoint != null)
                &&  (execData.interactionParams.obj              != null))
                    return true;
        }

        return false;
    }

    #region player_fsm_state_actions

    private void emptyAction(PlayerFsmExecData execData)
    {
        // do nothing
    }

    private void setPlayerIdle(PlayerFsmExecData execData)
    {
        playerFSM.reset();

        navigator.destination = transform.position;
        navigator.isStopped   = true;

        setIdleAnimation(execData);
    }

    private void checkRotation(PlayerFsmExecData execData)
    {
        const float ROTATION_COMPLETE_THRESHOLD = 5.0f; // angles

        if (checkExecDataForNulls(execData))
        {
            Vector3 toRotation = (execData.interactionParams.interactionPoint - transform.position).normalized;
            toRotation.y = 0;
            Vector3 fromRotation = transform.forward;

            var angleToTarget = Vector3.Angle(fromRotation, toRotation);

            if (angleToTarget <= ROTATION_COMPLETE_THRESHOLD)
                sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING, execData.interactionParams));
            else
                sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_ROTATION_INCOMPLETE, execData.interactionParams));
        }
        else
        {
            // error occured - no target to rotate
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING, execData.interactionParams));
        }
    }

    private void checkIfTargetIsReached(PlayerFsmExecData execData)
    {
        const float dist_threshold = 0.5f;

        if (checkExecDataForNulls(execData))
        {
            if (!navigator.pathPending && (navigator.remainingDistance <= dist_threshold))
				sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_REACHED_TARGET, execData.interactionParams));
            else
				sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_NOT_REACHED_TARGET, execData.interactionParams));
        }
        else
        {
            // error occured - no target to reach
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_REACHED_TARGET, execData.interactionParams));
        }
    }

    private void isUsingEnded(PlayerFsmExecData execData)
    {
        //TODO: isUsingEnded: implement and remove temporary implementation!!!

        if (Input.GetKey(KeyCode.Space))
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_USING_ENDED, execData.interactionParams));
        else
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_USING_NOT_ENDED, execData.interactionParams));
    }

    private void setIdleAnimation(PlayerFsmExecData execData)
    {
        playAnim(PlayerAnimation.IDLE);
		sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_IDLE_ANIMATION_SET, execData.interactionParams));
    }

    private void setMovingAnimation(PlayerFsmExecData execData)
    {
        playAnim(PlayerAnimation.WALKING);
		sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_WALKING_ANIMATION_SET, execData.interactionParams));
    }

    private void setUsingAnimation(PlayerFsmExecData execData)
    {
        playAnim(PlayerAnimation.USING);
		sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_USING_ANIMATION_SET, execData.interactionParams));
    }

    private void rotateToTarget(PlayerFsmExecData execData)
    {
        const float ROTATION_SPEED = 5.0f;

        if (checkExecDataForNulls(execData))
        {
            var targetRotation = Quaternion.LookRotation(execData.interactionParams.interactionPoint - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATION_SPEED * Time.fixedDeltaTime);

			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_ROTATION_PROCEEDED, execData.interactionParams));
        }
        else
        {
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING, execData.interactionParams));
        }
    }

    private void startMovingToTarget(PlayerFsmExecData execData)
    {
        if (checkExecDataForNulls(execData))
        {
            navigator.destination = execData.interactionParams.interactionPoint;
            navigator.isStopped = false;

			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_DESTINATION_SET, execData.interactionParams));
        }
        else
        {
            // error occured - no target to reach
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_REACHED_TARGET, execData.interactionParams));
        }
    }

    private void checkIfInterObjIsInRange(PlayerFsmExecData execData)
    {
		if (execData.interactionParams != null)
        {
			EventsManager.instance.sendEventToObject(execData.interactionParams.obj.name, EventID.PLAYER_TO_INTER_OBJ_IS_PLAYER_IN_RANGE, execData);
        }
        else
        {
            // error occured - no target to use
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_INTER_OBJECT_OUT_OF_RANGE, execData.interactionParams));
        }
    }

    private void startInteractiveSearch(PlayerFsmExecData execData)
    {
        if (checkExecDataForNulls(execData))
        {
            // TODO: startInteractiveSearch: do stuff
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_INTERACTIVE_DIALOG_OPENED, execData.interactionParams));
        }
        else
        {
            // error occured - nothing to use
			sendEventToSelf(new PlayerFsmExecData(PlayerStateMachine.Event.PLAYER_USING_ENDED, execData.interactionParams));
        }
    }

    #endregion

    private void fillStateMachinesTransitions()
    {
        // TODO: complete!!!

        /*                              current state                                      event                                           next state                           action */

        // IDLE transitions set
        playerFSM.addTransition(PlayerStateMachine.State.IDLE,               PlayerStateMachine.Event.PLAYER_STARTED_TURNING,            PlayerStateMachine.State.TURNING,            checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.IDLE,               PlayerStateMachine.Event.PLAYER_STARTED_MOVING,             PlayerStateMachine.State.MOVING,             checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.IDLE,               PlayerStateMachine.Event.PLAYER_STARTED_INTERACTIVE_SEARCH, PlayerStateMachine.State.INTERACTIVE_SEARCH, checkRotation);

        // TURNING transitions set
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,            PlayerStateMachine.Event.PLAYER_ROTATION_INCOMPLETE,        PlayerStateMachine.State.TURNING,            rotateToTarget);
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,            PlayerStateMachine.Event.PLAYER_ROTATION_PROCEEDED,         PlayerStateMachine.State.TURNING,            checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.TURNING,            PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING,         PlayerStateMachine.State.IDLE,               setPlayerIdle);

        // WALKING transitions set
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,             PlayerStateMachine.Event.PLAYER_ROTATION_INCOMPLETE,        PlayerStateMachine.State.MOVING,             rotateToTarget);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,             PlayerStateMachine.Event.PLAYER_ROTATION_PROCEEDED,         PlayerStateMachine.State.MOVING,             checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,             PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING,         PlayerStateMachine.State.MOVING,             setMovingAnimation);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,             PlayerStateMachine.Event.PLAYER_WALKING_ANIMATION_SET,      PlayerStateMachine.State.MOVING,             startMovingToTarget);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,             PlayerStateMachine.Event.PLAYER_DESTINATION_SET,            PlayerStateMachine.State.MOVING,             checkIfTargetIsReached);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,             PlayerStateMachine.Event.PLAYER_NOT_REACHED_TARGET,         PlayerStateMachine.State.MOVING,             checkIfTargetIsReached);
        playerFSM.addTransition(PlayerStateMachine.State.MOVING,             PlayerStateMachine.Event.PLAYER_REACHED_TARGET,             PlayerStateMachine.State.IDLE,               setPlayerIdle);
        
        // INTERACTIVE_SEARCH transitions set
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_ROTATION_INCOMPLETE,        PlayerStateMachine.State.INTERACTIVE_SEARCH, rotateToTarget);
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_ROTATION_PROCEEDED,         PlayerStateMachine.State.INTERACTIVE_SEARCH, checkRotation);
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_COMPLETED_ROTATING,         PlayerStateMachine.State.INTERACTIVE_SEARCH, checkIfInterObjIsInRange);
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_INTER_OBJECT_OUT_OF_RANGE,  PlayerStateMachine.State.IDLE,               setPlayerIdle);
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_INTER_OBJECT_IN_RANGE,      PlayerStateMachine.State.INTERACTIVE_SEARCH, setUsingAnimation);
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_USING_ANIMATION_SET,        PlayerStateMachine.State.INTERACTIVE_SEARCH, startInteractiveSearch);
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_INTERACTIVE_DIALOG_OPENED,  PlayerStateMachine.State.INTERACTIVE_SEARCH, isUsingEnded);
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_USING_NOT_ENDED,            PlayerStateMachine.State.INTERACTIVE_SEARCH, isUsingEnded);
        playerFSM.addTransition(PlayerStateMachine.State.INTERACTIVE_SEARCH, PlayerStateMachine.Event.PLAYER_USING_ENDED,                PlayerStateMachine.State.IDLE,               setPlayerIdle);
        
        // ANY_STATE transitions set

    }

    #endregion
}
