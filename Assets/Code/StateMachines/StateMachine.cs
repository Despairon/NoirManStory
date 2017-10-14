using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StateMachine<TransitionRuleType, StateActionArgType>
{
    #region private_members

    private Enum _currentState;
    private Enum _initialState;

    #endregion

    #region protected_members

    protected class TransitionNode
    {
        public TransitionNode(Enum currentState, TransitionRuleType transitionRule, Enum nextState, StateAction stateAction)
        {
            this.currentState   = currentState;
            this.transitionRule = transitionRule;
            this.nextState      = nextState;
            this.stateAction    = stateAction;
        }

        public readonly Enum               currentState;
        public readonly TransitionRuleType transitionRule;
        public readonly Enum               nextState;
        public readonly StateAction        stateAction;
    }

    protected List<TransitionNode> transitionsTable;

    #endregion

    #region public_members

    public Enum currentState
    {
        get           { return _currentState;  }
        protected set { _currentState = value; }
    }

    public delegate void StateAction(StateActionArgType arg);

    public StateMachine(Enum initialState)
    {
        transitionsTable = new List<TransitionNode>();

        _initialState = initialState;

        reset();
    }

    public void addTransition(Enum currentState, TransitionRuleType transitionRule, Enum nextState, StateAction stateAction)
    {
        var node = new TransitionNode(currentState, transitionRule, nextState, stateAction);

        transitionsTable.Add(node);
    }

    public abstract void execute(StateActionArgType data);

    public void reset()
    {
        currentState = _initialState;
    }

    #endregion
}
