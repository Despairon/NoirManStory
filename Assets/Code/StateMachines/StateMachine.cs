using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StateMachine<TransitionRuleType, StateActionType>
{
    #region private_members

    private Enum _currentState;

    #endregion

    #region protected_members

    protected class TransitionNode
    {
        public TransitionNode(Enum currentState, TransitionRuleType transitionRule, Enum nextState, StateActionType stateAction)
        {
            this.currentState   = currentState;
            this.transitionRule = transitionRule;
            this.nextState      = nextState;
            this.stateAction    = stateAction;
        }

        public readonly Enum               currentState;
        public readonly TransitionRuleType transitionRule;
        public readonly Enum               nextState;
        public readonly StateActionType    stateAction;
    }

    protected List<TransitionNode> transitionsTable;

    #endregion

    #region public_members

    public Enum currentState
    {
        get           { return _currentState;  }
        protected set { _currentState = value; }
    }

    public StateMachine()
    {
        transitionsTable = new List<TransitionNode>();
    }

    public void addNode(Enum currentState, TransitionRuleType transitionRule, Enum nextState, StateActionType stateAction)
    {
        var node = new TransitionNode(currentState, transitionRule, nextState, stateAction);

        transitionsTable.Add(node);
    }

    public abstract void execute(object data);

    #endregion
}
