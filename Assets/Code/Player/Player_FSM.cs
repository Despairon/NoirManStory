using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : MonoBehaviour
{
    #region private_members

    private PlayerStateMachine playerTurningFSM;
    private PlayerStateMachine playerMovingFSM;
    private PlayerStateMachine playerUsingFSM;

    private Dictionary<State, PlayerStateMachine> stateMachineMap;

    private void resetStateMachines()
    {
        playerTurningFSM.reset();
        playerMovingFSM.reset();
        playerUsingFSM.reset();
    }

    private void executeFsmForState(State state)
    {
        try
        {
            var stateMachine = stateMachineMap[state];

            if (stateMachine != null)
                stateMachine.execute(targetObject);
        }
        catch (KeyNotFoundException)
        {
            // do nothing
        }
    }

    private void fillStateMachineMap()
    {
        stateMachineMap.Add(State.TURNING, playerTurningFSM);
        stateMachineMap.Add(State.MOVING,  playerMovingFSM);
        stateMachineMap.Add(State.USING,   playerUsingFSM);
    }

    private void fillStateMachinesTransitions()
    {
        // TODO: complete state machines -> replace null's with callbacks!!!

        // player turning state machine transitions table section
        playerTurningFSM.addTransition(PlayerStateMachine.State.IDLE,    null, PlayerStateMachine.State.TURNING, null);
        playerTurningFSM.addTransition(PlayerStateMachine.State.TURNING, null, PlayerStateMachine.State.IDLE,    null);

        // player moving state machine transitions table section
        playerMovingFSM.addTransition(PlayerStateMachine.State.IDLE,    null, PlayerStateMachine.State.TURNING, null);
        playerMovingFSM.addTransition(PlayerStateMachine.State.TURNING, null, PlayerStateMachine.State.MOVING,  null);
        playerMovingFSM.addTransition(PlayerStateMachine.State.MOVING,  null, PlayerStateMachine.State.IDLE,    null);

        // player using state machine transitions table section
        playerUsingFSM.addTransition(PlayerStateMachine.State.IDLE,    null, PlayerStateMachine.State.TURNING, null);
        playerUsingFSM.addTransition(PlayerStateMachine.State.TURNING, null, PlayerStateMachine.State.MOVING,  null);
        playerUsingFSM.addTransition(PlayerStateMachine.State.MOVING,  null, PlayerStateMachine.State.USING,   null);
        playerUsingFSM.addTransition(PlayerStateMachine.State.USING,   null, PlayerStateMachine.State.IDLE,    null);

    }

    #endregion


    #region public_members

    #endregion
}
