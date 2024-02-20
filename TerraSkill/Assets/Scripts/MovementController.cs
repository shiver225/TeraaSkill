using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readytoJump = true;

    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public float dashForce;
    public float dashCooldown;
    bool readyToDash = true;
    bool dashing;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public Transform orientation;
    public Transform playerCam;
    float horizontalInput;
    float verticalInput;
    Vector3 movedirection;
    Rigidbody rb;

    [Header("Settings")]
    public bool useCameraFowrawd = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    public MovementState state;
    public enum MovementState
    {
        dashing
    }


    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    private void StateHandler()
    {
        // Mode - dashing
        if(dashing) {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        bool desiredMoveSpeedChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(lastState == MovementState.dashing) keepMomentum = true;

        if(desiredMoveSpeedChanged) {
            if (keepMomentum) {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private float speedChangeFactor;

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while(time < difference) {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        //handle drag
        if(state != MovementState.dashing) {
            rb.drag = groundDrag;
        }
        else {
            rb.drag = 0;
        }
    }
    
    private void FixedUpdate() 
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //jump check
        // Debug.Log("Jump: " + readytoJump);
        // Debug.Log("Ground: " + grounded);
        if(Input.GetKey(jumpKey) && readytoJump && grounded) {
            readytoJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKey(dashKey) && readyToDash) {
            readyToDash = false;
            Dash();
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void MovePlayer()
    {
        movedirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on ground
        if(grounded) {
            rb.AddForce(movedirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        //in air
        else if(!grounded) {
            rb.AddForce(movedirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //velocity limit
        if(flatVel.magnitude > moveSpeed) {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
        }
    }

    private void Jump()
    {
        //velocity (y) reset
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readytoJump = true;
    }

    private void Dash()
    {
        dashing = true;

        Transform forwardT;

        if(useCameraFowrawd) {
            forwardT = playerCam;
        }
        else {
            forwardT = orientation;
        }

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce * 10f;
        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayDashForce), 0.025f);
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
        readyToDash = true;
        dashing = false;
    }
    
    private Vector3 GetDirection(Transform forwardT)
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if(allowAllDirections) {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else {
            direction = forwardT.forward;
        }

        if(verticalInput == 0) {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }
}
