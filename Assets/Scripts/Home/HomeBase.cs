using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : CharacterBase
{
    public HomeType homeType;
    
    private GuardStat guardStat;


    public Team attackTeam;

    protected override void Start()
    {
        base.Start();
        guardStat=baseStat as GuardStat;
    }

    private void Initialize()
    {
        
    }
    
    private void HomeBoom(Team teamBoom,HomeType boomHomeType)
    {
        homeBoomEventChannel.Broadcast(teamBoom,boomHomeType);
        Debug.Log("Booom");
        gameObject.SetActive(false);
    }
    
    #region Stats



    public override void OnHealthChange(int num,Team team)
    {
        base.OnHealthChange(num,this.team);
        if (num == 0&& team!=Team.C)
        {
            HomeBoom(team,homeType);
        }
    }



    #endregion

    protected override void UpdateTrigger()
    {
        base.UpdateTrigger();

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.transform.CompareTag("Bullet")&&other.gameObject.layer!=gameObject.layer )
        {
            if (other.gameObject.layer == 10)
            {
                attackTeam = Team.A;
            }else if (other.gameObject.layer == 11)
            {
                attackTeam = Team.B;
            }
            //BaseBullet bullet = other.gameObject.GetComponent<BaseBullet>(); 
            //BeDamage(bullet.damage);
        }
    }

    #region buff
    

    public void BuffPlayer()
    {
        BuffDataFactory.Instance.BroadcastBuffEvent(BuffType.AddMana,team);
    }

    #endregion
    
}

public enum HomeType
{
    Main,
    OutSide
}
