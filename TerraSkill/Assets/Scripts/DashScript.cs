using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private MovementController mc;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashCooldown;
    private float dashCooldownTimer;
    public float dashDuration;
    public float maxDashYSpeed;

    [Header("Settings")]
    public bool useCameraFowrawd = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mc = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(mc.dashKey)) {
            Dash();
        }

        if(dashCooldownTimer > 0) {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        if(dashCooldownTimer > 0) return;
        else dashCooldownTimer = dashCooldown;


        mc.dashing = true;
        mc.maxYSpeed = maxDashYSpeed;

        Transform forwardT;

        if(useCameraFowrawd) {
            forwardT = playerCam;
        }
        else {
            forwardT = orientation;
        }

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce * 10f + orientation.up * dashUpwardForce;
        
        if(disableGravity) {
            rb.useGravity = false;
        }

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    Vector3 delayedForceToApply;
    private void DelayDashForce() 
    {
        if(resetVel) {
            rb.velocity = Vector3.zero;
        }
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        mc.dashing = false;
        mc.maxYSpeed = 0;

        if(disableGravity) {
            rb.useGravity = true;
        }
    }
    
    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if(allowAllDirections) {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else {
            direction = forwardT.forward;
        }

        if(verticalInput == 0 && horizontalInput == 0) {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }
}
