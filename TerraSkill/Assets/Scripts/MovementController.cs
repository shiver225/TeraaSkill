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
    public float maxYSpeed;

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

    [Header("Animator")]
    public Animator playerAnim;
    public Transform playerTrans;
    public bool isMoving;
    public bool isJumping;

    [Header("States")]
    public MovementState state;
    public enum MovementState
    {
        dashing
    }
    public bool dashing;

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
            playerAnim.SetTrigger("Jump");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Sprint");
            Jump();
            isJumping = true;
            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }

    private void MovePlayer()
    {
        //if (state == MovementState.dashing) return;

        movedirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        isMoving = movedirection.magnitude > 0;

        //on ground
        if(grounded) {
            rb.AddForce(movedirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        //in air
        else if(!grounded) {
            rb.AddForce(movedirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        if(isMoving && grounded && !isJumping) {
            playerAnim.SetTrigger("Sprint");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Jump");
        }
        else if(!isMoving && !isJumping) {
            playerAnim.ResetTrigger("Sprint");
            playerAnim.ResetTrigger("Jump");
            playerAnim.SetTrigger("Idle");
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

        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed) {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
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
        isJumping = false;
    }

}
