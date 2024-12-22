using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeParameterEventChannel<T1,T2,T3> : ScriptableObject
{
    private event Action<T1,T2,T3> Delegate;

    public void Broadcast(T1 param1, T2 param2, T3 param3)
    {
        Delegate?.Invoke(param1, param2, param3);
    }

    public void AddListener(Action<T1,T2,T3> action)
    {
        Delegate += action;
    }

    public void RemoveListener(Action<T1,T2,T3> action)
    {
        Delegate += action;
    }
    

}
