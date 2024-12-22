using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerStateMachine : StateMachine
{
    public AIPlayerController aiPlayerController;

    public bool isFight;

    private event Action fightTrigger ;
    
    [Header("基础StateSO")] 
    [SerializeField] private WanderState wanderState;
    [SerializeField] private AIFightState aiFightState;
    [SerializeField] private ProtectState protectState ;

    protected override void Start()
    {
        Init();
        aiPlayerController = character.GetComponent<AIPlayerController>();
        Debug.Log(aiPlayerController);
        TransitionState(StateType.Wander);
    }
    
    

    protected override void Init()
    {
        base.Init();
        wanderState.Initialize(this);
        aiFightState.Initialize(this);
        protectState.Initialize(this);
        states.Add(StateType.Wander,wanderState);
        states.Add(StateType.AIFight,aiFightState);
        states.Add(StateType.Protect,protectState);
    }
    
    public  void StartFight(StateType stateType,GameObject target)
    {
        isFight = true;
        fightState.fightTarget = target;
        StartCoroutine( fightState.StartAIShootBullet());
        
    }
    
}
