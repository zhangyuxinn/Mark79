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
    private bool initalize = false;
    //这是一个私有布尔字段，用于标记初始化是否完成
    [Header("Card")]
    public Card 父卡片;
    //这是一个公共字段，用于存储父卡片的引用，[Header("Card")]用于在Unity编辑器中为字段分组
    private Transform 卡片变换;
    private Vector3 旋转增量;
    private int 保存索引;
    Vector3 移动增量;
    private Canvas canvas;
    //这些是私有字段，用于存储卡片的变换、旋转增量、保存的索引、移动增量和Canvas组件
    [Header("References")]
    public Transform 视觉阴影;//这是一个公共字段，用于存储视觉阴影的变换
    private float 阴影偏移 = 20;
    private Vector2 阴影距离;
    private Canvas 阴影Canvas;
    [SerializeField] private Transform 摇动父对象;
    [SerializeField] private Transform 倾斜父对象;
    [SerializeField] private Image 卡片图像;
    //这些是私有字段，用于存储阴影偏移、阴影距离、阴影Canvas、摇动父对象、倾斜父对象和卡片图像的引用。
    [Header("Follow Parameters")]
    [SerializeField] private float 跟随速度 = 30;
    //这是一个序列化的私有字段，用于设置跟随速度
    [Header("Rotation Parameters")]
    [SerializeField] private float 旋转量 = 20;
    [SerializeField] private float 旋转速度 = 20;
    //这些是序列化的私有字段，用于设置旋转的量和速度
    [SerializeField] private float autoTiltAmount = 30;
    [SerializeField] private float manualTiltAmount = 20;
    [SerializeField] private float tiltSpeed = 20;
    
    [Header("Scale Parameters")]
    [SerializeField] private bool scaleAnimations = true;
    [SerializeField] private float scaleOnHover = 1.15f;
    [SerializeField] private float scaleOnSelect = 1.25f;
    [SerializeField] private float scaleTransition = .15f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    //这些是序列化的私有字段，用于设置缩放动画的参数
    [Header("Select Parameters")]
    [SerializeField] private float 选择时的冲击量 = 20;
    //这是一个序列化的私有字段，用于设置选择时的冲击量
    [Header("Hober Parameters")]
    [SerializeField] private float 悬停时的冲击角度 = 5;
    [SerializeField] private float 悬停时过渡时间 = .15f;
    //这些是序列化的私有字段，用于设置悬停时的冲击角度和过渡时间
    [Header("Swap Parameters")]
    [SerializeField] private bool swapAnimations = true;
    [SerializeField] private float swapRotationAngle = 30;
    [SerializeField] private float swapTransition = .15f;
    [SerializeField] private int swapVibrato = 5;
    //这些是序列化的私有字段，用于设置交换动画的参数
    [Header("Curve")]
    [SerializeField] private CurveParameters curve;
    //这是一个序列化的私有字段，用于存储曲线参数
    private float curveYOffset;
    private float curveRotationOffset;
    private Coroutine pressCoroutine;
    //这些是私有字段，用于存储曲线的Y偏移和旋转偏移，以及一个协程引用
    private void Start()
    {//Start方法在脚本实例化时调用，用于初始化阴影距离
        阴影距离 = 视觉阴影.localPosition;
    }

    public void Initialize(Card target, int index = 0)
    {//这是一个公共方法，用于初始化CardVisual组件，设置父卡片、事件监听器等
        //Declarations
        父卡片 = target;
        卡片变换 = target.transform;
        canvas = GetComponent<Canvas>();
        阴影Canvas = 视觉阴影.GetComponent<Canvas>();

        //Event Listening
        父卡片.PointerEnterEvent.AddListener(PointerEnter);
        父卡片.PointerExitEvent.AddListener(PointerExit);
        父卡片.BeginDragEvent.AddListener(BeginDrag);
        父卡片.EndDragEvent.AddListener(EndDrag);
        父卡片.PointerDownEvent.AddListener(PointerDown);
        父卡片.PointerUpEvent.AddListener(PointerUp);
        父卡片.SelectEvent.AddListener(Select);

        //Initialization
        initalize = true;
    }

    public void UpdateIndex(int length)
    {//这是一个公共方法，用于更新卡片的视觉索引
        transform.SetSiblingIndex(父卡片.transform.parent.GetSiblingIndex());
    }

    void Update()
    {//每帧调用，用于执行手的位置、平滑跟随、跟随旋转和卡片倾斜的逻辑
        if (!initalize || 父卡片 == null) return;

        HandPositioning();
        SmoothFollow();
        FollowRotation();
        CardTilt();

    }

    private void HandPositioning()
    {//这个方法用于处理手的位置逻辑
        curveYOffset = (curve.positioning.Evaluate(父卡片.NormalizedPosition()) * curve.positioningInfluence) * 父卡片.SiblingAmount();
        curveYOffset = 父卡片.SiblingAmount() < 5 ? 0 : curveYOffset;
        curveRotationOffset = curve.rotation.Evaluate(父卡片.NormalizedPosition());
    }

    private void SmoothFollow()
    {//这个方法用于处理平滑跟随逻辑
        Vector3 verticalOffset = (Vector3.up * (父卡片.是否正在拖动 ? 0 : curveYOffset));
        transform.position = Vector3.Lerp(transform.position, 卡片变换.position + verticalOffset, 跟随速度 * Time.deltaTime);
    }

    private void FollowRotation()
    {//这个方法用于处理跟随旋转逻辑
        Vector3 movement = (transform.position - 卡片变换.position);
        移动增量 = Vector3.Lerp(移动增量, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = (父卡片.是否正在拖动 ? 移动增量 : movement) * 旋转量;
        旋转增量 = Vector3.Lerp(旋转增量, movementRotation, 旋转速度 * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(旋转增量.x, -60, 60));
    }

    private void CardTilt()
    {//这个方法用于处理卡片倾斜逻辑
        保存索引 = 父卡片.是否正在拖动 ? 保存索引 : 父卡片.ParentIndex();
        float sine = Mathf.Sin(Time.time + 保存索引) * (父卡片.是否悬停 ? .2f : 1);
        float cosine = Mathf.Cos(Time.time + 保存索引) * (父卡片.是否悬停 ? .2f : 1);

        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float tiltX = 父卡片.是否悬停 ? ((offset.y * -1) * manualTiltAmount) : 0;
        float tiltY = 父卡片.是否悬停 ? ((offset.x) * manualTiltAmount) : 0;
        float tiltZ = 父卡片.是否正在拖动 ? 倾斜父对象.eulerAngles.z : (curveRotationOffset * (curve.rotationInfluence * 父卡片.SiblingAmount()));

        float lerpX = Mathf.LerpAngle(倾斜父对象.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(倾斜父对象.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(倾斜父对象.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

        倾斜父对象.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    private void Select(Card card, bool state)
    {//这个方法用于处理选择逻辑
        DOTween.Kill(2, true);
        float dir = state ? 1 : 0;
        摇动父对象.DOPunchPosition(摇动父对象.up * 选择时的冲击量 * dir, scaleTransition, 10, 1);
        摇动父对象.DOPunchRotation(Vector3.forward * (悬停时的冲击角度 / 2), 悬停时过渡时间, 20, 1).SetId(2);

        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);

    }

    public void Swap(float dir = 1)
    {//这是一个公共方法，用于处理交换逻辑
        if (!swapAnimations)
            return;

        DOTween.Kill(2, true);
        摇动父对象.DOPunchRotation((Vector3.forward * swapRotationAngle) * dir, swapTransition, swapVibrato, 1).SetId(3);
    }

    private void BeginDrag(Card card)
    {//这个方法用于处理开始拖动逻辑
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

        canvas.overrideSorting = true;
    }

    private void EndDrag(Card card)
    {//这个方法用于处理结束拖动逻辑
        canvas.overrideSorting = false;
        transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    private void PointerEnter(Card card)
    {//这个方法用于处理指针进入逻辑
        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);

        DOTween.Kill(2, true);
        摇动父对象.DOPunchRotation(Vector3.forward * 悬停时的冲击角度, 悬停时过渡时间, 20, 1).SetId(2);
    }

    private void PointerExit(Card card)
    {//这个方法用于处理指针离开逻辑
        if (!父卡片.是否被拖动过)
            transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    private void PointerUp(Card card, bool longPress)
    {//这个方法用于处理指针释放逻辑
        if (scaleAnimations)
            transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase);
        canvas.overrideSorting = false;

        视觉阴影.localPosition = 阴影距离;
        阴影Canvas.overrideSorting = true;
    }

    private void PointerDown(Card card)
    {//这个方法用于处理指针按下逻辑
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

        视觉阴影.localPosition += (-Vector3.up * 阴影偏移);
        阴影Canvas.overrideSorting = false;
    }

}

