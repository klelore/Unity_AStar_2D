using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    public int beginX=-3;
    public int beginY=5;

    public int offsetX=2;
    public int offsetY=2;

    public int mapW=5;
    public int mapH=5;


    private Dictionary<string, GameObject> cubes=new Dictionary<string, GameObject>();

    public Material red;
    public Material yellow;
    public Material green;
    public Material write;

    private void Start()
    {
        AStarManger.Instance.InitMapInfo(mapW, mapH);
        AStarManger.Instance.mapH = mapH;
        AStarManger.Instance.mapW = mapW;
        for(int i=0;i<mapW;i++)
        {
            for(int j=0;j<mapH;j++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                obj.transform.parent = transform;
                obj.transform.position = new Vector2(beginX + i * offsetX, beginY + j * offsetY);

                obj.name = i + "_" + j;
                cubes.Add(obj.name, obj);

                AStarNode node = AStarManger.Instance.nodes[i, j];
                if(node.type == E_Node_Type.Stop)
                {
                    obj.GetComponent<MeshRenderer>().material = red;
                }
            }
        }
    }


    private Vector2 beginPos = Vector2.right * -1;
    List<AStarNode> path;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (beginPos == Vector2.right * -1)
                {
                    if (path!=null)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {
                            cubes[path[i].x + "_" + path[i].y].GetComponent<MeshRenderer>().material = write;
                        }
                    }


                    string[] strs = hit.collider.gameObject.name.Split('_');
                    beginPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));

                    hit.collider.gameObject.GetComponent<MeshRenderer>().material = yellow;
                } else
                {
                    cubes[beginPos.x + "_" + beginPos.y].GetComponent<MeshRenderer>().material = write;
                    string[] strs = hit.collider.gameObject.name.Split('_');
                    Vector2 endPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));


                    path = AStarManger.Instance.FindPath(beginPos,endPos);

                    if(path!= null)
                    {
                        for(int i=0;i<path.Count;i++)
                        {
                            cubes[path[i].x + "_" + path[i].y].GetComponent<MeshRenderer>().material = green;
                        }
                    }

                    beginPos = Vector2.right * -1;
                }

            }
        }
    }


}
