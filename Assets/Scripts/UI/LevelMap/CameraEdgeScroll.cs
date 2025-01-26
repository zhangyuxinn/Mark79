using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;


public class CameraEdgeScroll : MonoBehaviour
{
    private Camera c;//�ű�����main camera��

    public RectTransform observeWindow; //�۲촰��
    public SpriteRenderer referenceBound;//�ο��߽磬�󶨵�ͼ�ľ���

    [Header("Observe Parameters")]
    //����һ�� ���ԣ�Attribute���������� Unity Inspector �����Ϊ�������ı������һ�����⣨Header��
    public Rect rfRect;
    public Rect obRect;

    public Vector2 d;

    public float zoomUpperBound; //��������
    public float zoomLowerBound; //��������
    public float zoomSpeed = 10; //�����ٶ�

    public float scrollAreaSize = 5;
    public float scrollSpeed = 5;

    public bool isDraging = false;
    public Vector2 dragStartMousePosition;
    public Vector2 dragStartCameraPosition;

    private void Awake()
    {
        c = GetComponent<Camera>();//��ȡ�󶨽ű������
    }

    private void Start()
    {
        UpdateParameter();
    }

    void Update()
    {
        //UpdateParameter();

        HandleDrag();

        if(!isDraging)
        {
            HandleZoom();
           /* if (turnOnEdgeScroll)
            {
                HandleEdgeScroll();
            }*/
        }

        Adjust();
    }

    void UpdateParameter()
    {
        rfRect = MapTool.GetWorldRect(referenceBound);
        obRect = MapTool.GetWorldRect(observeWindow,true);

        zoomUpperBound = c.orthographicSize * Mathf.Min(rfRect.width / obRect.width, rfRect.height / obRect.height) * 0.98f;//0.98�������Ǳ�֤��ͼ������С�ñȹ۲촰�ڸ�С
        zoomLowerBound = zoomUpperBound / 2;
        //2Ϊ���Ŵ�ϵ������������ģ�ָ�ܹ��Ŵ�ı���Ϊ�۲촰�ڵļ���

        scrollSpeed = obRect.width * 0.2f;
        zoomSpeed = zoomUpperBound * 0.05f;

    }

    void Adjust()
    {
        obRect = MapTool.GetWorldRect(observeWindow, true);

        float dx = Mathf.Max(0, rfRect.xMin - obRect.xMin) - Mathf.Max(0, obRect.xMax - rfRect.xMax);
        float dy = Mathf.Max(0, rfRect.yMin - obRect.yMin) - Mathf.Max(0, obRect.yMax - rfRect.yMax);

        d = new(dx, dy);
        c.transform.Translate(d);
    }

    void HandleDrag()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            isDraging = true;

            dragStartMousePosition = Input.mousePosition;
            dragStartCameraPosition = transform.position;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
        }

        if (isDraging)
        {
            if (dragStartCameraPosition != Vector2.zero && dragStartCameraPosition != Vector2.zero)
            {
                Vector2 dragCurrentMousePosition = Input.mousePosition;
                Vector2 distanceInWorldSpace = c.ScreenToWorldPoint(dragStartMousePosition) - c.ScreenToWorldPoint(dragCurrentMousePosition);

                Vector2 newPos = dragStartCameraPosition + distanceInWorldSpace;
                transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            }
            else
            {
                Debug.Log("���������ʼλ�û�ȡ����");
            }

        }
    }

        void HandleZoom()
        {
            float size = c.orthographicSize;
            //�Ŵ�
            if(Input.mouseScrollDelta.y>0 && size < zoomUpperBound)
            {
                Vector2 screenP0 = Input.mousePosition;
                Vector2 worldP0 = c.ScreenToWorldPoint(screenP0);

                c.orthographicSize += zoomSpeed;
                c.orthographicSize = c.orthographicSize > zoomUpperBound ? zoomUpperBound : c.orthographicSize;

                Vector2 worldP1 = c.ScreenToWorldPoint(screenP0);
                transform.Translate(worldP0 - worldP1);
            }
            //��С
            if(Input.mouseScrollDelta.y<0 && size > zoomLowerBound)
            {
                Vector2 screenP0 = Input.mousePosition;
                Vector2 worldP0 = c.ScreenToWorldPoint(screenP0);

                c.orthographicSize -= zoomSpeed;
                c.orthographicSize = c.orthographicSize < zoomLowerBound ? zoomLowerBound : c.orthographicSize;

                Vector2 worldP1 = c.ScreenToWorldPoint(screenP0);
                transform.Translate(worldP0 - worldP1);
            }
        }
    }






