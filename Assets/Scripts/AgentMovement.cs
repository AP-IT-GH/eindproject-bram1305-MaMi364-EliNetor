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
    public LayerMask platformLayer;
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
        currentJumpForce = 0f; 
    }


    public override void CollectObservations(VectorSensor sensor)
    {

        GameObject[] jumpPoints = GameObject.FindGameObjectsWithTag("JumpPoints");

        // Find the nearest jump point
        float nearestDistance = Mathf.Infinity;
        foreach (GameObject jumpPoint in jumpPoints)
        {
            float distance = Vector3.Distance(transform.position, jumpPoint.transform.position);
            if (distance > 2f)
            {
                nearestDistance = Mathf.Min(nearestDistance, distance); ;
                Debug.Log(nearestDistance + " Distance F");
            }
        }

        // Add the distance to the nearest jump point as an observation
        sensor.AddObservation(nearestDistance);
        sensor.AddObservation(currentJumpForce);
        sensor.AddObservation(forwardForce);
        sensor.AddObservation(maxJumpForce);

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
            AddReward(5f);
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
            AddReward(0.2f);
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