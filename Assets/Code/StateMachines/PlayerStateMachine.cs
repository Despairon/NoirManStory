using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerFsmExecData : EventData
{
    public PlayerFsmExecData(PlayerStateMachine.Event evt, GameObject target) : base()
    {
        this.evt    = evt;
        this.target = target;
    }

    public readonly PlayerStateMachine.Event evt;
    public readonly GameObject               target;
}

public class PlayerStateMachine : StateMachine<PlayerStateMachine.Event, PlayerFsmExecData>
{
    #region public_members

    public enum State
    {
        ANY_STATE,

        IDLE,
        TURNING,
        WALKING,
        INTERACTIVE_SEARCH
    }

    public enum Event
    {
        PLAYER_STARTED_IDLING,
        PLAYER_STARTED_TURNING,
        PLAYER_STARTED_MOVING,
        PLAYER_STARTED_INTERACTIVE_SEARCH,
        ROTATION_COMPLETE,
        ROTATION_INCOMPLETE,
        ROTATION_PROCEEDED,
        TARGET_REACHED,
        TARGET_NOT_REACHED,
        IDLE_ANIMATION_SET,
        WALKING_ANIMATION_SET,
        USING_ANIMATION_SET,
        PLAYER_USING_ENDED,
        PLAYER_USING_NOT_ENDED
    }

    public PlayerStateMachine(State initialState) : base(initialState) { }

    public override void execute(PlayerFsmExecData execData)
    {
        Debug.Log("execution for event " + execData.evt);

        var transition = transitionsTable.Find(tr => ((tr.currentState.Equals(currentState)) || (tr.currentState.Equals(State.ANY_STATE as Enum))
                                                  &&  (tr.transitionRule == execData.evt)));

        if (transition != null)
        {
            // TODO: delete debug log
            Debug.Log("transition from " + currentState.ToString() + " to " + transition.nextState.ToString() + " by event: " + transition.transitionRule.ToString());

            // perform exit action
            if (transition.stateAction != null)
                transition.stateAction(execData);

            currentState = transition.nextState;
        }
        else
            Debug.Log("no such transition");
    }

    #endregion
}

