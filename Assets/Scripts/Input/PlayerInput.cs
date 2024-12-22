using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Vector3EventChannel mouseDownEventChannel;
    [SerializeField] private FightTargetEventChannel fightTargetEventChannel;
    public ProcessChangeEventChannel processChangeEventChannel;
    [SerializeField] private FloatEventChannel scrollEventChannel;

    [SerializeField] LayerMask rayCastLayerMask;
    public float scrollValue;
    [Header("Visual")] 
    [SerializeField] private ParticleSystem vfxCanMovePrefab;
    [SerializeField] private ParticleSystem vfxCantMovePrefab;
    [SerializeField] private OutLineVisual outLineVisual;
    private void Update()
    {
        // 获取鼠标滚轮的垂直滚动值
        scrollValue = Input.GetAxis("Mouse ScrollWheel");

        if (scrollValue != 0)
        {
            scrollEventChannel.Broadcast(scrollValue);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                GetMousePoint();
            }

        }                                      
        
        //TODO:做一个中间层， 因为input的鼠标输入时不统一的，有拖拽有直接点击，需要一个类来把他转换成相同的输出，然后给playerController处理。
    }


    public void GetMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
       
        
        
        if (Physics.Raycast(ray, out hit,1000f,rayCastLayerMask))
        {
            if (outLineVisual.oldSpriteRenderer != null)
            {
                outLineVisual.ResetMaterial(outLineVisual.oldSpriteRenderer);
            }
            Debug.Log(hit.collider.gameObject.tag);
            Debug.Log(hit.collider.gameObject.layer);
            if (hit.transform.CompareTag("Ground"))
            {
                mouseDownEventChannel.Broadcast(hit.point);
                Instantiate(vfxCanMovePrefab, hit.point,quaternion.identity);
            }
            else if(hit.transform.CompareTag("Home"))
            {
                Instantiate(vfxCantMovePrefab, hit.point,quaternion.identity);
            }else if (hit.transform.CompareTag("Player")||hit.transform.CompareTag("Guard"))
            {
                CharacterBase characterBase = hit.collider.gameObject.GetComponent<CharacterBase>();
                fightTargetEventChannel.Broadcast(characterBase,Team.A);
                outLineVisual.OutLineShow(characterBase.visualSprite);
            }else if (hit.transform.CompareTag("BeginFight"))
            {
                StartCoroutine(GameMode.Instance.StartProcessChange());
            }else if (hit.transform.CompareTag("RestartButton"))
            {
                GameMode.Instance.ActiveEndButton();
            }else if (hit.transform.CompareTag("BeginNotice"))
            {
                GameMode.Instance.processOneText.SetActive(true);
                hit.transform.gameObject.SetActive(false);
            }
        }
    }

    

}