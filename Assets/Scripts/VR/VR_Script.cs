using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using UnityEngine.XR.Interaction.Toolkit; // Required for controls
using UnityEngine.InputSystem; // Required for controls
using static UnityEngine.GraphicsBuffer;

public class CubeAgentRays : Agent
{
    public Animator anim;  // Animation controller
    public float minJumpForce = 5f; // Minimum jump force
    public float maxJumpForce = 10f; // Maximum jump force
    public float chargeRate = 2f; // Rate at which jump force charges
    public LayerMask platformLayer;
    private Vector3 startingPosition;
    private Vector3 beginPos;
    private Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    private bool isChargingJump = false; // Flag to track if jump is being charged
    private float currentJumpForce = 0f; // Current jump force
    private float forwardForce;
    private bool points = true;
    public Transform dir;

    public ActionBasedController controller; // Reference to the Action-based XR Controller
    public InputActionReference primaryReference; // Pick reference action in menu
    public InputActionReference secondaryReference; // Pick reference action in menu

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
            }
        }

        // Add the distance to the nearest jump point as an observation
        sensor.AddObservation(nearestDistance);
        Debug.Log(nearestDistance + " Distance");
        sensor.AddObservation(currentJumpForce);
        sensor.AddObservation(forwardForce);
        sensor.AddObservation(maxJumpForce);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float jumpAction = actions.ContinuousActions[0];
        float rotateAction = actions.ContinuousActions[1];

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
            AddReward(-1f);
            EndEpisode();
        }

        if (rotateAction > 0f && !hasRotated && IsGrounded())
        {
            StartCoroutine(RotateCube());
            hasRotated = true;
        }
        else if (rotateAction == 0f)
        {
            hasRotated = false;
        }
    }

    private bool hasRotated = false;

    private IEnumerator RotateCube()
    {
        anim.SetBool("turn", true);

        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("turn"))
        {
            yield return null;
        }

        while (anim.GetCurrentAnimatorStateInfo(0).IsName("turn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        Vector3 newRotation = transform.eulerAngles;
        newRotation.y += 180f;
        transform.eulerAngles = newRotation;
        anim.SetBool("turn", false);

    }


    private void OnCollisionStay(Collision collision)
    {
        if (IsGrounded() && collision.gameObject.CompareTag("Platform") && points)
        {
            Debug.Log("GOT POINTS");
            AddReward(1f);
            points = false;
        }
        else if (IsGrounded() && collision.gameObject.CompareTag("Checkpoint") && points)
        {
            Debug.Log("GOT POINTS");
            AddReward(1f);
            startingPosition = transform.position;
            points = false;
        }
        else if (IsGrounded() && collision.gameObject.CompareTag("Einde"))
        {
            Debug.Log("FINISHED!");
            AddReward(5f);
            startingPosition = beginPos;
            EndEpisode();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            points = true;
        }
    }

    private void StartChargingJump()
    {
        isChargingJump = true;
        currentJumpForce = minJumpForce;
        anim.SetBool("crouch", true);
    }

    private void ContinueChargingJump()
    {
        // Increase jump force as long as the jump button is held
        currentJumpForce = Mathf.Min(currentJumpForce + Time.fixedDeltaTime * chargeRate, maxJumpForce);
    }

    private void EndChargingJump()
    {
        anim.SetBool("crouch", false);
        anim.SetBool("jump", true);
        isChargingJump = false;
        Jump(dir);
    }

    private void Jump(Transform target)
    {
        if (IsGrounded())
        {
            AddReward(0.2f);

            // Calculate jump direction towards the target
            Vector3 jumpDirection = (target.position - transform.position).normalized;

            // Calculate forward force based on jump direction
            forwardForce = currentJumpForce / 2;
            Vector3 forwardJumpDirection = target.forward * forwardForce;

            // Apply upward force and forward jump direction
            Vector3 upwardForce = Vector3.up * currentJumpForce;
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

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;

        // Reset continuous actions
        actions[0] = 0f;
        actions[1] = 0f;

        // Jump control
        float isPrimaryPressed = primaryReference.action.ReadValue<float>(); //reads value of primary reference
        if (isPrimaryPressed >0 )
        {
            actions[0] = 1f;
            Debug.Log("Pri:True");
        }

        // Rotate control
        float isSecondaryPressed = secondaryReference.action.ReadValue<float>(); //reads value of primary reference
        if (isSecondaryPressed > 0)
        {
            actions[1] = 1f;
            Debug.Log("Sec:True");
        }
        
    }
}