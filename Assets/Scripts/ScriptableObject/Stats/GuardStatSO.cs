using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/Stats/GuardStatSO")]
public class GuardStatSO : StatSO
{
    public GuardType guardType;
    public int callCost;
    public bool isfront;
}
public enum GuardType {
    NormalGuard=1,
    SniperGuard=2,
    BazookaGuard=3,
    StuttererGuard=4,
    GunshipGuard=5,
    TankGuard=6,
    MainHome=7,
    OutsideHome=8,
    
    
}