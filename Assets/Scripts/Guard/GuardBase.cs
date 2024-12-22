using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GuardBase : CharacterBase,IInGroup
{
    
    [Header("角色属性")]
    [SerializeField]private GuardStat guardStat;
    public float speed = 2;
    public float maxSpeed = 2f;


    [Header("群相关")] 
    public GroupLeader groupLeader;
    [SerializeField]private float seekOffset;
    [SerializeField]private float fleeOffset;
    [SerializeField]private float slowOffset=0.2f;
    public bool isFront;

    [Header("事件")]
    [SerializeField] private GuardChangeEventChannel guardChangeEventChannel_die;
    
    protected override void Start()
    {
        base.Start();
        guardStat = baseStat as GuardStat;
        isFront = guardStat.isFront;
    }



    protected override void OnDisable()
    {
        base.OnDisable();
    }
    
    protected override void Update()
    {
        base.Update();
        SlowDown();
    }

    public override void OnHealthChange(int num, Team team)
    {
        base.OnHealthChange(num, team);
        if (num == 0)
        {
            Die();

        }
    }

    #region Flock

    public void Seek(Vector3 target)
    {
        Vector3 dir = (target- gameObject.transform.position);
        Vector3 steer = dir * speed - rb.velocity;
        rb.AddForce(dir*seekOffset);
    }

    public void Flee(Vector3 target)
    {
        Vector3 dir = (target- gameObject.transform.position);
        int k = 1;
        if (dir.x < 1 || dir.z < 1)
        {
            if (dir.x != 0)
            {
                dir.x = 1 / dir.x;
                
            }
            if (dir.z != 0)
                dir.z = 1 / dir.z;
        }
        Vector3 steer = dir * speed - rb.velocity;
        rb.AddForce(-steer*fleeOffset); 
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
    #endregion
    public override void Die()
    {
        base.Die();
        guardChangeEventChannel_die.Broadcast(team);
        Destroy(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
        groupLeader.guards.Remove(this);
        BuffDataFactory.Instance.BroadcastBuffEvent(BuffType.UpSpeed,guardStat.team);
    }


    

    public void OnBuff(BuffBase buffBase)
    {
        
    }

    public void RemoveBuff(BuffBase buffBase)
    {
        
    }

    public IEnumerator StartBuff(BuffBase buffBase)
    {
        yield return 1f ;
    }

    public int GetCost()
    {
        Debug.Log(guardStat);
        return guardStat.guardStatSo.callCost;
    }
}
