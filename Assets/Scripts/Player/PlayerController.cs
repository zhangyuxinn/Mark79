using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase
{
    [SerializeField]private PlayerInput playerInput;
    [SerializeField]private AStarFind aStarFind;
    
    [Header("事件订阅")]
    [SerializeField] private Vector3EventChannel mouseDownEventChannel;
    [SerializeField] private StatChangeEventChannel statChangeEventChannelPlayerHealth;
    [SerializeField] private StatChangeEventChannel statChangeEventChannelPlayerKill;
    [SerializeField] private StatChangeEventChannel statChangeEventChannelPlayerMana;
    [SerializeField] private FightTargetEventChannel fightTargetEventChannel;

    [Header("人物属性")] 
    protected PlayerStat playerStat;
    [SerializeField] protected float moveSpeed=2f;
    //[SerializeField] private float attackInterval = 1f;
    
    private bool isMove;
    private List<Vector3> path;

    protected override void Start()
    {
        base.Start();
        playerStat = baseStat as PlayerStat;
        path=new List<Vector3>(1000);
        OnManaChange(playerStat.currentMana,team);
        PlayerInit();
    }

    public void PlayerInit()
    {
        HealthInit();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        mouseDownEventChannel.AddListener(OnStartMove);
        fightTargetEventChannel.AddListener(FightAimingTarget);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        mouseDownEventChannel.RemoveListener(OnStartMove);
        fightTargetEventChannel.RemoveListener(FightAimingTarget);

    }

    protected override void Update()
    {
        base.Update();
        if (isMove)
        {
            MoveAlongPath();
        }
        
    }

    #region Move

    public void OnStartMove(Vector3 pos)
    {
        isMove = true;
        GetMovePath(pos);
    }

    private void GetMovePath(Vector3 pos)
    { 
        Vector3[] list={ pos};
        path = new List<Vector3>(list); 
        if (path.Count == 0)
        {
            Debug.Log("00000");
        }
    }

    private void MoveAlongPath()
    {
        if (path.Count != 0)
        {
            Vector3 tmpTarget = path[0];
            
            //向当前节点移动
            Vector3 dir = (this.transform.position - tmpTarget).normalized;
            rb.velocity=-dir*moveSpeed;
            if ((transform.position - tmpTarget).sqrMagnitude < 1)
            {
                
                rb.velocity=Vector3.zero;
                path.RemoveAt(0);
            }
        }
        else
        {
            isMove = false;
            
        }
    }

    #endregion
    
    #region buff
    public override void OnBuff(BuffBase buffBase)
    {
        Debug.Log(buffBase.buffType);
        effectingBuff.Add(buffBase);
        switch (buffBase.buffType)
        {
            case BuffType.UpSpeed:
                moveSpeed += buffBase.effectValue;
                Debug.Log(gameObject.name);
                StartCoroutine(StartBuffUpSpeed(buffBase));
                break;
            case BuffType.AddMana:
                StartCoroutine(StartBuffAddMana(buffBase));
                break;
            default:
                break;
                
        }
    }
    
    public override IEnumerator StartBuff(BuffBase buffBase)
    {
        return base.StartBuff(buffBase);
    }

    private IEnumerator StartBuffUpSpeed(BuffBase buffBase)
    {
        yield return new WaitForSeconds(buffBase.durationTime);
        RemoveBuff(buffBase);
        moveSpeed -= buffBase.effectValue;
    }
    
    private IEnumerator StartBuffAddMana(BuffBase buffBase)
    {
        while (effectingBuff.Contains(buffBase))
        {
            yield return new WaitForSeconds(1);
            AddMana(buffBase.effectValue);
        }
    }


    #endregion

    public void FightAimingTarget(CharacterBase target, Team thisTeam)
    {
        if(team!=thisTeam||target==null)return;
        Vector3 dir = (target.transform.position - transform.position).normalized;
        rb.velocity = dir * moveSpeed;
        if(stateMachine.currentStateType==StateType.Fight)
            stateMachine.TransitionState(StateType.Fight,target.gameObject);
    }
    
    public virtual bool UseMana(int num)
    {
        if( num> playerStat.currentMana)return false;
        playerStat.DecrementMana(num);
        OnManaChange(playerStat.currentMana,team);
        return true;
    }
    public virtual void AddMana(int num)
    {
        playerStat.IncrementMana(num);
        OnManaChange(playerStat.currentMana,team);
    }
    protected virtual void OnManaChange(int num,Team team)
    {
        statChangeEventChannelPlayerMana.Broadcast(num,team);
    }

    /*public override void BeDamage(int num)
    {
        base.BeDamage(num);
        OnHealthChange(num,team);
    }*/

    public override void OnHealthChange(int num, Team team)
    {
        base.OnHealthChange(num, team);
        statChangeEventChannelPlayerHealth.Broadcast(num,team);
        if (num == 0)
        {
            GameMode.Instance.PlayerReborn(this);
        }
    }
    
}
