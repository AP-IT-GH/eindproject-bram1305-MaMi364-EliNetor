using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRotation : MonoBehaviour
{
    public float speed = 10.0f;
    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            float h = speed * Input.GetAxis("Mouse X");
            transform.Rotate(0, h, 0);
            lastMousePosition = Input.mousePosition;
        }
        else if (lastMousePosition == Input.mousePosition)
        {
            // No mouse movement, stop rotation
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
    }
}
