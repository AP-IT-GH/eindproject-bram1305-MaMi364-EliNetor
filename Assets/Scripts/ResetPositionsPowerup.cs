using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionsPowerup : MonoBehaviour
{
    public CubeAgentRays playerScript;
    public AgentMovement agentScript;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerScript != null)
            {
                playerScript.ResetPosition();
            }
            if (agentScript != null)
            {
                agentScript.ResetPosition();
            }
            
            Destroy(gameObject);
        }
    }
}
