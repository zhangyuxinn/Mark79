using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "Data/Buff",fileName = "Buff_")]
public class BuffBase : ScriptableObject
{
    public BuffType buffType;
    public BuffOverlap buffOverlap;
    public BuffCloseType buffCloseType;
    public BuffCalculateType buffCalculateType;
    public BuffRange buffRange;
    public EnableTime enableTime;
    
    public int effectValue;//效果值
    public int maxLimit;//最大次数
    public int level;//层数
    public float intervalTime;//间隔时间
    public float durationTime;//持续时间
    [HideInInspector] public float timer;//计时器


}
public enum BuffType
{
    None=0,
    UpSpeed=1,
    AddMana=2,
}

public enum EnableTime
{
    Duration,
    Once,
    Permanent
}
public enum BuffOverlap //叠加类型
{
    None,
    StackedTime,//增加时间
    StackedLayer,//增加层数
    ResetTime,//重置时间
}
public enum BuffCloseType
{
    All,//全部关闭
    Layer,//逐层关闭
}
public enum BuffCalculateType//执行类型 
{
    Once,//一次
    Loop,//每次
}
public enum BuffRange//生效范围 
{
    All,//全体
    Self,//自己
}