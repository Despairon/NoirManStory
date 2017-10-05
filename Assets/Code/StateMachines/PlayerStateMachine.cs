using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public delegate bool PlayerFSM_TransitionRule();

public class PlayerStateMachine : StateMachine<PlayerFSM_TransitionRule, GameObject>
{
    #region public_members

    public enum State
    {
        ANY_STATE,

        IDLE,
        TURNING,
        MOVING,
        USING
    }

    public PlayerStateMachine(State initialState) : base(initialState) { }

    public override void execute(GameObject targetObject)
    {
        // any state transitions check
        var anyStateTransitions = transitionsTable.FindAll(transition => transition.currentState == State.ANY_STATE as Enum);

        if (anyStateTransitions != null)
            foreach(var transition in anyStateTransitions)
                if (transition.transitionRule() == true)
                {
                    // proceed to next state
                    currentState = transition.nextState;
                    return;
                }

        // ordinary state transitions check
        var stateTransitions = transitionsTable.FindAll(transition => transition.currentState == currentState);

        if (stateTransitions != null)
            foreach(var transition in stateTransitions)
            {
                if (transition.transitionRule() == true)
                    currentState = transition.nextState;

                if (transition.stateAction != null)
                    transition.stateAction(targetObject);
            }
    }

    #endregion
}

