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
    private bool initalize = false; // �Ƿ��Ѿ���ʼ��
    private Canvas canvas;

    [Header("Card")]
    public Card parentCard; // �߼���Ƭ���󣬿��ƽ�����Ϊ
    public int index;//��Ƭ��������
    private Transform cardTransform; // �߼���Ƭ�� Transform
    private Vector3 rotationDelta; // ��תƫ����
    Vector3 movementDelta; // �˶�ƫ����
    private Vector2 normalPosition;//��פλ��
    private Collider2D collider1;//��ײ���У������жϿ����Ƿ��ͷ�
    private Collider2D collider2;


    [Header("References")]
    [SerializeField] private Transform shakeParent; // �𶯶����ĸ���
    [SerializeField] private Transform tiltParent;  // ��б�����ĸ���
    [SerializeField] private Image cardImage; // ��ƬͼƬ

    [Header("Follow Parameters")]
    [SerializeField] public float followSpeed = 30; // �����ٶ�

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 20; // ��תӰ������Ƕ�
    [SerializeField] private float rotationSpeed = 20;  // ��תƽ���ٶ�

    [Header("Scale Parameters")]
    [SerializeField] private bool scaleAnimations = true; // �Ƿ��������Ŷ���
    [SerializeField] private float scaleOnHover = 1.15f; // �����ͣʱ������ֵ
    [SerializeField] private float scaleOnSelect = 1.25f; // ѡ��ʱ������ֵ
    [SerializeField] private float scaleTransition = .15f; // ���Ź���ʱ��
    [SerializeField] private Vector3 normalSize = new Vector3(0.8f, 0.8f,0.8f);
    [SerializeField] private Ease scaleEase = Ease.OutBack; // ���Ŷ�������Ч��

    [Header("Select Parameters")]
    [SerializeField] private float selectPunchAmount = 20; // ѡ��ʱ�ĵ�������

    [Header("Hover Parameters")]
    [SerializeField] private float hoverPunchAngle = 5; // ��ͣʱ����ת����
    [SerializeField] private float hoverTransition = .15f; // ��ͣ����ʱ��

    [Header("Curve")]
    [SerializeField] private CurveParameters curve; // �����������е����߲���

    private float curveYOffset; // ���� Y ��ƫ��
    private float curveRotationOffset; // ��ת�Ƕ�ƫ��
    private Coroutine pressCoroutine; // ����Э�̣�δʹ�ã�

    [Header("�¼�����")]
    [SerializeField] private CallGuardEventChannel callGuardEventChannel;
    MainUIPanel mainUIPanelInstance;

    private void Start()
    {
        // ��ʼ��ʱû�ж����߼�
      
    }

    /// ��ʼ�� CardVisual ���

    public void Initialize(Card target)
    {
        //Debug.Log("��ʼ�����");
        parentCard = target; // ���߼���Ƭ
        cardTransform = target.transform; // ��ȡ�߼���Ƭ Transform
        canvas = GetComponentInParent<Canvas>(); // ��ȡ Canvas ���
        normalPosition = target.transform.position;//��ȡ��פλ��
        mainUIPanelInstance = FindObjectOfType<MainUIPanel>();//��ȡMainUIPanel
        collider1 = GetComponent<Collider2D>();
        GameObject temp = GameObject.Find("BackCard");
        if(temp != null)
        {
           collider2 = temp.GetComponent<Collider2D>();
        }
        

        // ������Ƭ�Ľ����¼�
        parentCard.PointerEnterEvent.AddListener(PointerEnter);
        parentCard.PointerExitEvent.AddListener(PointerExit);
        parentCard.BeginDragEvent.AddListener(BeginDrag);
        parentCard.EndDragEvent.AddListener(EndDrag);
        parentCard.PointerDownEvent.AddListener(PointerDown);
        parentCard.PointerUpEvent.AddListener(PointerUp);

        initalize = true; // ����ѳ�ʼ��
    }

    /// <summary>
    /// ���¿�Ƭ�Ĳ㼶����
    /// </summary>
    /*
    public void UpdateIndex(int length)
    {
        transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
    }*/

    void Update()
    {
        if (!initalize || parentCard == null) return;

       // HandPositioning(); // ����������������
        SmoothFollow();    // ���Ӿ�λ��ƽ�������߼�λ��
        FollowRotation();  // ��ת����
    }

    /// <summary>
    /// �����������е�ƫ����
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
    /// ���Ӿ�λ��ƽ�������߼���Ƭ��λ��
    /// </summary>
    private void SmoothFollow()
    {
        Vector3 verticalOffset = (Vector3.up * (parentCard.isDragging ? 0 : curveYOffset));
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// �ÿ�Ƭ�ĽǶȸ����ƶ�������ת
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
        canvas.overrideSorting = true; // ��קʱ�øÿ�Ƭλ��ǰ��
    }

    private void EndDrag(Card card)
    {
        canvas.overrideSorting = false; // ��ק������ָ�
        transform.DOScale(normalSize, scaleTransition).SetEase(scaleEase);//�ָ�ԭ�еĴ�С
                                                                          //���������ͷź͸�λ�߼�����д��λ

        //  bool isColliding = collider1.bounds.Intersects(collider2.bounds);
        // Debug.Log("Bounds Collision: " + isColliding);

        RectTransform rt1 = collider1.GetComponent<RectTransform>();
        RectTransform rt2 = collider2.GetComponent<RectTransform>();
        //bool isUIIntersecting =  rt1.rect.Overlaps(rt2.rect, true);

        
        bool isUIIntersecting =  IsRectOverlapping(rt1,rt2);
        
        Debug.Log("UI ��ײ�����: " + isUIIntersecting);


        if (isUIIntersecting)
        {
            Debug.Log("����δ�ͷţ����и�λ");
            transform.DOMove(normalPosition, 0.5f).SetEase(Ease.OutQuad);

        }
        else
        {
            transform.position = normalPosition;
            mainUIPanelInstance.BtnCallGuard(index);
            //BtnCallGuard(index);


        }
        bool IsRectOverlapping(RectTransform rt1, RectTransform rt2)
        {
            Rect worldRect1 = GetWorldRect(rt1);
            Rect worldRect2 = GetWorldRect(rt2);
            return worldRect1.Overlaps(worldRect2);
        }

        // ��ȡ UI ����������ռ��е� Rect
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

    public void BtnCallGuard(int index)
    {
        callGuardEventChannel.Broadcast(index, Team.A);
        callGuardEventChannel.Broadcast(index, Team.B);
    }
}
