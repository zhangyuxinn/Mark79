using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class PlayerStat : BaseStat
{
    public int maxMana;
    public int currentMana=0;
    public PlayerStatSO playerStatSo;
    protected override void Awake()
    {
        playerStatSo = initStat as PlayerStatSO;
        base.Awake();
    }

    protected override void Initialize()
    {
        base.Initialize();
        maxMana = playerStatSo.maxMana;

    }

    public void DecrementMana(int num)
    {
        currentMana -= num;
        
        
        currentMana = math.clamp(currentMana, 0, maxMana);
    }
    
    public void IncrementMana(int num)
    {
        currentMana += num;
        currentMana = math.clamp(currentMana, 0, maxMana);
    }
    
}
