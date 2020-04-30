using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node forward;
    public Node left;
    public Node right;
    public Node back;

    public Node GetLastNodeOnThisAxis()
    {
        if (back == null) return this;
        return back.GetLastNodeOnThisAxis();
    }

    public Node GetFirstNodeOnThisAxis()
    {
        if (forward == null) return this;
        return forward.GetFirstNodeOnThisAxis();
    }

    public Node GetFirstNodeOnOppositeAxis()
    {
        return left.left.left.GetFirstNodeOnThisAxis();
    }
}
