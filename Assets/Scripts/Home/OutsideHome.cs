using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideHome : HomeBase
{
    [Header("事件参数")] 
    public OutsideHomeOccupiedEventChannel outsideHomeOccupiedEventChannel;
    private void OutsideHomeOccupied()
    {
        team = attackTeam;
        if (team == Team.A)
        {
            gameObject.layer = 10;
            transform.GetChild(0).gameObject.layer = 10;
        }
        else
        {
            gameObject.layer = 11;
            transform.GetChild(0).gameObject.layer = 11;
        }
        outsideHomeOccupiedEventChannel.Broadcast(team);
        Debug.Log("Occupy");
        SetHealth(10000);
        BuffDataFactory.Instance.BroadcastBuffEvent(BuffType.AddMana,team);
        
    }

    public override void OnHealthChange(int num,Team team)
    {
        base.OnHealthChange(num,team);
        if (num == 0 && team == Team.C)
        {
            OutsideHomeOccupied();
        }
    }
}
