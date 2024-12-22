using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHome : HomeBase
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(StartAddMana());
    }

    IEnumerator StartAddMana()
    {
        yield return new WaitForSeconds(0.5f);
        BuffDataFactory.Instance.BroadcastBuffEvent(BuffType.AddMana,team);
        
    }
        public override void SetHralthBar(int num)
        {
            healthRateTransform.localScale =new Vector3((float)num/(float)baseStat.maxHealth*0.6f 
                ,healthRateTransform.localScale.y,healthRateTransform.localScale.z);
        }
}
