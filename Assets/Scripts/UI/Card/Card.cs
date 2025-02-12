using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler

{
    private Canvas Pcanvas;
    private Image image_component;//卡牌底框
    private Image characterImage;//角色图片
   // private VisualCardsHandler visualHandler;
    private Vector3 offset;

    [SerializeField] private float moveSpeedLimit = 5000;//卡牌移动速度上限

    public bool selected;
    public float selectionOffset = 50;//被选中时的偏移量
    private float pointerDownTime;
    private float pointerUpTime;


    [SerializeField] private GameObject cardVisualPrefab;//卡片的视觉效果预制体
    [HideInInspector] public CardVisual cardVisual;//当前卡片的 CardVisual 组件实例

    public bool isHovering;
    public bool isDragging;
    [HideInInspector] public bool wasDragged;

    //定义多个事件，用于外部监听交互事件
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;

    void Start()
    {
        Pcanvas = GetComponentInParent<Canvas>();
        //这个方法可以在绑定物品的所有父级中寻找第一个对应的组件并返回
        image_component = GetComponent<Image>();
        characterImage = GetComponentInChildren<Image>();
        CardVisual cardVisual = GetComponent<CardVisual>();
        cardVisual.Initialize(this);

    }

    void Update()
    {
        ClampPosition();

        if (isDragging)
        {
            //Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 targetPosition = Input.mousePosition - offset;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    void ClampPosition()//计算屏幕边界，确保卡片不会超出可视范围
    {
        Vector2 screenBounds = new Vector2(Screen.width, Screen.height);
        Vector3 clampedPosition = transform.position;
        //Debug.Log(clampedPosition);
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        //Mathf.Clamp（a,b,c）若a大于b小于c，则返回a的值，否则视情况返回b,c
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent.Invoke(this);
        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition = Input.mousePosition;
        offset = mousePosition - (Vector2)transform.position;//记录鼠标偏移量
        isDragging = true;
        Pcanvas.GetComponent<GraphicRaycaster>().enabled = false;
        image_component.raycastTarget = false;
        //禁用该图像的射线检测目标。这意味着拖动时图像组件将不会接收点击或其他射线事件，确保它不干扰拖动操作
        characterImage.raycastTarget = false;
        wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);
        isDragging = false;
        Pcanvas.GetComponent<GraphicRaycaster>().enabled = true;
        image_component.raycastTarget = true;
        characterImage.raycastTarget = true;

        StartCoroutine(FrameWait());

        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();//在下一帧重置 wasDragged，防止误判点击事件。
            wasDragged = false;//如果快速拖动结束后快速点击卡牌，可能会误判点击事件为拖动事件
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PointerDownEvent.Invoke(this);
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pointerUpTime = Time.time;
        PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

        if (wasDragged || pointerUpTime - pointerDownTime > .2f)
            return;

        selected = !selected;
        SelectEvent.Invoke(this, selected);
        transform.localPosition += selected ? (cardVisual.transform.up * selectionOffset) : -cardVisual.transform.up * selectionOffset;
    }

    public void Deselect()
    {
        if (selected)
        {
            selected = false;
            if (selected)
                transform.localPosition += (cardVisual.transform.up * 50);
            else
                transform.localPosition = Vector3.zero;
        }
    }

    /*
    public int SiblingAmount()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }

    public float NormalizedPosition()
    {
        return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
    }

    private void OnDestroy()
    {
        if (cardVisual != null)
            Destroy(cardVisual.gameObject);
    }
    */

}
