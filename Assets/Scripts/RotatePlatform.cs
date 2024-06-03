using UnityEngine;
using System.Collections;

public class RotatePlatform : MonoBehaviour
{
    public float rotationSpeed = 360f; // Degrees per second
    public float pauseTime = 6f; // Pause duration in seconds

    void Start()
    {
        StartCoroutine(RotateAndPause());
    }

    IEnumerator RotateAndPause()
    {
        while (true)
        {
            // Start rotating
            float angleRotated = 0f;
            while (angleRotated < 360f)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(0, 0, rotationStep);
                angleRotated += rotationStep;
                yield return null;
            }

            // Correct any overshoot in rotation
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.eulerAngles.z / 360f) * 360f);

            // Pause for a specified duration
            yield return new WaitForSeconds(pauseTime);
        }
    }
}
