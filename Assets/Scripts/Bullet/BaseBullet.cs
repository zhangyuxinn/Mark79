using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBullet : MonoBehaviour
{
    [SerializeField]private Rigidbody rb;
    [SerializeField]private SphereCollider collider;
    [Header("子弹属性")]
    public float bulletSize=0.1f;
    public float shootSpeed=15f;
    public float damageRange=1;
    public int damage=1;
    [SerializeField] private float effectTime=0.2f;
    [SerializeField] private GameObject effectPrefab;

    public AudioClip shootAudioEffect;
    //public bool canIgnoreObstacle;做成派生类，当作不同的子弹。

    private void Start()
    {
        //collider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
    }

    public void Initialize(float size,float speed,Vector3 dir,float range,int damageNum,int teamLayer)
    {
        this.gameObject.transform.localScale = new Vector3(size, size, size);
        gameObject.layer = teamLayer;
        //速度方向
        shootSpeed = speed;
        rb.velocity = dir * speed;
        //伤害范围
        damageRange = range;
        collider.radius = range;
        //伤害数值
        damage = damageNum;
        StartCoroutine(LateDestroy());
        AudioManager.Instance.PlayMultipleSound(shootAudioEffect);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer!=gameObject.layer&& (other.CompareTag("Player")||other.CompareTag("Home")||other.CompareTag("Guard")) )
        {
            Debug.Log(other.gameObject.name);
            CharacterBase characterBase = other.gameObject.GetComponent<CharacterBase>();
            if (characterBase != null)
            {
                characterBase.BeDamage(damage);
                StartCoroutine(StartEffect());
                Debug.Log(damage);
            }
        }
    }
    public IEnumerator StartEffect()
    {
        GameObject effect= Instantiate(effectPrefab, gameObject.transform);
        Debug.Log(effect.name);
        rb.velocity=Vector3.zero;
        yield return new WaitForSeconds(effectTime);
        Destroy(this.gameObject);

    }



    IEnumerator LateDestroy()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("Destroy");
        Destroy(this);
    }
    
    
}
