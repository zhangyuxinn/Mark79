using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

    public IState currentState;
    public StateType currentStateType;
    public Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

    [Header("基础StateSO")] 
    [SerializeField] private IdleState initIdleState;
    [SerializeField] private FightState initFightState;

    [SerializeField] private IdleState idleState;
    [SerializeField] protected FightState fightState;

    [Header("State需要处理的组件")]
    public Animator animator;

    [HideInInspector]public GameObject character;
    protected virtual void Start()
    {
        Init();
        TransitionState(StateType.Idle);
    }

    protected virtual void Update()
    {
        currentState.OnUpdate();
    }

    protected virtual void Init()
    {
        fightState =initFightState.CopyNewState();
        idleState.Initialize(this);
        fightState.Initialize(this);
        states.Add(StateType.Idle,idleState);
        states.Add(StateType.Fight,fightState);
        //还要获取动画器,初始化，
        character = gameObject;
    }

    public void TransitionState(StateType stateType)//技能释放等效果的执行接口，可以将其重载成多种情况，例如读入释放的技能序号等。
    {
        if (currentState != null)
        {
            currentState.OnEnd();
        }

        currentState = states[stateType];
        currentStateType = stateType;
        Debug.Log("State Switch!!!"+currentState);
        
        currentState.OnEnter();
    }
    public virtual void TransitionState(StateType stateType,GameObject target)//重载成攻击，传入敌人
    {
        if (currentState != null)
        {
            currentState.OnEnd();
        }
        
        if (stateType == StateType.Fight)
        {
            if (currentStateType != StateType.Fight)
            {
                currentState = states[stateType];
                currentStateType = stateType;
                fightState.fightTarget = target;
                currentState.OnEnter();
                StartCoroutine( fightState.StartShootBullet());
            }else
            {
                fightState.fightTarget = target;
            }
        }
    }
    



}

public enum StateType
{
    Idle,
    Fight,
    Wander,
    Protect,
    AIFight,
    ProcessOne,
    ProcessTwoAndThree,
    ProcessFour
}