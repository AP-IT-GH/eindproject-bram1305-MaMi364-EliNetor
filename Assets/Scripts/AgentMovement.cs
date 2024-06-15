using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using UnityEngine.UIElements;

public class AgentMovement : Agent
{
    public Animator anim;  // Animation controller
    public float minJumpForce = 8f; // Minimum jump force
    public float maxJumpForce = 16f; // Maximum jump force
    public float chargeRate = 4f; // Rate at which jump force charges
    public LayerMask platformLayer;
    private Vector3 startingPosition;
    private Vector3 beginPos;
    private Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    private bool isChargingJump = false; // Flag to track if jump is being charged
    private float currentJumpForce = 0f; // Current jump force
    private float forwardForce;
    private bool points = true;
    private float platformUnderAgentXPosition;
    private float nearestPlatformXPosition;
    private string checkpointName;
    private string previousCheckpoint;
    private const float stuckThreshold = 60f; 
    private float stuckTimer = 0f;
    private const float StandingStillThreshold = 10f;
    private float StandingStillTimer = 0f;
    private float chargingTime = 0f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position; // Begin positie als variabele nemen om later te resetten
        beginPos = transform.position;
    }
    public override void OnEpisodeBegin()
    {
        points = true;
        transform.position = startingPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentJumpForce = 0f;
        anim.SetBool("jump", false);
        stuckTimer = 0f;
        StandingStillTimer = 0f;
        chargingTime = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        GameObject[] jumpPoints = GameObject.FindGameObjectsWithTag("JumpPoints");
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        GameObject[] movingPlatforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
        List<GameObject> allPlatforms = new List<GameObject>(platforms);
        allPlatforms.AddRange(movingPlatforms);

        // Find the nearest jump point
        float nearestJumpPointDistance = 100000f;
        foreach (GameObject jumpPoint in jumpPoints)
        {
            float distance = Vector3.Distance(transform.position, jumpPoint.transform.position);
            if (jumpPoint.transform.position.z > transform.position.z)
            {
                if (distance > 3.5f)
                {
                    if (distance < nearestJumpPointDistance)
                    {
                        nearestJumpPointDistance = distance;
                    }
                }
            }
        }

        RaycastHit hit;
        platformUnderAgentXPosition = 0f;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                platformUnderAgentXPosition = hit.collider.transform.position.x;
            }
        }

        // Find the nearest moving platform
        float nearestPlatformDistance = Mathf.Infinity;
        nearestPlatformXPosition = 0f;
        foreach (GameObject platform in allPlatforms)
        {
            float distance = Vector3.Distance(transform.position, platform.transform.position);
            if (distance < nearestPlatformDistance)
            {
                if (distance > 9.5f)
                {
                    nearestPlatformDistance = distance;
                    nearestPlatformXPosition = platform.transform.position.x;
                }
            }
        }

        if(!isChargingJump && IsGrounded())
        {
            chargingTime += Time.deltaTime;
        }
        else
        {
            chargingTime = 0;
        }

        // Add observations
        sensor.AddObservation(nearestJumpPointDistance);
        //Debug.Log("DISTANCE: " + nearestJumpPointDistance);

        sensor.AddObservation(currentJumpForce);
        sensor.AddObservation(forwardForce);
        sensor.AddObservation(maxJumpForce);

        sensor.AddObservation(chargingTime);

        sensor.AddObservation(platformUnderAgentXPosition);
        //Debug.Log(platformUnderAgentXPosition + " X position of platform under agent");
        sensor.AddObservation(nearestPlatformXPosition);
        //Debug.Log(nearestPlatformXPosition + " X position of nearest moving platform");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float jumpAction = actions.ContinuousActions[0];

        if (IsGrounded())
        {
            anim.SetBool("jump", false);
        }
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
            AddReward(-2f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsGrounded() && points)
        {
            if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("MovingPlatform"))
            {
                Debug.Log("GOT POINTS");
                AddReward(1f);
                points = false;
            }
            else if (collision.gameObject.CompareTag("Checkpoint"))
            {
                checkpointName = collision.gameObject.name;

                if (checkpointName != previousCheckpoint)
                {
                    Debug.Log("GOT POINTS Checkpoint");
                    AddReward(1f);
                    previousCheckpoint = collision.gameObject.name;
                }
                try
                {
                    Transform respawnPoint = collision.gameObject.transform.Find("RespawnPointAgent");
                    Vector3 currentPosition = transform.position;
                    startingPosition = new Vector3(currentPosition.x, respawnPoint.position.y, respawnPoint.position.z);
                    points = false;
                }
                catch (NullReferenceException ex)
                {
                    Debug.Log("Not Set ");
                }

            }
            else if (collision.gameObject.CompareTag("Speler"))
            {
                EndEpisode();
            }
        }

        if (IsGrounded()) 
        {
            if (collision.gameObject.CompareTag("Einde"))
            {
                Debug.Log("FINISHED!");
                AddReward(5f);
                startingPosition = beginPos;
                EndEpisode();
            }
        }
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                Debug.Log("Stuck in MovingPlatform");
                stuckTimer = 0f; // Reset timer 
                EndEpisode();
            }
        }  else
        {
            stuckTimer = 0f;
        }
        if (collision.gameObject.CompareTag("Platform"))
        {
            StandingStillTimer += Time.deltaTime;
            if (StandingStillTimer >= StandingStillThreshold)
            {
                Debug.Log("Standing Still");
                StandingStillTimer = 0f; // Reset timer 
                AddReward(-0.7f);
            }
        }
        else
        {
            StandingStillTimer = 0f;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Checkpoint") || collision.gameObject.CompareTag("MovingPlatform"))
        {
            points = true;
        }
    }

    private void StartChargingJump()
    {
        if (IsGrounded())
        {
            isChargingJump = true;
            currentJumpForce = minJumpForce;
            anim.SetBool("crouch", true);
            AddReward(0.01f);
        }
    }

    private void ContinueChargingJump()
    {
        // Increase jump force as long as the jump button is held
        currentJumpForce = Mathf.Min(currentJumpForce + Time.fixedDeltaTime * chargeRate, maxJumpForce);
        if (currentJumpForce >= maxJumpForce)
        {
            EndChargingJump();
        }
    }

    private void EndChargingJump()
    {
        anim.SetBool("crouch", false);
        anim.SetBool("jump", true);
        isChargingJump = false;
        Jump();

    }

    private void Jump()
    {
        if (IsGrounded())
        {
            forwardForce = currentJumpForce / 2;
            Vector3 upwardForce = Vector3.up * currentJumpForce;
            Vector3 forwardJumpDirection = transform.forward * forwardForce;

            rb.AddForce(upwardForce + forwardJumpDirection, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position + Vector3.down, 0.22f, platformLayer))
        {
            // changing the collider center Y position to adjust to the actual animations
            Vector3 newCenter = new Vector3(capsuleCollider.center.x, -0.35f, capsuleCollider.center.z);
            capsuleCollider.center = newCenter;
            return true;
        }
        else
        {
            Vector3 newCenter = new Vector3(capsuleCollider.center.x, 0.4f, capsuleCollider.center.z);
            capsuleCollider.center = newCenter;
            return false;
        }
    }

    public void ResetPosition()
    {
        transform.position = startingPosition;
        rb.velocity = Vector3.zero;
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
