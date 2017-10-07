using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public delegate bool PlayerFSM_TransitionRule(GameObject targetObject);

public class PlayerStateMachine : StateMachine<PlayerFSM_TransitionRule, GameObject>
{
    #region public_members

    public enum State
    {
        ANY_STATE,

        IDLE,
        TURNING,
        END_TURNING,
        SET_MOVING_ANIMATION,
        START_MOVING,
        MOVING,
        SET_IDLE_ANIMATION,
        END_MOVING,
        SET_USING_ANIMATION,
        START_USING,
        USING,
        END_USING,
    }

    public PlayerStateMachine(State initialState) : base(initialState) { }

    public override void execute(GameObject target)
    {
        // any state transitions check
        var anyStateTransitions = transitionsTable.FindAll(transition => transition.currentState.Equals(State.ANY_STATE as Enum));

        if (anyStateTransitions != null)
            foreach (var transition in anyStateTransitions)
            {
                if (transition.transitionRule != null)
                {
                    if (transition.transitionRule(target) == true)
                    {
                        // proceed to next state
                        currentState = transition.nextState;
                        return;
                    }
                }
            }

        // ordinary state transitions check
        var stateTransitions = transitionsTable.FindAll(transition => transition.currentState.Equals(currentState) );

        if (stateTransitions != null)
            foreach(var transition in stateTransitions)
            {
                // perform state action
                if (transition.stateAction != null)
                    transition.stateAction(target);

                if (transition.transitionRule != null)
                {
                    if (transition.transitionRule(target) == true)
                    {
                        // proceed to next state
                        currentState = transition.nextState;
                        break;
                    }
                }
            }
    }

    #endregion
}

