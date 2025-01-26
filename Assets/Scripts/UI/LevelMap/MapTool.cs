using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTool : MonoBehaviour
{
    public static Rect GetWorldRect(SpriteRenderer sr)//������������ھ���ȷ��λ��
    {
        var position = sr.bounds.center - sr.bounds.size / 2;
        //centerΪ���ε����ĵ㣬��ȥ����֮һ�Ĵ�С�Ͷ�λ�������½ǵĵ�

        var size = sr.bounds.size;

        return new Rect(position, size);//�����½ǵĵ��λ�����Сȷ��������ε�λ��
    }
 
    public static Rect GetWorldRect(RectTransform rt, bool overlayUI = false)
    {
        if (overlayUI)
        {
            float xmin, ymin, xmax, ymax;//���½������Ͻ�����
            float scaleFactor = rt.GetComponent<Canvas>().scaleFactor;//��ȡCanvas����������
            xmin = rt.anchorMin.x * Screen.width + rt.offsetMin.x * scaleFactor;
            xmax = rt.anchorMax.x * Screen.width + rt.offsetMax.x * scaleFactor;
            ymin = rt.anchorMin.y * Screen.width + rt.offsetMin.y * scaleFactor;
            ymax = rt.anchorMax.y * Screen.width + rt.offsetMax.y * scaleFactor;
            //anchor��ê���minΪ���½ǵĵ����꣬maxΪ���Ͻǵ�
            //offset�ǵ�ê��Ϊ��״ʱminΪ���е�left��Bottom������maxΪTop��Right


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
