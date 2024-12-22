using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoParameterEventChannel<T1,T2> : ScriptableObject
{
    private event Action<T1,T2> Delegate;

    public void Broadcast(T1 param1, T2 param2)
    {
        Delegate?.Invoke(param1, param2);
    }

    public void AddListener(Action<T1,T2> action)
    {
        Delegate += action;
    }

    public void RemoveListener(Action<T1,T2> action)
    {
        Delegate += action;
    }
    

}
