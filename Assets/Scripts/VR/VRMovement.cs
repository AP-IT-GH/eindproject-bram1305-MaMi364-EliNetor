using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class VRMovement : MonoBehaviour
{
    public float minJumpForce = 5f;   // Minimum jump force
    public float maxJumpForce = 10f;  // Maximum jump force
    public float chargeRate = 2f;     // Rate at which jump force charges
    public LayerMask platformLayer;   // Layer mask to identify platforms

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool isChargingJump = false;
    private float currentJumpForce = 0f;
    private bool isGrounded = true;

    public InputActionReference left_button;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        CheckGroundedStatus();

        bool isJumpButtonPressed = left_button.action.ReadValue<bool>();

        if (isJumpButtonPressed)
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
    }

    private void CheckGroundedStatus()
    {
        if (Physics.CheckSphere(transform.position + Vector3.down * (capsuleCollider.height / 2 + 0.1f), 0.22f, platformLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void StartChargingJump()
    {
        isChargingJump = true;
        currentJumpForce = minJumpForce;
    }

    private void ContinueChargingJump()
    {
        currentJumpForce = Mathf.Min(currentJumpForce + Time.deltaTime * chargeRate, maxJumpForce);
    }

    private void EndChargingJump()
    {
        isChargingJump = false;
        Jump();
    }

    private void Jump()
    {
        if (isGrounded)
        {
            Vector3 upwardForce = Vector3.up * currentJumpForce;
            rb.AddForce(upwardForce, ForceMode.Impulse);
        }
    }
}

