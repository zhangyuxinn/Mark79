using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardStat : BaseStat
{
    public GuardType guardType;
    public int cost;
    public bool isFront;
    public GuardStatSO guardStatSo=>initStat as GuardStatSO;
    protected override void Initialize()
    {
        base.Initialize();
        cost = guardStatSo.callCost;
        isFront = guardStatSo.isfront;
    }
}
