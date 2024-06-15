using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class EndGame : MonoBehaviour
{
    public GameObject agent;
    public GameObject player;
    public TextMeshProUGUI endText;
    public GameObject EndingScreen;
    private Rigidbody rbPlayer;
    private Rigidbody rbAgent;
    private bool end = false;
    
    private void Update()
    {
        if (end)
        {
            rbPlayer.velocity = Vector3.zero;
            rbPlayer.useGravity = false;
            rbAgent.velocity = Vector3.zero;
            rbAgent.useGravity = false;
        }
    }
    private void Start()
    {
        rbPlayer = player.GetComponent<Rigidbody>();
        rbAgent = player.GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == agent && rbAgent.velocity.y == 0)
        {
            endText.text = "You lose";
            EndingScreenEnable();
        }
        else if (collision.gameObject == player && rbPlayer.velocity.y == 0) 
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
        EndingScreen.transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z + 20f);
    }
}
