using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SideFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 Offset;
    public float smoothSpeed;

    void LateUpdate()
    {
        Vector3 desiredPosition = player.transform.position + Offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    
}

