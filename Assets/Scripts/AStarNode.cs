using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum E_Node_Type
{
    Walk,
    Stop,
}

/// <summary>
/// A星格子类
/// </summary>
public class AStarNode
{

    public float F;//寻路消耗
    public float H;//距终点距离
    public float G;//距起点距离

    public AStarNode parent;//父格子

    //位置
    public int x;
    public int y;

    //格子类型
    public E_Node_Type type;

    public AStarNode(int x, int y, E_Node_Type type = E_Node_Type.Walk)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}
