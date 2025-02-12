using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CardVisual : MonoBehaviour
{
    private bool initalize = false; // 是否已经初始化
    private Canvas canvas;

    [Header("Card")]
    public Card parentCard; // 逻辑卡片对象，控制交互行为
    private Transform cardTransform; // 逻辑卡片的 Transform
    private Vector3 rotationDelta; // 旋转偏移量
    Vector3 movementDelta; // 运动偏移量
    private Vector2 normalPosition;//常驻位置
    private Collider2D collider1;//碰撞检测盒，用于判断卡牌是否释放
    private Collider2D collider2;


    [Header("References")]
    [SerializeField] private Transform shakeParent; // 震动动画的父级
    [SerializeField] private Transform tiltParent;  // 倾斜动画的父级
    [SerializeField] private Image cardImage; // 卡片图片

    [Header("Follow Parameters")]
    [SerializeField] public float followSpeed = 30; // 跟随速度

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 20; // 旋转影响的最大角度
    [SerializeField] private float rotationSpeed = 20;  // 旋转平滑速度

    [Header("Scale Parameters")]
    [SerializeField] private bool scaleAnimations = true; // 是否启用缩放动画
    [SerializeField] private float scaleOnHover = 1.15f; // 鼠标悬停时的缩放值
    [SerializeField] private float scaleOnSelect = 1.25f; // 选中时的缩放值
    [SerializeField] private float scaleTransition = .15f; // 缩放过渡时间
    [SerializeField] private Vector3 normalSize = new Vector3(0.8f, 0.8f,0.8f);
    [SerializeField] private Ease scaleEase = Ease.OutBack; // 缩放动画缓动效果

    [Header("Select Parameters")]
    [SerializeField] private float selectPunchAmount = 20; // 选中时的弹跳幅度

    [Header("Hover Parameters")]
    [SerializeField] private float hoverPunchAngle = 5; // 悬停时的旋转幅度
    [SerializeField] private float hoverTransition = .15f; // 悬停动画时间

    [Header("Curve")]
    [SerializeField] private CurveParameters curve; // 处理手牌排列的曲线参数

    private float curveYOffset; // 曲线 Y 轴偏移
    private float curveRotationOffset; // 旋转角度偏移
    private Coroutine pressCoroutine; // 长按协程（未使用）

    private void Start()
    {
        // 初始化时没有额外逻辑
      
    }

    /// 初始化 CardVisual 组件

    public void Initialize(Card target)
    {
        //Debug.Log("初始化完成");
        parentCard = target; // 绑定逻辑卡片
        cardTransform = target.transform; // 获取逻辑卡片 Transform
        canvas = GetComponentInParent<Canvas>(); // 获取 Canvas 组件
        normalPosition = target.transform.position;//获取常驻位置
        collider1 = GetComponent<Collider2D>();
        GameObject temp = GameObject.Find("BackCard");
        if(temp != null)
        {
           collider2 = temp.GetComponent<Collider2D>();
        }
        

        // 监听卡片的交互事件
        parentCard.PointerEnterEvent.AddListener(PointerEnter);
        parentCard.PointerExitEvent.AddListener(PointerExit);
        parentCard.BeginDragEvent.AddListener(BeginDrag);
        parentCard.EndDragEvent.AddListener(EndDrag);
        parentCard.PointerDownEvent.AddListener(PointerDown);
        parentCard.PointerUpEvent.AddListener(PointerUp);

        initalize = true; // 标记已初始化
    }

    /// <summary>
    /// 更新卡片的层级索引
    /// </summary>
    /*
    public void UpdateIndex(int length)
    {
        transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
    }*/

    void Update()
    {
        if (!initalize || parentCard == null) return;

       // HandPositioning(); // 处理手牌曲线排列
        SmoothFollow();    // 让视觉位置平滑跟随逻辑位置
        FollowRotation();  // 旋转处理
    }

    /// <summary>
    /// 计算曲线排列的偏移量
    /// </summary>
    /// 
    /*
    private void HandPositioning()
    {
        curveYOffset = (curve.positioning.Evaluate(parentCard.NormalizedPosition()) * curve.positioningInfluence) * parentCard.SiblingAmount();
        curveYOffset = parentCard.SiblingAmount() < 5 ? 0 : curveYOffset;
        curveRotationOffset = curve.rotation.Evaluate(parentCard.NormalizedPosition());
    }*/

    /// <summary>
    /// 让视觉位置平滑跟随逻辑卡片的位置
    /// </summary>
    private void SmoothFollow()
    {
        Vector3 verticalOffset = (Vector3.up * (parentCard.isDragging ? 0 : curveYOffset));
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 让卡片的角度根据移动方向旋转
    /// </summary>
    private void FollowRotation()
    {
        Vector3 movement = (transform.position - cardTransform.position);
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = (parentCard.isDragging ? movementDelta : movement) * rotationAmount;
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
    }

    private void BeginDrag(Card card)
    {
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);
        canvas.overrideSorting = true; // 拖拽时让该卡片位于前景
    }

    private void EndDrag(Card card)
    {
        canvas.overrideSorting = false; // 拖拽结束后恢复
        transform.DOScale(normalSize, scaleTransition).SetEase(scaleEase);//恢复原有的大小
                                                                          //下面增加释放和复位逻辑，先写复位

        //  bool isColliding = collider1.bounds.Intersects(collider2.bounds);
        // Debug.Log("Bounds Collision: " + isColliding);

        RectTransform rt1 = collider1.GetComponent<RectTransform>();
        RectTransform rt2 = collider2.GetComponent<RectTransform>();
        //bool isUIIntersecting =  rt1.rect.Overlaps(rt2.rect, true);

        
        bool isUIIntersecting =  IsRectOverlapping(rt1,rt2);
        
        Debug.Log("UI 碰撞检测结果: " + isUIIntersecting);


        if (isUIIntersecting)
        {
            Debug.Log("卡牌未释放，进行复位");
            transform.DOMove(normalPosition, 0.5f).SetEase(Ease.OutQuad);

        }
        bool IsRectOverlapping(RectTransform rt1, RectTransform rt2)
        {
            Rect worldRect1 = GetWorldRect(rt1);
            Rect worldRect2 = GetWorldRect(rt2);
            return worldRect1.Overlaps(worldRect2);
        }

        // 获取 UI 物体在世界空间中的 Rect
        Rect GetWorldRect(RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }

    }

    private void PointerEnter(Card card)
    {
        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);

        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
    }

    private void PointerExit(Card card)
    {
        if (!parentCard.wasDragged)
            transform.DOScale(normalSize, scaleTransition).SetEase(scaleEase);
    }

    private void PointerUp(Card card, bool longPress)
    {
        if (scaleAnimations)
            transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase);
        canvas.overrideSorting = false;
    }

    private void PointerDown(Card card)
    {
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);
    }
}
