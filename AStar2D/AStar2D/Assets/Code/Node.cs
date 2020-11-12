using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeType CurType;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SwitchType(NodeType nodeType)
    {
        // if (CurType == nodeType)
        // {
        //     return;
        // }
        switch (nodeType)
        {
            case NodeType.Normal:
                spriteRenderer.color = Color.white;
                break;
            case NodeType.Start:
                spriteRenderer.color = Color.red;
                break;
            case NodeType.End:
                spriteRenderer.color = Color.green;
                break;
            case NodeType.Obstacle:
                spriteRenderer.color = Color.black;
                break;
            case NodeType.Path:
                spriteRenderer.color = Color.yellow;
                break;
        }

        CurType = nodeType;
    }

    public void SetToPathNode()
    {
        SwitchType(NodeType.Path);
    }

    public void OnUpdateType(Vector3 mousePos, NodeType nodeType)
    {
        if (CurType == nodeType)
        {
            return;
        }
        if (Vector3.Distance(mousePos, transform.position) < 0.5f)
        {
            SwitchType(nodeType);
        }
    }

    public void OnReset()
    {
        SwitchType(NodeType.Normal);
    }
}
