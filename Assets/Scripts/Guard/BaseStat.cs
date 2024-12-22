using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseStat : MonoBehaviour
{
    public StatSO initStat;
    public Team team;
    public int maxHealth;
    public int currentHealth=0;

    
    
    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        maxHealth = initStat.health;


    }
    

    private void OnEnable()
    {
    }

    public void DecrementHealth(int num)
    {
        currentHealth -= num;
        currentHealth = math.clamp(currentHealth, 0, maxHealth);
    }
    
    public void IncrementHealth(int num)
    {
        currentHealth += num;
        currentHealth = math.clamp(currentHealth, 0, maxHealth);
    }
}
