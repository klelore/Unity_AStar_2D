using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum E_Node_Type
{
    Walk,
    Stop,
}

/// <summary>
/// A�Ǹ�����
/// </summary>
public class AStarNode
{

    public float F;//Ѱ·����
    public float H;//���յ����
    public float G;//��������

    public AStarNode parent;//������

    //λ��
    public int x;
    public int y;

    //��������
    public E_Node_Type type;

    public AStarNode(int x, int y, E_Node_Type type = E_Node_Type.Walk)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}
