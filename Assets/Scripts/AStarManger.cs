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


    public AStarNode[,] nodes;//��ͼ���и���

    public List<AStarNode> openList = new List<AStarNode>();//�����б�
    public List<AStarNode> closeList = new List<AStarNode>();//�ر��б�

    //��ͼ��С
    public int mapH;
    public int mapW;


    //��������
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

    //�Ƚ�ʵ�ʵ�ͼ�������Ƿ�
    public bool PointEquals(object a, object b)
    {
        return Mathf.Abs((float)a - (float)b) <= 0.1;
    }

    //Ѱ·��������
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

    //�����پ��빫ʽ
    public float ManhattanDistance(AStarNode a, AStarNode b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    //���ȶ����ϸ�
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

    //Ѱ�����·��
    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        //��С�������ֵΪ��
        openList.Add(null);


        //�ж��Ƿ��ڵ�ͼ��
        if (startPos.x < 0 || startPos.x >= mapW ||
            startPos.y < 0 || startPos.y >=mapH ||
            endPos.x < 0 || endPos.x >= mapW ||
            endPos.y < 0 || endPos.y >= mapH)
        {
            Debug.Log("�������յ�û���ڵ�ͼ��"+startPos+endPos);
            return null;
        }

        //�ж��Ƿ����ϰ�
        AStarNode startNode = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode endNode = nodes[(int)endPos.x, (int)endPos.y];
        if (startNode.type == E_Node_Type.Stop || endNode.type == E_Node_Type.Stop)
        {
            Debug.Log("�������յ����ϰ�");
            return null;
        }

        //��չرպͿ����б�

        openList.Clear();
        closeList.Clear();


        //����ʼ�����ر��б�
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
                //�Ƿ��ڵ�ͼ��
                if ((current.x + pointDis[i][0] >= mapW) || (current.y + pointDis[i][1] >= mapH) ||
                    (current.x + pointDis[i][0] < 0) || (current.y + pointDis[i][1] < 0))
                {
                    continue;
                }

                AStarNode nextNode = nodes[current.x + (int)pointDis[i][0], current.y + (int)pointDis[i][1]];

                //�����б�͹ر��б��Ƿ��������㣬����������Ƿ����ϰ�
                if (nextNode == null ||
                    openList.Contains(nextNode) ||
                    closeList.Contains(nextNode) ||
                    nextNode.type == E_Node_Type.Stop)
                {
                    continue;
                }


                nextNode.parent = current;

                //����Gֵ
                float G = 0;
                if (pointDis[i][0] == 0 || pointDis[i][1] == 0)
                {
                    G = 1.0f;
                } else
                {
                    G = 1.4f;
                }
                nextNode.G = nextNode.parent.G + G;//��������
                nextNode.H = ManhattanDistance(nextNode, endNode);//���յ����

                nextNode.F = nextNode.G + nextNode.H;//��������

                openList.Add(nextNode);
                Up(openList, openList.Count - 1);
            }

            if(openList.Count <= 1)//ͨ���жϿ����б�������û�нڵ����ж���û��·�����Ե����յ�
            {
                Debug.Log("û��·���ܵ����յ�");
                return null;
            }


            current = openList[1];//ȡ����һ��Ԫ��
            openList.RemoveAt(1);//�����б�ȥ����һ��Ԫ��
            closeList.Add(current);//�ر��б������ϵ�Ԫ��




            if (current == endNode)//�������յ�ʱ��ͨ��parent�ڵ����ҵ����·��
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
