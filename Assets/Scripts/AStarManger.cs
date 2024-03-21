using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;


public class AStarManger
{

    private static AStarManger instance;
    public static AStarManger Instance
    {
        get
        {
            if (instance == null)
                instance = new AStarManger();
            return instance;
        }
        set
        {
            instance = value;
        }
    }


    public AStarNode[,] nodes;//地图所有格子

    public List<AStarNode> openList = new List<AStarNode>();//开启列表
    public List<AStarNode> closeList = new List<AStarNode>();//关闭列表

    //地图大小
    public int mapH;
    public int mapW;


    //创建格子
    public void InitMapInfo(int w, int h, List<Vector2> block=null)
    {
        nodes = new AStarNode[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {

                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk);

                /*                if (k < block.Count && PointEquals(block[k].x,i) && PointEquals(block[k].y,j))
                                {
                                    node.type = E_Node_Type.Stop;
                                }else
                                    node.type = E_Node_Type.Walk;*/

                nodes[i, j] = node;
            }
        }
    }

    //比较实际地图到格子是否
    public bool PointEquals(object a, object b)
    {
        return Mathf.Abs((float)a - (float)b) <= 0.1;
    }

    //寻路方向数组
    private List<Vector2> pointDis = new List<Vector2>
    {
        new Vector2(0,1),
        new Vector2(0,-1),
        new Vector2(1,1),
        new Vector2(1,0),
        new Vector2(1,-1),
        new Vector2(-1,1),
        new Vector2(-1,0),
        new Vector2(-1,-1),

    };

    //曼哈顿距离公式
    public float ManhattanDistance(AStarNode a, AStarNode b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    //优先队列上浮
    public int Up(List<AStarNode> openList, int n)
    {
        if (openList.Count <= 2 || n <= 1) return 0;
        if (openList[n].F > openList[n / 2].F)
            return 0;

        AStarNode temp = openList[n];
        openList[n] = openList[n / 2];
        openList[n / 2] = temp;

        return Up(openList, n / 2);
    }

    //寻找最短路径
    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        //最小堆排序初值为空
        openList.Add(null);


        //判断是否在地图内
        if (startPos.x < 0 || startPos.x >= mapW ||
            startPos.y < 0 || startPos.y >=mapH ||
            endPos.x < 0 || endPos.x >= mapW ||
            endPos.y < 0 || endPos.y >= mapH)
        {
            Debug.Log("起点或者终点没有在地图内"+startPos+endPos);
            return null;
        }

        //判断是否是障碍
        AStarNode startNode = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode endNode = nodes[(int)endPos.x, (int)endPos.y];
        if (startNode.type == E_Node_Type.Stop || endNode.type == E_Node_Type.Stop)
        {
            Debug.Log("起点或者终点是障碍");
            return null;
        }

        //清空关闭和开启列表

        openList.Clear();
        closeList.Clear();


        //将开始点加入关闭列表
        startNode.parent = null;
        startNode.G = 0;
        startNode.H = 0;
        startNode.F = 0;
        closeList.Add(startNode);



        AStarNode current = startNode;
        while (true)
        {
            for (int i = 0; i < 8; i++)
            {
                //是否在地图内
                if ((current.x + pointDis[i][0] >= mapW) || (current.y + pointDis[i][1] >= mapH) ||
                    (current.x + pointDis[i][0] < 0) || (current.y + pointDis[i][1] < 0))
                {
                    continue;
                }

                AStarNode nextNode = nodes[current.x + (int)pointDis[i][0], current.y + (int)pointDis[i][1]];

                //开启列表和关闭列表是否包含这个点，并且这个点是否是障碍
                if (nextNode == null ||
                    openList.Contains(nextNode) ||
                    closeList.Contains(nextNode) ||
                    nextNode.type == E_Node_Type.Stop)
                {
                    continue;
                }


                nextNode.parent = current;

                //计算G值
                float G = 0;
                if (pointDis[i][0] == 0 || pointDis[i][1] == 0)
                {
                    G = 1.0f;
                } else
                {
                    G = 1.4f;
                }
                nextNode.G = nextNode.parent.G + G;//距起点距离
                nextNode.H = ManhattanDistance(nextNode, endNode);//距终点距离

                nextNode.F = nextNode.G + nextNode.H;//距离消耗

                openList.Add(nextNode);
                Up(openList, openList.Count - 1);
            }

            if(openList.Count <= 1)//通过判断开启列表里面有没有节点来判定有没有路径可以到达终点
            {
                Debug.Log("没有路径能到达终点");
                return null;
            }


            current = openList[1];//取出第一个元素
            openList.RemoveAt(1);//开启列表去除第一个元素
            closeList.Add(current);//关闭列表加入符合的元素




            if (current == endNode)//当到达终点时，通过parent节点来找到最近路径
            {
                List<AStarNode> path = new List<AStarNode>();
                
                path.Add(endNode);
                while(endNode.parent!=null)
                {
                    path.Add(endNode.parent);
                    endNode = endNode.parent;
                }

                path.Reverse();

                return path;
            }
        }

    }

}
