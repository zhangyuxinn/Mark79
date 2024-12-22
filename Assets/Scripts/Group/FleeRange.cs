using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeRange : MonoBehaviour
{
    private GuardBase guardBase;

    private void Start()
    {
        guardBase = GetComponentInParent<GuardBase>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(!other.gameObject.CompareTag("Guard"))return;
        if(other.name=="CollisionFight")return;
        GuardBase otherGuard = other.GetComponent<GuardBase>();
        GroupLeader otherGroupLeader = otherGuard.groupLeader;
        if (otherGroupLeader)
        {
            if (guardBase.groupLeader == otherGroupLeader)
            {
                guardBase.Flee(other.transform.position);
            }
        }
    }
}
