using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionsPowerup : MonoBehaviour
{
    public AgentMovement agentScript;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (agentScript != null)
            {
                agentScript.ResetPosition();
            }
            
            Destroy(gameObject);
        }
    }
}
