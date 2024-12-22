using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneParameterEventChannel<T> : ScriptableObject
{
    private event Action<T> Delegate;

    public void Broadcast(T obj)
    {
        Delegate?.Invoke(obj);
    }

    public void AddListener(Action<T> action)
    {
        Delegate += action;
    }

    public void RemoveListener(Action<T> action)
    {
        Delegate += action;
    }
    

}
