using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;


public class CameraEdgeScroll : MonoBehaviour
{
    private Camera c;//脚本绑定在main camera上

    public RectTransform observeWindow; //观察窗口
    public SpriteRenderer referenceBound;//参考边界，绑定地图的精灵

    [Header("Observe Parameters")]
    //这是一个 属性（Attribute），用于在 Unity Inspector 面板中为接下来的变量添加一个标题（Header）
    public Rect rfRect;
    public Rect obRect;

    public Vector2 d;

    public float zoomUpperBound; //缩放上限
    public float zoomLowerBound; //缩放下限
    public float zoomSpeed = 10; //缩放速度

    public float scrollAreaSize = 5;
    public float scrollSpeed = 5;

    public bool isDraging = false;
    public Vector2 dragStartMousePosition;
    public Vector2 dragStartCameraPosition;

    private void Awake()
    {
        c = GetComponent<Camera>();//获取绑定脚本的相机
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

        zoomUpperBound = c.orthographicSize * Mathf.Min(rfRect.width / obRect.width, rfRect.height / obRect.height) * 0.98f;//0.98的作用是保证地图不会缩小得比观察窗口更小
        zoomLowerBound = zoomUpperBound / 2;
        //2为最大放大系数，可随意更改，指能够放大的倍数为观察窗口的几倍

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
                Debug.Log("鼠标或相机初始位置获取错误");
            }

        }
    }

        void HandleZoom()
        {
            float size = c.orthographicSize;
            //放大
            if(Input.mouseScrollDelta.y>0 && size < zoomUpperBound)
            {
                Vector2 screenP0 = Input.mousePosition;
                Vector2 worldP0 = c.ScreenToWorldPoint(screenP0);

                c.orthographicSize += zoomSpeed;
                c.orthographicSize = c.orthographicSize > zoomUpperBound ? zoomUpperBound : c.orthographicSize;

                Vector2 worldP1 = c.ScreenToWorldPoint(screenP0);
                transform.Translate(worldP0 - worldP1);
            }
            //缩小
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






