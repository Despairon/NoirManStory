﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class PlayerFsmExecData : EventData
{
	public PlayerFsmExecData(PlayerStateMachine.Event evt, GameObject target, GameObject originalObject) : base()
    {
        this.evt            = evt;
        this.target         = target;
		this.originalObject = originalObject;
    }

    public readonly PlayerStateMachine.Event evt;
    public readonly GameObject               target;
	public readonly GameObject               originalObject;
}

public class PlayerStateMachine : StateMachine<PlayerStateMachine.Event, PlayerFsmExecData>
{
    #region public_members

    public enum State
    {
        ANY_STATE,

        IDLE,
        TURNING,
        MOVING,
        INTERACTIVE_SEARCH
    }

    public enum Event
    {
        PLAYER_STARTED_TURNING,
        PLAYER_STARTED_MOVING,
        PLAYER_STARTED_INTERACTIVE_SEARCH,
        PLAYER_COMPLETED_ROTATING,
        PLAYER_ROTATION_INCOMPLETE,
        PLAYER_ROTATION_PROCEEDED,
        PLAYER_DESTINATION_SET,
        PLAYER_NOT_REACHED_TARGET,
        PLAYER_REACHED_TARGET,
        PLAYER_INTERACTIVE_DIALOG_OPENED,
        PLAYER_INTER_OBJECT_IN_RANGE,
        PLAYER_INTER_OBJECT_OUT_OF_RANGE,
        PLAYER_IDLE_ANIMATION_SET,
        PLAYER_WALKING_ANIMATION_SET,
        PLAYER_USING_ANIMATION_SET,
        PLAYER_USING_ENDED,
        PLAYER_USING_NOT_ENDED
    }

    public PlayerStateMachine(State initialState) : base(initialState) { }

    public override void execute(PlayerFsmExecData execData)
    {
        var transition = transitionsTable.Find(tr => ( ((tr.currentState.Equals(currentState)) || (tr.currentState.Equals(State.ANY_STATE as Enum)))
                                                  &&    (tr.transitionRule == execData.evt)));

        if (transition != null)
        {
            // perform exit action
            if (transition.stateAction != null)
                transition.stateAction(execData);

            if (!currentState.Equals(transition.nextState))
            {
                //TODO: remove log when not needed anymore
                Debug.Log("transition from " + currentState.ToString() + " to " + transition.nextState.ToString() + " by event: " + transition.transitionRule.ToString());
                currentState = transition.nextState;
            }
        }
    }

    #endregion
}

