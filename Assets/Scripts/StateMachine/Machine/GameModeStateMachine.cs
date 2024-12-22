using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeStateMachine : StateMachine
{

    [SerializeField] private ProcessFourState processFourState;
    protected override void Init()
    {
        processFourState.Initialize(this);
        states.Add(StateType.ProcessFour,processFourState);
    }

    protected override void Start()
    {
        Init();
    }
}
