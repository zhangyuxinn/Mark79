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
    private Image image_component;//���Ƶ׿�
    private Image characterImage;//��ɫͼƬ
   // private VisualCardsHandler visualHandler;
    private Vector3 offset;

    [SerializeField] private float moveSpeedLimit = 5000;//�����ƶ��ٶ�����

    public bool selected;
    public float selectionOffset = 50;//��ѡ��ʱ��ƫ����
    private float pointerDownTime;
    private float pointerUpTime;


    [SerializeField] private GameObject cardVisualPrefab;//��Ƭ���Ӿ�Ч��Ԥ����
    [HideInInspector] public CardVisual cardVisual;//��ǰ��Ƭ�� CardVisual ���ʵ��

    public bool isHovering;
    public bool isDragging;
    [HideInInspector] public bool wasDragged;

    //�������¼��������ⲿ���������¼�
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
        //������������ڰ���Ʒ�����и�����Ѱ�ҵ�һ����Ӧ�����������
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

    void ClampPosition()//������Ļ�߽磬ȷ����Ƭ���ᳬ�����ӷ�Χ
    {
        Vector2 screenBounds = new Vector2(Screen.width, Screen.height);
        Vector3 clampedPosition = transform.position;
        //Debug.Log(clampedPosition);
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        //Mathf.Clamp��a,b,c����a����bС��c���򷵻�a��ֵ���������������b,c
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent.Invoke(this);
        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition = Input.mousePosition;
        offset = mousePosition - (Vector2)transform.position;//��¼���ƫ����
        isDragging = true;
        Pcanvas.GetComponent<GraphicRaycaster>().enabled = false;
        image_component.raycastTarget = false;
        //���ø�ͼ������߼��Ŀ�ꡣ����ζ���϶�ʱͼ�������������յ�������������¼���ȷ�����������϶�����
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
            yield return new WaitForEndOfFrame();//����һ֡���� wasDragged����ֹ���е���¼���
            wasDragged = false;//��������϶���������ٵ�����ƣ����ܻ����е���¼�Ϊ�϶��¼�
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
