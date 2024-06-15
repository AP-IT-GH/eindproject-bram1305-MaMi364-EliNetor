using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target; // The character's transform
    private Vector3 offset; // The initial offset between camera and character

    void Start()
    {
        if (target != null)
        {
            // Calculate the initial offset between camera and character
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the position behind the camera
            Vector3 targetPosition = transform.position - transform.forward * offset.magnitude;

            // Set the character's position along the horizontal plane
            targetPosition.y = target.position.y;

            // Set the character's position
            target.position = targetPosition;

            // Rotate the character to face the same direction as the camera
            target.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
    }
}
