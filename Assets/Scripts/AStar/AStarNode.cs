using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{ 
    public float x, z;
    public int Mapx, Mapy;
    public int backValue, frontValue, totalValue;

    public AStarNode(float X,float Y,int getMapx ,int getMapy)
    {
        x = X;
        z = Y;
        Mapx = getMapx;
        Mapy = getMapy;
    }
}
