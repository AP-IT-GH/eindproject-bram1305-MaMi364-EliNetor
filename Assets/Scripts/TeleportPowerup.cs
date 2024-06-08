using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPowerup : MonoBehaviour
{
    // Public variables to set the teleport destination in the Unity Inspector
    public Vector3 teleportDestination;

    // This method is called when another collider enters the trigger collider attached to the object where this script is attached
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Teleport the player to the specified destination
            other.transform.position = teleportDestination;
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.angularVelocity = Vector3.zero; // Also stops any rotational movement
            }
            Destroy(gameObject);
        }
    }
}
