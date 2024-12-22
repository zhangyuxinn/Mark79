using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateMachine),typeof(BaseStat),typeof(Rigidbody))]
public abstract class CharacterBase : MonoBehaviour,IBuffable,IHasStat
{
    
    [Header("基础组件")]
    public Rigidbody rb;
    protected StateMachine stateMachine;
    protected BaseStat baseStat;
    public Transform healthRateTransform;
    [SerializeField]private SpriteRenderer healthSprite;
    public SpriteRenderer visualSprite;
    public Color visualSpriteColor;
    

    [Header("Buff")] 
    [SerializeField] public BuffEventChannel buffEventChannel;
    [SerializeField] protected HomeBoomEventChannel homeBoomEventChannel;
    public List<BuffBase> effectingBuff { get; set; }

    [Header("Fight")] 
    protected InFightRange inFightRange;
    protected List<GameObject> fightNeutral => inFightRange.inFightRangeNeutral;
    protected List<GameObject> fightEnemies => inFightRange.inFightRangeEnemies;
    
    [SerializeField] private float effectTime=0.2f;
    [SerializeField] private GameObject effectPrefab;

    [SerializeField] private AudioClip dieAudio;
    public Team team;
    
    protected virtual void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        rb = GetComponent<Rigidbody>();
        effectingBuff = new List<BuffBase>();
        inFightRange = GetComponentInChildren<InFightRange>();
        baseStat = GetComponent<BaseStat>();
        Init();

    }

    public virtual void Init()
    {
        if (gameObject.layer == 10)
        {
            team = Team.A;
            healthSprite.color = Color.green;
            visualSprite.color = Color.cyan;
        }
        if (gameObject.layer == 11)
        {
            team = Team.B;
            healthSprite.color = Color.red;
            if (gameObject.name == "DreamEnemy(Clone)")
            {
                visualSprite.color = Color.gray;
            }
        }
        if (gameObject.layer == 12)
        {
            team = Team.C;
        }

        HealthInit();
    }

    protected virtual void Update()
    {
        UpdateTrigger();
        visualSprite.flipX=rb.velocity.x<0;
    }

    protected virtual void OnEnable()
    {
        buffEventChannel.AddListener(OnBuff);
    }

    protected virtual void OnDisable()
    {
        buffEventChannel.RemoveListener(OnBuff);

    }

    public virtual void OnBuff(BuffBase buffBase)
    {
    }

    public virtual void RemoveBuff(BuffBase buffBase)
    {
        effectingBuff.Remove(buffBase);
    }

    public virtual IEnumerator StartBuff(BuffBase buffBase)
    {
        yield return 1;
    }

    #region Stat

    public void HealthInit()
    {
        SetHealth(baseStat.maxHealth);
    }
    public virtual void BeDamage(int num)
    {
        if (team == Team.A)
        {
            num += 1;
            num /=  2;
        }
        baseStat.DecrementHealth(num);
        OnHealthChange(baseStat.currentHealth,team);
    }

    public virtual void SetHealth(int num)
    {
        baseStat.DecrementHealth(baseStat.maxHealth);
        baseStat.IncrementHealth(num);
        OnHealthChange(baseStat.currentHealth,team);
    }

    public virtual void OnHealthChange(int num,Team team)
    {
        SetHralthBar(num);
    }

    public virtual void SetHralthBar(int num)
    {
        healthRateTransform.localScale =new Vector3((float)num/(float)baseStat.maxHealth*1.2f 
            ,healthRateTransform.localScale.y,healthRateTransform.localScale.z);
        
    }
    #endregion
    
    protected virtual void UpdateTrigger()
    {
        if (GameMode.Instance.gameProcess == GameProcess.One || GameMode.Instance.gameProcess == GameProcess.Four)
        {
            if(stateMachine.currentStateType==StateType.Fight)
            {
                stateMachine.TransitionState(StateType.Idle);
            }
            else
            {
                return;
            }
        }
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
            }
            stateMachine.TransitionState(StateType.Fight,fightTarget);
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
                }
                stateMachine.TransitionState(StateType.Fight,fightTarget);
            }
            else
            {
                if(stateMachine.currentStateType!=StateType.Idle)
                    stateMachine.TransitionState(StateType.Idle);
            }
        }
    }

    public virtual void Die()
    {
        AudioManager.Instance.PlayMultipleSound(dieAudio);
    }

    protected virtual void OnTriggerEnter (Collider other)
    {
        /*if (other.gameObject.CompareTag("Bullet") && other.gameObject.layer!=gameObject.layer )
        {
            BaseBullet bullet = other.gameObject.GetComponent<BaseBullet>(); 
            BeDamage(bullet.damage);
            Destroy(other.gameObject);
            Debug.Log(gameObject.name);
        }*/
    }

    protected virtual void OnHomeBoom(Team boomTeam,HomeType homeType)
    {
        
    }
}
