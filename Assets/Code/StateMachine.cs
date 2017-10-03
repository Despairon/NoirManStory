using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSM_TransitionState
{
    IDLE,
    ANY_STATE,
    PLAYER_TURNING,
    PLAYER_MOVING,
    PLAYER_USING,

    // add all user states here
}

public class StateMachine : MonoBehaviour
{
#region private_members

    private FSM_TransitionState _state;
    private bool                _isActive;

    private class TransitionNode
    {
        public TransitionNode(FSM_TransitionState currentState, FSM_TransitionRule transitionRule, FSM_TransitionState nextState, FSM_StateAction stateAction)
        {
            this.currentState   = currentState;
            this.transitionRule = transitionRule;
            this.nextState      = nextState;
            this.stateAction    = stateAction;
        }

        public FSM_TransitionState currentState;
        public FSM_TransitionRule  transitionRule;
        public FSM_TransitionState nextState;
        public FSM_StateAction     stateAction;
    }

    private List<TransitionNode> transitionsTable;

    private void execute()
    {
        foreach (var node in transitionsTable.FindAll(tr => (tr.currentState == state) || (tr.currentState == FSM_TransitionState.ANY_STATE)))
        {
            if (node.transitionRule() == true)
            {
                // transit to next state if transition rule is true
                state = node.nextState;
                break;
            }
            else
            {
                if (node.stateAction != null)
                    node.stateAction();
            }
        }        
    }

#endregion

#region public members

    public delegate bool FSM_TransitionRule();
    public delegate void FSM_StateAction();

    public StateMachine()
    {
        transitionsTable = new List<TransitionNode>();
    }

    public FSM_TransitionState state
    {
        get         { return _state;  }
        private set { _state = value; }
    }

    public bool isActive
    {
        get         { return _isActive;  }
        private set { _isActive = value; }
    }

    public void startExecution()
    {
        state    = FSM_TransitionState.IDLE;
        isActive = true;
    }

    public void stopExecution()
    {
        state    = FSM_TransitionState.IDLE;
        isActive = false;
    }

    public void addNode(FSM_TransitionState currentState, FSM_TransitionRule transitionRule, FSM_TransitionState nextState, FSM_StateAction stateAction)
    {
        var node = new TransitionNode(currentState, transitionRule, nextState, stateAction);

        transitionsTable.Add(node);
    }

#endregion

#region unity_defined_methods

    void Start()
    {
        state    = FSM_TransitionState.IDLE;
        isActive = false;
    }

    void FixedUpdate()
    {
        if (isActive)
            execute();
    }

#endregion
}
