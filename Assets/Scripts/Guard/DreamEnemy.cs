using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DreamEnemy : GuardBase
{
    private Transform targetTransform;

    protected override void Start()
    {
        base.Start();

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        targetTransform = FindObjectOfType<PlayerStateMachine>().gameObject.transform;
        Debug.Log("1111"+FindObjectOfType<PlayerStateMachine>().gameObject);
    }

    protected override void Update()
    {
        Seek(targetTransform.position);
    }

    protected override void OnDestroy()
    {
    }
}
