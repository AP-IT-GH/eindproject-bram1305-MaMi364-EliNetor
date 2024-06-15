using UnityEngine;
using TMPro;

public class Placement : MonoBehaviour
{
    // Public variables to assign the five objects and the TextMeshPro component in the Inspector
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;
    public GameObject object4;
    public GameObject object5;
    public TextMeshProUGUI positionText;

    void Update()
    {
        // Check if all objects are assigned
        if (object1 != null && object2 != null && object3 != null && object4 != null && object5 != null)
        {
            // Calculate the distances from the current object to each of the five objects
            float distance1 = Vector3.Distance(transform.position, object1.transform.position);
            float distance2 = Vector3.Distance(transform.position, object2.transform.position);
            float distance3 = Vector3.Distance(transform.position, object3.transform.position);
            float distance4 = Vector3.Distance(transform.position, object4.transform.position);
            float distance5 = Vector3.Distance(transform.position, object5.transform.position);

            // Determine the position of object1 relative to the others
            int position = 1; // Start with 1st position as default

            if (distance1 > distance2) position++;
            if (distance1 > distance3) position++;
            if (distance1 > distance4) position++;
            if (distance1 > distance5) position++;

            // Update the TextMeshPro component based on the position of object1
            switch (position)
            {
                case 1:
                    positionText.text = "1st";
                    break;
                case 2:
                    positionText.text = "2nd";
                    break;
                case 3:
                    positionText.text = "3rd";
                    break;
                case 4:
                    positionText.text = "4th";
                    break;
                case 5:
                    positionText.text = "5th";
                    break;
                default:
                    positionText.text = "Unknown";
                    break;
            }
        }
    }
}
