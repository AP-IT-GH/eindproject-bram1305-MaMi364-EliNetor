using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappearingwall : MonoBehaviour
{
    // Duration for which the cube will disappear
    public float disappearDuration = 2.0f;
    // Duration for which the cube will reappear
    public float reappearDuration = 2.0f;

    // Reference to the Renderer component
    private Renderer cubeRenderer;
    // Reference to the Collider component
    private Collider cubeCollider;

    private void Start()
    {
        // Get the Renderer and Collider components
        cubeRenderer = GetComponent<Renderer>();
        cubeCollider = GetComponent<Collider>();

        // Start the coroutine to handle disappearance and reappearance
        StartCoroutine(DisappearReappearCycle());
    }

    private IEnumerator DisappearReappearCycle()
    {
        while (true)
        {
            // Disappear the cube
            SetCubeActive(false);
            // Wait for the specified disappear duration
            yield return new WaitForSeconds(disappearDuration);

            // Reappear the cube
            SetCubeActive(true);
            // Wait for the specified reappear duration
            yield return new WaitForSeconds(reappearDuration);
        }
    }

    // Method to enable or disable the cube
    private void SetCubeActive(bool isActive)
    {
        // Enable or disable the Renderer component
        cubeRenderer.enabled = isActive;
        // Enable or disable the Collider component
        cubeCollider.enabled = isActive;
    }
}
