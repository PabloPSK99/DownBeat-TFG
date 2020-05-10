using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAt : MonoBehaviour
{
    public Transform mainCamera;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(-1 * mainCamera.transform.position);
    }
}
