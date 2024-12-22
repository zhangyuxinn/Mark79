using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : ScriptableObject,IState
{
    public StateMachine stateMachine;
    
    
    public virtual void Initialize(StateMachine SM)
    {
        stateMachine = SM;
    }
     public virtual void OnEnter()
    {
        
    }

    public virtual void OnUpdate()
    {
        
    }

    public virtual void OnEnd()
    {
        
    }


}
