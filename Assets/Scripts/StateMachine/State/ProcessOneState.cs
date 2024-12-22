using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/State/ProcessOne",fileName = "ProcessOneState_")]
public class ProcessOneState : CharacterState
{
    private GameModeStateMachine gameModeStateMachine;


    public override void Initialize(StateMachine SM)
    {
        
        gameModeStateMachine = SM as GameModeStateMachine;
    }

    public override void OnEnter()
    {
        GameMode.Instance.StartProcessOne();
    }

    public override void OnUpdate()
    {
        GameMode.Instance.UpdateProcessOne();
    }

    public override void OnEnd()
    {
        GameMode.Instance.EndProcessOne();

    }
}
