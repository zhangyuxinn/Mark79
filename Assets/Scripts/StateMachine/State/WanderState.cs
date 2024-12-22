using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/State/Wander",fileName = "WanderState_")]
public class WanderState : CharacterState
{
    private AIPlayerStateMachine aiPlayerStateMachine;


    public override void Initialize(StateMachine SM)
    {
        
        aiPlayerStateMachine = SM as AIPlayerStateMachine;
    }

    public override void OnEnter()
    {
        aiPlayerStateMachine.aiPlayerController.StartWander();
    }

    public override void OnUpdate()
    {
        aiPlayerStateMachine.aiPlayerController.UpdateWander();
    }

    public override void OnEnd()
    {
        aiPlayerStateMachine.aiPlayerController.EndWander();
    }
}
