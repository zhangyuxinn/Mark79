using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/State/Protect",fileName = "ProtectState_")]
public class ProtectState : CharacterState
{
    private AIPlayerStateMachine aiPlayerStateMachine;


    public override void Initialize(StateMachine SM)
    {
        
        aiPlayerStateMachine = SM as AIPlayerStateMachine;
    }

    public override void OnEnter()
    {
        aiPlayerStateMachine.aiPlayerController.StartProtect();
    }

    public override void OnUpdate()
    {
        aiPlayerStateMachine.aiPlayerController.ProtectHome();
    }

    public override void OnEnd()
    {
    }
}
