using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AStarFind : MonoBehaviour
{
    
    //TODO:还没检测障碍的位置，还没做核心算法
    [Header("map部分")] 
    [SerializeField] private float mapXSize;
    [FormerlySerializedAs("mapYSize")] [SerializeField] private float mapZSize;
    [SerializeField] private Transform centerMap;
    [SerializeField] private int subdivisionDegree=100;//细分程度
    
    private List<List<AStarNode>> nodeMap=new List<List<AStarNode>>();
    [Header("AStar部分")]
    private Queue<AStarNode> que;
    [SerializeField] private LayerMask mapMask;
    private Vector2[] dirInGraph = new Vector2[8];
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        dirInGraph[0] = new Vector2(0, 1);
        dirInGraph[1] = new Vector2(1, 1);
        dirInGraph[2] = new Vector2(1, 0);
        dirInGraph[3] = new Vector2(1, -1);
        dirInGraph[4] = new Vector2(0, -1);
        dirInGraph[5] = new Vector2(-1, -1);
        dirInGraph[6] = new Vector2(-1, 0);
        dirInGraph[7] = new Vector2(-1, 1);
        float subX =mapXSize/subdivisionDegree;
        float subZ =mapZSize/subdivisionDegree;
        float startX =centerMap.position.x-mapXSize/2;
        float startZ =centerMap.position.y-mapZSize/2;
        
        for (int i = 0; i < subdivisionDegree; i++)
        {
            List<AStarNode> tmpAStarNodes = new List<AStarNode>();
            for (int j = 0; j < subdivisionDegree; j++)
            {
                tmpAStarNodes.Add(new AStarNode(startX+i*subX,startZ+j*subZ,i,j));
            }    
            nodeMap.Add(tmpAStarNodes);
        }

    }

    
    public List<Vector3> AStarFindPath(Vector3 startPos, Vector3 endPos)
    {
        List<AStarNode> storeNodes=new List<AStarNode>();
        
        List<Vector3> result=new List<Vector3>();
        Vector3 dir = endPos-startPos;
        float dist = dir.magnitude;
        RaycastHit hit;
        //找离起点最近的节点。
        if (startPos.x > centerMap.position.x + mapXSize / 2
            || startPos.x < centerMap.position.x - mapXSize / 2
            || startPos.z > centerMap.position.z + mapZSize / 2
            || startPos.z < centerMap.position.z - mapZSize / 2) return result;
        int startX = (int)((startPos.x - centerMap.position.x)*subdivisionDegree / mapXSize + subdivisionDegree / 2);
        int startZ = (int)((startPos.z - centerMap.position.z)*subdivisionDegree / mapZSize + subdivisionDegree / 2);
        
        //找离终点最近的节点
        if (endPos.x > centerMap.position.x + mapXSize / 2
            || endPos.x < centerMap.position.x - mapXSize / 2
            || endPos.z > centerMap.position.z + mapZSize / 2
            || endPos.z < centerMap.position.z - mapZSize / 2) return result;
        int endX = (int)((endPos.x - centerMap.position.x)*subdivisionDegree / mapXSize + subdivisionDegree / 2);
        int endZ = (int)((endPos.z - centerMap.position.z)*subdivisionDegree / mapZSize + subdivisionDegree / 2);

        AStarNode startNode = nodeMap[startX][startZ];
        AStarNode endNode = nodeMap[endX][endZ];
        //正式开始算最短路径
        storeNodes.Add(startNode);
        while (storeNodes.Count!=0)
        {
            AStarNode curNode = storeNodes[0];
            AStarNode nextNode;
            if(curNode==endNode)break;
            for (int i = 0; i < 8; i++)
            {
                
                nextNode = nodeMap[curNode.Mapx+(int)dirInGraph[i].x][curNode.Mapy+(int)dirInGraph[i].y];
            }
        }
        
        
        if (Physics.Raycast(startPos, dir, out hit, dist,mapMask))
        {

        }
        else
        {
            result.Add(endPos);

        }
        return result;
    }

}

