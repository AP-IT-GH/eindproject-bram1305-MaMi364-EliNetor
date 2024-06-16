using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public GameObject[] agent;
    public GameObject player;
    private Rigidbody rbPlayer;
    private Rigidbody[] rbAgent;

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

    private void OnCollisionEnter(Collision collision)
    {
        // Check collisions with each agent and the player
        for (int i = 0; i < agent.Length; i++)
        {
            if (collision.gameObject == agent[i] && rbAgent[i].velocity.y == 0)
            {
                LoadScene(3);
                return; // Exit early once we find a matching agent
            }
        }

        if (collision.gameObject == player && rbPlayer.velocity.y == 0)
        {
            LoadScene(2);
        }
    }

    private void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
    }
}
