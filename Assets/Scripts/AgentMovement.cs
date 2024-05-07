using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class CubeAgentRays : Agent
{
    public float minJumpForce = 5f; // Minimum jump force
    public float maxJumpForce = 10f; // Maximum jump force
    public float chargeRate = 2f; // Rate at which jump force charges

    private Vector3 startingPosition;
    private Rigidbody rb;
    private bool isChargingJump = false; // Flag to track if jump is being charged
    private float currentJumpForce = 0f; // Current jump force
    private float forwardForce;
    private bool points = true;
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }
    public override void OnEpisodeBegin()
    {
        points = true;
        transform.position = startingPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentJumpForce = 0f; // Reset charge level at the beginning of each episode
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        float minDistance = Mathf.Infinity;
        float distanceToEndPlatform = Mathf.Infinity;
        GameObject endPlatform = GameObject.FindGameObjectWithTag("Einde");

        // Iterate through all platforms to find the nearest one
        foreach (GameObject plat in GameObject.FindGameObjectsWithTag("Platform"))
        {
            float distance = Vector3.Distance(transform.position, plat.transform.position);
            minDistance = Mathf.Min(minDistance, distance);
        }

        distanceToEndPlatform = Vector3.Distance(transform.position, endPlatform.transform.position);

        sensor.AddObservation(minDistance);
        sensor.AddObservation(currentJumpForce);
        sensor.AddObservation(distanceToEndPlatform);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float jumpAction = actions.ContinuousActions[0];

        if (jumpAction > 0f)
        {
            if (!isChargingJump)
            {
                StartChargingJump();
            }
            else
            {
                ContinueChargingJump();
            }
        }
        else if (isChargingJump)
        {
            EndChargingJump();
        }   
        
        if (this.transform.localPosition.y < 0f)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(IsGrounded() && collision.gameObject.CompareTag("Platform") && points)
        {
            Debug.Log("GOT POINTS");
            AddReward(1f);
            points = false;
        }
        else if (IsGrounded() && collision.gameObject.CompareTag("Einde"))
        {
            Debug.Log("FINISHED!");
            AddReward(2f);
            EndEpisode();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            points = true;
        }
    }

    private void StartChargingJump()
    {
        isChargingJump = true;
        currentJumpForce = minJumpForce;
    }

    private void ContinueChargingJump()
    {
        // Increase jump force as long as the jump button is held
        currentJumpForce = Mathf.Min(currentJumpForce + Time.fixedDeltaTime * chargeRate, maxJumpForce);
    }

    private void EndChargingJump()
    {
        isChargingJump = false;
        Jump();
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            AddReward(0.1f);
            forwardForce = currentJumpForce / 2;

            Vector3 forwardJumpDirection = transform.forward * forwardForce;

            // Add an upward force to the rigidbody to simulate jumping
            rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);

            // Add a forward force to the rigidbody for the forward jump
            rb.AddForce(forwardJumpDirection, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        float raycastDistance = 0.8f;
        return Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;

        // Reset continuous actions
        actions[0] = 0f;

        // Jump control
        if (Input.GetKey(KeyCode.Space))
        {
            actions[0] = 1f;
        }
    }
}