using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/State/Fight",fileName = "FightState_")]
public class FightState :CharacterState,IBuffable
{
    public GameObject fightTarget;
    public BuffEventChannel buffEventChannel;
    public List<BuffBase> effectingBuff { get; set; }

    public GameObject bullet;
    [Header("射出的子弹属性")] 
    public float shootInterval;
    public float bulletSize=0.1f;
    public float shootSpeed=20f;
    public float damageRange=1;
    public int damage=1;
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        
    }
    
    public void OnEnd()
    {
        
    }
    
    public virtual FightState CopyNewState()
    {
        FightState returnFightState = ScriptableObject.CreateInstance<FightState>();
        returnFightState.fightTarget=fightTarget;
        returnFightState.buffEventChannel = buffEventChannel;
        returnFightState.effectingBuff = effectingBuff;

        returnFightState.bullet=bullet;
        returnFightState.shootInterval = shootInterval;
        returnFightState.bulletSize=bulletSize;
        returnFightState.shootSpeed=shootSpeed;
        returnFightState. damageRange= damageRange;
        returnFightState.damage= damage;
        return returnFightState;
    }

    private void ShootBullet()
    {
        Vector3 dir = (fightTarget.transform.position-stateMachine.character.transform.position).normalized;
        dir.y = 0;
        GameObject currentBullet= Instantiate(bullet, 
            new Vector3(stateMachine.character.transform.position.x,
                stateMachine.character.transform.position.y+0.5f,
                stateMachine.character.transform.position.z),
            quaternion.identity);

        Bullet bulletComponent = currentBullet.GetComponent<Bullet>();
        bulletComponent.Initialize(bulletSize,shootSpeed,dir,damageRange,damage,stateMachine.character.layer);
    }

    public IEnumerator StartShootBullet()
    {
        //如果你想要比较两个对象的值，你需要确保比较的是对象的实际内容，而不是它们的引用，
        //所以要将对象显式转换成该对象，然后去看能不能转换成功，以此来比较
        while (stateMachine.currentState is FightState)
        {
            if (fightTarget == null|| fightTarget.layer==stateMachine.character.layer)
            {
                stateMachine.TransitionState(StateType.Idle);
                break;
            }
            ShootBullet();
            yield return new WaitForSeconds(shootInterval);
        }
    }
    public IEnumerator StartAIShootBullet()
    {
        //如果你想要比较两个对象的值，你需要确保比较的是对象的实际内容，而不是它们的引用，
        //所以要将对象显式转换成该对象，然后去看能不能转换成功，以此来比较
        AIPlayerStateMachine aiPlayerStateMachine = stateMachine as AIPlayerStateMachine;
        while (aiPlayerStateMachine.isFight)
        {
            Debug.Log(fightTarget);
            if (fightTarget == null|| fightTarget.layer!=stateMachine.character.layer)
            {
                stateMachine.TransitionState(StateType.Idle);
            }
            ShootBullet();
            yield return new WaitForSeconds(shootInterval);
        }
    }


    public void OnBuff(BuffBase buffBase)
    {
        
    }

    public void RemoveBuff(BuffBase buffBase)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator StartBuff(BuffBase buffBase)
    {
        throw new System.NotImplementedException();
    }
}
