using System;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPool<T>
{
    T Get();
    void Recycle(T item);
    void Cleanup(Func<T, bool> shouldCleanup);
    void EnqueueHandle(T item);
    void DequeueHandle(T item);
}
