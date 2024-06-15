using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Placement : MonoBehaviour
{
    // Public variables to assign the two objects and the TextMeshPro component in the Inspector
    public GameObject object1;
    public GameObject object2;
    public TextMeshProUGUI positionText;

    void Update()
    {
        // Check if the objects are assigned
        if (object1 != null && object2 != null)
        {
            // Calculate the distances from the finish line
            float distance1 = Vector3.Distance(transform.position, object1.transform.position);
            float distance2 = Vector3.Distance(transform.position, object2.transform.position);

            // Update the TextMeshPro component based on which object is closer
            if (distance1 < distance2)
            {
                positionText.text = "1st";
            }
            else
            {
                positionText.text = "2nd";
            }
        }
    }
}