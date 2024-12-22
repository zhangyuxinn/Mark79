using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasStat
{
    public void BeDamage(int num);


    public void OnHealthChange(int num,Team team);
}
