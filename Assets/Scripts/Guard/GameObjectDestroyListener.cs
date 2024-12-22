using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDestroyListener : MonoBehaviour
{
    public InFightRange inFightRange;

    private void Update()
    {
        if (inFightRange==null)
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        if (inFightRange!= null)
        {
            inFightRange.RemoveGameObjectFromList(gameObject);
        }
    }
}
