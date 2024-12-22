using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectPool<T> : IObjectPool<T> , IDisposable where T : new()//对泛型T进行限制，要求T有无参构造函数
{
    private Queue<T> objectQue;
    //这个不懂是个啥
    private Func<T> objectFactory;

    [SerializeField]private int initialPoolSize=0;
    [SerializeField] private int maxPoolSize = 100;
    private int currentCount = 0;
    //这个也不懂
    private bool disposed;
    
    
    public ObjectPool(Func<T> theObjectFactory, int initialPoolSize = 0, int maxPoolSize = 100)
    {
        objectFactory = theObjectFactory;
        objectQue = new Queue<T>();
        this.maxPoolSize = maxPoolSize;
        this.initialPoolSize = initialPoolSize;
        currentCount = 0;
        for (int i = 0; i < this.initialPoolSize; i++)
        {
            var obj = CreateObject();
            if (obj != null)
            {
                objectQue.Enqueue(obj);    
            }
        }
    }
     private T CreateObject()
    {
        if(currentCount>=maxPoolSize) return default;

        var newObject = objectFactory != null ? objectFactory() : new T();
        currentCount++;
        EnqueueHandle(newObject);
        return newObject;
    }
     
    
    public T Get()
    {
        T obj = currentCount > 0 ? objectQue.Dequeue() : CreateObject();
        //这个不懂
        if (obj != null)
        {
            DequeueHandle(obj);
        }

        return obj;

    }

    public void Recycle(T item)
    {
        if (item == null)
        {
            return;
        }

        if (objectQue.Contains(item))
        {
            EnqueueHandle(item);
            objectQue.Enqueue(item);
        }
    }

    public void Cleanup(Func<T, bool> shouldCleanup)
    {
        for (int i = 0; i < currentCount; i++)
        {
            T obj = objectQue.Dequeue();
            if (shouldCleanup(obj))
            {
                currentCount--;
            }
            else
            {
                objectQue.Enqueue(obj);
            }
        }
    }

    public void EnqueueHandle(T item)
    {
        
    }

    public void DequeueHandle(T item)
    {
        
    }

    public void Dispose()
    {
    }
}
