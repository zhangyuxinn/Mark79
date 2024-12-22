using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/State/ProcessFour",fileName = "ProcessFourState_")]
public class ProcessFourState : CharacterState
{
    private GameModeStateMachine gameModeStateMachine;


    public override void Initialize(StateMachine SM)
    {
        
        gameModeStateMachine = SM as GameModeStateMachine;
    }

    public override void OnEnter()
    {
        GameMode.Instance.StartProcessFour();
    }

    public override void OnUpdate()
    {
        GameMode.Instance.UpdateProcessFour();
    }

    public override void OnEnd()
    {
        GameMode.Instance.EndProcessFour();

    }
}
