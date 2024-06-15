using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class EndGame : MonoBehaviour
{
    public GameObject[] agent;
    public GameObject player;
    public TextMeshProUGUI endText;
    public GameObject EndingScreen;
    private Rigidbody rbPlayer;
    private Rigidbody[] rbAgent;
    private bool end = false;

    private void Start()
    {
        // Initialize the rbAgent array with the correct size
        rbAgent = new Rigidbody[agent.Length];

        // Assign Rigidbody components to each agent's Rigidbody in the rbAgent array
        for (int i = 0; i < agent.Length; i++)
        {
            rbAgent[i] = agent[i].GetComponent<Rigidbody>();
        }

        // Assign Rigidbody component to the player's Rigidbody
        rbPlayer = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (end)
        {
            // Stop player's and agents' movement and gravity
            rbPlayer.velocity = Vector3.zero;
            rbPlayer.useGravity = false;

            foreach (Rigidbody agentRB in rbAgent)
            {
                agentRB.velocity = Vector3.zero;
                agentRB.useGravity = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check collisions with each agent and the player
        for (int i = 0; i < agent.Length; i++)
        {
            if (collision.gameObject == agent[i] && rbAgent[i].velocity.y == 0)
            {
                endText.text = "You lose";
                EndingScreenEnable();
                return; // Exit early once we find a matching agent
            }
        }

        if (collision.gameObject == player && rbPlayer.velocity.y == 0)
        {
            endText.text = "You win";
            EndingScreenEnable();
        }
    }

    private void EndingScreenEnable()
    {
        end = true;
        EndingScreen.SetActive(true);
        Vector3 playerPosition = player.transform.position;
        EndingScreen.transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z + 5f);
    }
}
