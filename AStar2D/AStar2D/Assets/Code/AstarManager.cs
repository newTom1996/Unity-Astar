using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum NodeType
{
    Normal = 0,
    Start = 1,
    End = 2,
    Obstacle = 3,
    Path = 4,
}
public class AstarManager : MonoBehaviour
{
    #region Config
    public GameObject NodePrefab;
    public Camera MainCam;
    public int Width = 10;
    public int Height = 10;
    #endregion
    
    #region C++

    [DllImport("AstarDll", EntryPoint = "Add")]
    public static extern int Add(int x,int y);
    
    [DllImport("AstarDll")]
    public static extern IntPtr GenerateEcho(float x);
	
    [DllImport("AstarDll")]
    public static extern void SetX(IntPtr echo,float x);
	
    [DllImport("AstarDll")]
    public static extern float GetX(IntPtr echo);
	
    [DllImport("AstarDll")]
    public static extern void ReleaseEcho(IntPtr echo);
    
    [DllImport("AstarDll", EntryPoint = "CalculatePath")]
    public static extern unsafe void CalculatePath(int*[] map,int width,int height,ref int resultX, ref int resultY);

    #endregion
    
    /// <summary>
    /// 地图数据
    /// </summary>
    private Node[,] map;
    
    /// <summary>
    /// 路径结果列表横坐标
    /// </summary>
    private int[] resultX;
    
    /// <summary>
    /// 路径结果列表纵坐标
    /// </summary>
    private int[] resultY;
    
    private static AstarManager instacne;
    public static AstarManager Instance
    {
        get
        {
            if (instacne == null)
            {
                instacne = FindObjectOfType(typeof(AstarManager)) as AstarManager;
                if (instacne == null)
                {
                    GameObject obj = new GameObject();
                    instacne = (AstarManager)obj.AddComponent(typeof(AstarManager));
                }
            }
            return instacne;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instacne == null)
        {
            instacne = this;
        }

        resultX = new int[Width * Height];
        resultY = new int[Width * Height];
    }

    void Start () {
        
        //c++
        // IntPtr echo = GenerateEcho(3.0f);
        // Debug.Log(GetX(echo));
        // SetX(echo,2.5f);
        // Debug.Log(GetX(echo));
        // ReleaseEcho(echo);
        
       GenerateGrid(Width,Height);
    }

    /// <summary>
    /// 生成网格
    /// </summary>
    public void GenerateGrid(int width,int height)
    {
        map = new Node[width,height];
        Vector3 centerPos = Vector3.zero;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject nodeObj = GameObject.Instantiate(NodePrefab, null, true);
                nodeObj.transform.position = new Vector3(i,j,0);
                centerPos += nodeObj.transform.position;
                Node node = nodeObj.GetComponent<Node>();
                map[i,j] = node;
            }
        }

        centerPos = centerPos / (width * height);
        MainCam.transform.position = new Vector3(centerPos.x,centerPos.y,MainCam.transform.position.z);
        MainCam.orthographicSize = width / 1.5f;
    }

    /// <summary>
    /// 重置网格
    /// </summary>
    public void ResetGrid()
    {
        foreach (var node in map)
        {
            node.SwitchType(NodeType.Normal);
        }
    }
    
    /// <summary>
    /// 计算路径
    /// </summary>
    public void CalculatePath()
    {
        int[,] mapInt = new int[Width,Height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Node curNode = map[i, j];
                mapInt[Height - j - 1, i] = (int) curNode.CurType;
                // //TODO 测试
                // if (curNode.CurType == NodeType.Obstacle)
                // {
                //     Debug.Log("障碍（" + (Height - j - 1) + "," + (i) + "）");
                // }
            }
        }
        int row = mapInt.GetUpperBound(0) + 1;
        int col = mapInt.GetUpperBound(1) + 1;
        unsafe
        {
            fixed (int* fp = mapInt)
            {
                int*[] farr = new int*[row];
                for (int i = 0; i < row; i++)
                {
                    farr[i] = fp + i * col;
                }
                CalculatePath(farr,row, col, ref resultX[0], ref resultY[0]);
            }
        }
        // unsafe
        // {
        //     fixed (int* fp = mapInt)
        //     {
        //         int*[] farr = new int*[Width];
        //         for (int i = 0; i < Width; i++)
        //         {
        //             farr[i] = fp + i * Height;
        //         }
        //         CalculatePath(farr,Width, Height, ref resultX[0], ref resultY[0]);
        //     }
        // }
        
        bool hasRecordZero = false;
        for (int i = 0; i < resultX.Length; i++)
        {
            if (resultX[i] == 0 && resultY[i] == 0)
            {
                continue;
            }
            //Debug.Log("坐标" + "(" + resultX[i] + "," + resultY[i] + ")");
            // int xPos = resultY[i];
            // int yPos = Height - 1 - resultX[i];
            Debug.Log("坐标" + "(" + resultX[i] + "," + resultY[i] + ")");
            int xPos = resultY[i];
            int yPos = Height - 1 - resultX[i];
            if (resultX[i] == 0 && resultY[i] == 0)
            {
                if (hasRecordZero)
                {
                    continue;
                }
                //map[resultX[i],resultY[i]].SetToPathNode();
                //Debug.Log("坐标" + "(" + xPos + "," + yPos + ")");
                hasRecordZero = true;
            }
            else
            {
                //Debug.Log("坐标" + "(" + xPos + "," + yPos + ")");
                map[xPos,yPos].SetToPathNode();
            }
        }
    }

    /// <summary>
    /// 更新节点类型
    /// </summary>
    /// <param name="mousePos"></param>
    /// <param name="nodeType"></param>
    public void UpdateAllNodeType(Vector3 mousePos,NodeType nodeType)
    {
        foreach (var node in map)
        {
            node.OnUpdateType(mousePos,nodeType);
        }
    }
}
