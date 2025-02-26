using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroupLeader : MonoBehaviour
{
    public List<GuardBase> guards = new List<GuardBase>();
    private Vector3 averagePosition;

    [Header("士兵召唤部分")] 
    [SerializeField] private float guardRange;
    [SerializeField] private float guardRangeStart;
    [SerializeField] private float guardRangeAdd;
    [SerializeField] private AudioClip callGuardAudioEffect;
    
    private PlayerController playerController;
    [SerializeField]private CallGuardEventChannel callGuard3EventChannel;

    [SerializeField] private AllGuardPrefab allGuardPrefab; //所有士兵预制体的集合（从中实例化士兵）

    [SerializeField] private float forceFront;

    private void Start()
    {
        
        averagePosition = transform.position;
        playerController = gameObject.GetComponent<PlayerController>();
        guardRange = guardRangeStart;
    }

    private void OnEnable()
    {
        callGuard3EventChannel.AddListener(OnCallGuard);
    }

    public void Update()
    {

        UpdateAverage();
        FLock();//调整编队
    }
    private bool InSight(GameObject other)//计算士兵是否在GroupLeader的视线内
    {
        return Vector3.Distance(transform.position, other.transform.position) < guardRange;
    }
    private void FLock()
    {
        guardRange = guardRangeStart + guards.Count * guardRangeAdd;
        //要对群落中每个GB进行作用
        //对于加入队列（在群落里的，不在视野中就施加指向平均中心，然后和其他的GB距离太近就收到反向力
        for (int i=0;i<guards.Count;i++)
        {
            if (guards[i] == null)
            {
                guards.Remove(guards[i]);
                i--;
                continue;
            }
            if (!InSight(guards[i].gameObject))
            {
                guards[i].Seek(averagePosition);
            }
            float tmpNum= Vector3.Dot(playerController.rb.velocity.normalized,
                (guards[0].gameObject.transform.position - playerController.transform.position).normalized);
            if (guards[i].isFront)
            {

                guards[i].rb.AddForce(
                    playerController.rb.velocity.normalized*(1-tmpNum)*forceFront);
                
            }
            else
            {
                if (tmpNum > 0)
                {
                    guards[i].rb.AddForce(
                        -playerController.rb.velocity.normalized*(tmpNum+1)*forceFront);
                }
            }
            
        }
    }
    void UpdateAverage()//更新groupLeader的位置
    {
        averagePosition=transform.position;

    }

    private void OnCallGuard(int guardIndex, Team tmpTeam)
    {
        if(tmpTeam!=playerController.team)return;//判断角色归属队伍与当前队伍是否相同
        GuardBase tmpGuardBase= allGuardPrefab.guardPrefab[guardIndex].GetComponent<GuardBase>();
        //获取预制件中对应士兵的预制件
        Debug.Log(tmpGuardBase.name);
        if (!playerController.UseMana(tmpGuardBase.GetCost()))
        {
            Debug.Log("不够蓝");
            return;
        }
        AudioManager.Instance.PlaySingleSound(callGuardAudioEffect);
        GameObject instantiateGuard = Instantiate(allGuardPrefab.guardPrefab[guardIndex], transform);
        //用于克隆士兵预制件，并使其生成在当前的GroupLeader下
        instantiateGuard.layer = gameObject.layer;
        ChangeLayer(instantiateGuard.transform, gameObject.layer);
        tmpGuardBase.groupLeader = this;
        guards.Add(instantiateGuard.GetComponent<GuardBase>());//将士兵加入当前士兵列表中
    }

    private void ChangeLayer(Transform transform, int layerIndex)
    {
        transform.gameObject.layer = layerIndex;

    }
    
}
