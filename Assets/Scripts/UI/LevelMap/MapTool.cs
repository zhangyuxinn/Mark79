using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTool : MonoBehaviour
{
    public static Rect GetWorldRect(SpriteRenderer sr)//这个方法适用于精灵确定位置
    {
        var position = sr.bounds.center - sr.bounds.size / 2;
        //center为矩形的中心点，减去二分之一的大小就定位到了左下角的点

        var size = sr.bounds.size;

        return new Rect(position, size);//由左下角的点的位置与大小确定这个矩形的位置
    }
 
    public static Rect GetWorldRect(RectTransform rt, bool overlayUI = false)
    {
        if (overlayUI)
        {
            float xmin, ymin, xmax, ymax;//左下角与右上角坐标
            float scaleFactor = rt.GetComponent<Canvas>().scaleFactor;//获取Canvas的缩放因子
            xmin = rt.anchorMin.x * Screen.width + rt.offsetMin.x * scaleFactor;
            xmax = rt.anchorMax.x * Screen.width + rt.offsetMax.x * scaleFactor;
            ymin = rt.anchorMin.y * Screen.width + rt.offsetMin.y * scaleFactor;
            ymax = rt.anchorMax.y * Screen.width + rt.offsetMax.y * scaleFactor;
            //anchor是锚点框，min为左下角的点坐标，max为右上角的
            //offset是当锚点为框状时min为其中的left与Bottom参数，max为Top与Right


            Vector2 leftBottom = new(xmin,ymin);
            Vector2 rightTop = new(xmax,ymax);

            leftBottom = Camera.main.ScreenToWorldPoint(leftBottom);
            rightTop = Camera.main.ScreenToWorldPoint(rightTop);

            return new Rect(leftBottom, rightTop - leftBottom);
        }

        else
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector2 size = corners[2] - corners[0];
            Vector3 position = corners[0];
            return new Rect(position, size);
        }
    }

}
