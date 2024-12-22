using System;
using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AIPlayerController : PlayerController
{

    private PlayerController playerController;
    private AIPlayerStateMachine aiPlayerStateMachine;
    [SerializeField]private GameObject mainHome;
    [SerializeField] private GroupLeader groupLeader;
    private Vector2 dirToPlayer=>
        new Vector2((playerController.transform.position-gameObject.transform.position).x,
            (playerController.transform.position-gameObject.transform.position).z);  
    private Vector2 dirToHome=>
        new Vector2((mainHome.transform.position-gameObject.transform.position).x,
            (mainHome.transform.position-gameObject.transform.position).z);
    public float protectHomeRate =0.5f ;
    [Header("距离控制")]
    public float keepDisForceRate =0.5f ;

    public float keepDisHome = 100;
    public float keepDisPlayer = 5;
    private Vector3 dirVect;
    float timeCount = 0;
    bool isRight = true;

    [Header("随机性")] 
    public float randomWaitTime;
    private Vector2 randomDir;
    public float randomRunRate =0.5f ; 
    [Header("稳定性")]
    [SerializeField]private float slowOffset=0.2f;
    [SerializeField]private float maxSpeed=0.2f;
    [Header("state改变")] 
    [SerializeField] private int manaCost = 0;
    [Range(0,1)][SerializeField] private float protectHomeHealthRate = 0.5f;
    [Header("召唤士兵")] 
    [SerializeField] private CallGuardEventChannel callGuardEventChannel;
    
    protected override void Start()
    {
        base.Start();
        playerController=GameObject.Find("Player1").GetComponent<PlayerController>();
        StartCoroutine(StartRandomRun());
        aiPlayerStateMachine= stateMachine as AIPlayerStateMachine;
        playerStat.currentMana = 10;

    }

    protected override void OnEnable()
    {
        buffEventChannel.AddListener(OnBuff); 
        homeBoomEventChannel.AddListener(OnHomeBoom);
    }

    protected override void OnDisable()
    {
        buffEventChannel.RemoveListener(OnBuff);
        homeBoomEventChannel.RemoveListener(OnHomeBoom);
    }
    protected override void Update()
    {
        UpdateTrigger();
        RandomRun();
        SlowDown();
        rb.velocity = rb.velocity.normalized * moveSpeed;
        KeepDistance(keepDisPlayer);
        if (aiPlayerStateMachine.currentStateType == StateType.Wander)
        {
            if ((float)mainHome.GetComponent<GuardStat>().currentHealth / mainHome.GetComponent<GuardStat>().maxHealth <
                protectHomeHealthRate)
            {
                aiPlayerStateMachine.TransitionState(StateType.Protect);
            }
            if (groupLeader.guards.Count >= 4)
            {
                Debug.Log(groupLeader.guards.Count);
                aiPlayerStateMachine.TransitionState(StateType.AIFight);
            }
        }
        if (groupLeader.guards.Count < 4 && aiPlayerStateMachine.currentStateType == StateType.AIFight)
        {
            Debug.Log(groupLeader.guards.Count);
            aiPlayerStateMachine.TransitionState(StateType.Wander);
        }
        if ((float)mainHome.GetComponent<GuardStat>().currentHealth/mainHome.GetComponent<GuardStat>().maxHealth
            < protectHomeHealthRate 
            && aiPlayerStateMachine.currentStateType == StateType.AIFight)
        {
            aiPlayerStateMachine.TransitionState(StateType.Wander);
        }
    }

    #region AIControl

    private void KeepDistance(float distance)
    {
        if (dirToPlayer.magnitude > distance)
        {
            rb.AddForce(new Vector3(dirToPlayer.x,0,dirToPlayer.y) *keepDisForceRate);
        }
        else
        {
            rb.AddForce(-new Vector3(dirToPlayer.x,0,dirToPlayer.y) *keepDisForceRate);
        }
    }
    
    protected override void UpdateTrigger()
    {

        if (!aiPlayerStateMachine.isFight)
        {
            if (fightEnemies.Count != 0)
            {
                GameObject fightTarget=fightEnemies[0];
                float minMagnitude=(gameObject.transform.position - fightEnemies[0].transform.position).magnitude;
            
                foreach (var fightEnemy in fightEnemies)
                {
                    if (minMagnitude > (gameObject.transform.position - fightEnemy.transform.position).magnitude)
                    {
                        fightTarget = fightEnemy;
                    }
                }
                //todo 临时用着。。
                if (fightTarget.layer == gameObject.layer)
                {
                    inFightRange.inFightRangeEnemies.Remove(fightTarget);
                    return;
                }
                aiPlayerStateMachine.StartFight(StateType.Fight,fightTarget);
            }
            else
            {
                if (fightNeutral.Count != 0)
                {
                    GameObject fightTarget=fightNeutral[0];
                    float minMagnitude=(gameObject.transform.position - fightNeutral[0].transform.position).magnitude;
                    foreach (var fightEnemy in fightNeutral)
                    {
                        if (minMagnitude > (gameObject.transform.position - fightEnemy.transform.position).magnitude)
                        {
                            fightTarget = fightEnemy;
                        }
                    }
                    //todo 临时用着。。
                    if (fightTarget.layer == gameObject.layer)
                    {
                        inFightRange.inFightRangeNeutral.Remove(fightTarget);
                        return;
                    }
                    aiPlayerStateMachine.StartFight(StateType.Fight,fightTarget);
                }

            }
        }
        else
        {
            if (fightEnemies.Count == 0&&fightNeutral.Count == 0)
                aiPlayerStateMachine.isFight = false;
        }


        
    }
    
    public void SlowDown()
    {
        Vector3 dir = -rb.velocity;
        rb.AddForce(dir*slowOffset);
        if (rb.velocity.magnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
    IEnumerator StartRandomRun()
    {
        while (true)
        {
            randomDir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            rb.AddForce(new Vector3( randomDir.x,0,randomDir.y)*randomRunRate,ForceMode.Impulse);
            yield return new WaitForSeconds(randomWaitTime);
        }
    }
    
    private void RandomRun()
    {
        
        
    }

    public void StartWander()
    {
        //与敌方主角保持距离，
        keepDisPlayer = 7;
        keepDisHome = 10;
    }

    public void UpdateWander()
    {
        RangePlayer();
    }

    private void RangePlayer()
    {
        if (dirToHome.magnitude > keepDisHome&& timeCount>1)
        {
            if(isRight)
            {
                isRight = false;
            }
            else
            {
                isRight = true;
            }
            timeCount = 0;
        }
        timeCount += Time.deltaTime;
        if (isRight)
        {
            dirVect = new Vector3(dirToPlayer.y, 0, -dirToPlayer.x);
        }
        else
        {
            dirVect = new Vector3(-dirToPlayer.y, 0, dirToPlayer.x);
        }
        
        rb.AddForce(dirVect);
    }
    public void EndWander()
    {
        //与敌方主角保持距离，

    }

    public void StartProtect()
    {
        keepDisHome = 5;
    }
    
    public void ProtectHome()
    {
        RangePlayer();
        
        Vector3 dir = mainHome.transform.position - transform.position;
        rb.AddForce(dir*protectHomeRate);
    }

    public void StartFight()
    {
        keepDisPlayer = 5f;
    }

    #endregion

    public override bool UseMana(int num)
    {
        manaCost += num;

        return base.UseMana(num);
    }

    protected override void OnManaChange(int num, Team team)
    {
        base.OnManaChange(num, team);
        playerStat.currentMana = 10;

    }

    protected override void OnHomeBoom(Team boomTeam, HomeType homeType)
    {
        base.OnHomeBoom(boomTeam, homeType);
        gameObject.SetActive(false);
    }
}
