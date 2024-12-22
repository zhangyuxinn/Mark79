using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDataFactory : MonoBehaviour
{
    public static BuffDataFactory Instance;

    public BuffBase buffBaseTest;
    public BuffBase buffBaseTest2;
    public Dictionary<BuffType,BuffBase> buffs;
    public BuffEventChannel buffEventChannel_A;
    public BuffEventChannel buffEventChannel_B;
    
    //TODO:读数据、改框架，好好学习一下别人的咋写的，在浏览器收藏夹

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        buffs = new Dictionary<BuffType, BuffBase>();
        buffs.Add(BuffType.UpSpeed,buffBaseTest);
        buffs.Add(BuffType.AddMana,buffBaseTest2);
        Debug.Log(buffs[BuffType.AddMana]);


    }

    public BuffBase CreateBuff(BuffType buffType)// 创建对应算法
    {
        return buffs[buffType];
        
        
        
        
        
        
        
        //
        // string className = string.Format("Platform.Buff.Buffs.{0}Buff", buffType.ToString());
        // if (!typeCache.TryGetValue(className, out Type type))
        // {
        //     // 如果缓存中没有该类型，反射获取并缓存
        //     type = Type.GetType(className);
        //     if (type != null)
        //     {
        //         typeCache[className] = type;
        //     }
        //     else
        //     {
        //         Debug.LogError($"Type {className} not found.");
        //         return null;
        //     }
        // }
        //
        // //给buff填充数据
        // var BuffData = buffData.GetBuff(buffType);
        //
        // BuffBase buff = Activator.CreateInstance(type) as BuffBase;
        //
        // if (buff != null) CopyProperties(BuffData, buff);
        //
        // return buff;
    }

    public void BroadcastBuffEvent(BuffType buffType,Team team)
    {
        if (team == Team.A)
        {

            buffEventChannel_A.Broadcast(buffs[buffType]);
        }
    }
    
}

public enum Team
{
    A,
    B,
    C,
}
