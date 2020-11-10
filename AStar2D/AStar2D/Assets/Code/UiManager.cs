using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private bool isCheckChooseType;
    private bool isCheckResetType;
    private NodeType curChooseNodeType;
    void Start()
    {
        
    }

    public void OnClickResetBtn()
    {
        AstarManager.Instance.ResetGrid();
    }

    public void OnClickCalculateBtn()
    {
        AstarManager.Instance.CalculatePath();
    }

    public void OnClickStartBtn()
    {
        curChooseNodeType = NodeType.Start;
    }

    public void OnClickEndBtn()
    {
        curChooseNodeType = NodeType.End;
    }
    
    public void OnClickObstacleBtn()
    {
        curChooseNodeType = NodeType.Obstacle;
    }
    void Update()
    {
        if (isCheckChooseType)
        {
            Vector3 checkPos = ScreenPosToWorldPos2d(Input.mousePosition);
            AstarManager.Instance.UpdateAllNodeType(checkPos,curChooseNodeType);
        }
        if (isCheckResetType)
        {
            Vector3 checkPos = ScreenPosToWorldPos2d(Input.mousePosition);
            AstarManager.Instance.UpdateAllNodeType(checkPos,NodeType.Normal);
        }
        if (Input.GetMouseButtonDown(0))
        {
            isCheckChooseType = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isCheckChooseType = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            isCheckResetType = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isCheckResetType = false;
        }
    }

    Vector3 ScreenPosToWorldPos2d(Vector3 screenPos)
    {
        Vector3 worldPos = AstarManager.Instance.MainCam.ScreenToWorldPoint(screenPos);
        return new Vector3(worldPos.x,worldPos.y,0);
    }
}
