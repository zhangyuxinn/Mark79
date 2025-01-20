
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private Canvas canvas;
    private Image imageComponent;
    [SerializeField] private bool 是否实例化视觉处理器 = true;
    private VisualCardsHandler visualHandler;
    private Vector3 offset;

    [Header("Movement")]
    [SerializeField] private float 卡片移动的速度限制 = 50;

    [Header("Selection")]
    public bool 被选择;
    public float selectionOffset = 50;
    private float pointerDownTime;
    private float pointerUpTime;

    [Header("Visual")]
    [SerializeField] private GameObject 卡片视觉预制件;
    [HideInInspector] public CardVisual 卡片视觉处理器;
    //这些字段用于卡片的视觉表现，包括预制件和视觉处理器的引用
    [Header("States")]
    public bool 是否悬停;
    public bool 是否正在拖动;
    [HideInInspector] public bool 是否被拖动过;
    //这些字段用于跟踪卡片的状态，如是否悬停、是否正在拖动以及是否被拖动过
    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;
    //这些是UnityEvent字段，用于在Unity编辑器中方便地分配事件处理函数
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();

        if (!是否实例化视觉处理器)
            return;

        visualHandler = FindObjectOfType<VisualCardsHandler>();
        卡片视觉处理器 = Instantiate(卡片视觉预制件, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<CardVisual>();
        卡片视觉处理器.Initialize(this);
    }
    //Start方法在脚本实例化时调用，用于初始化Canvas和Image组件的引用，以及实例化视觉处理器
    //也就是说，点击开始运行之后，生成一堆的Canvas和Image组件，同时每个card槽位上装载一个视觉化的卡面
    void Update()
    {
        ClampPosition();

        if (是否正在拖动)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 velocity = direction * Mathf.Min(卡片移动的速度限制, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }
    }
    //限制卡片的位置并处理拖动逻辑（每帧）
    void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }
    //// 限制卡片位置在屏幕范围内
    public void OnBeginDrag(PointerEventData eventData)
    {// 处理开始拖动事件
        BeginDragEvent.Invoke(this);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = mousePosition - (Vector2)transform.position;
        是否正在拖动 = true;
        canvas.GetComponent<GraphicRaycaster>().enabled = false;
        imageComponent.raycastTarget = false;

        是否被拖动过 = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {// 处理结束拖动事件
        EndDragEvent.Invoke(this);
        是否正在拖动 = false;
        canvas.GetComponent<GraphicRaycaster>().enabled = true;
        imageComponent.raycastTarget = true;

        StartCoroutine(FrameWait());

        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
            是否被拖动过 = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {// 处理指针进入事件
        PointerEnterEvent.Invoke(this);
        是否悬停 = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {// 处理指针离开事件
        PointerExitEvent.Invoke(this);
        是否悬停 = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {// 处理指针按下事件
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PointerDownEvent.Invoke(this);
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {// 处理指针释放事件
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pointerUpTime = Time.time;

        PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

        if (pointerUpTime - pointerDownTime > .2f)
            return;

        if (是否被拖动过)
            return;

        被选择 = !被选择;
        SelectEvent.Invoke(this, 被选择);

        if (被选择)
            transform.localPosition += (卡片视觉处理器.transform.up * selectionOffset);
        else
            transform.localPosition = Vector3.zero;
    }
    //这些方法实现了接口中定义的事件处理逻辑，用于处理用户的拖动和指针交互
    public void Deselect()
    {// 取消选择卡片
        if (被选择)
        {
            被选择 = false;
            if (被选择)
                transform.localPosition += (卡片视觉处理器.transform.up * 50);
            else
                transform.localPosition = Vector3.zero;
        }
    }


    public int SiblingAmount()
    {// 获取同级元素数量
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {// 获取父元素的索引
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }

    public float NormalizedPosition()
    {// 获取标准化位置
        return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
    }

    private void OnDestroy()
    {// 在对象销毁时调用，用于清理
        if (卡片视觉处理器 != null)
            Destroy(卡片视觉处理器.gameObject);
    }
}

