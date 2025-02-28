using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorStateMachine 
{
    //State machine
    private EditorState currentState = new CellEditState();
    public EditorState CurrentState => currentState;

    public void ChangeState(EditorState targetState)
    {
        if (currentState.GetType() != targetState.GetType())
        {
            currentState.Exit();
            currentState = targetState;
            currentState.Enter();
        }
    }
}
