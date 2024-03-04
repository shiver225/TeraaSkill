using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;

public class MovementController : MonoBehaviour
{

    [field: Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float timer;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Walking")]
    public float walkSpeed;

    [Header("Sprinting")]
    public float sprintSpeed;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Dashing")]
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public float maxYSpeed;
    public bool dashing;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    [Header("Slope handling")]

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.E;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    public Transform orientation;
    public Transform playerCam;
    float horizontalInput;
    float verticalInput;
    Vector3 movedirection;
    Rigidbody rb;
    public Transform charTrans;

    [Header("Animator")]
    public Animator playerAnim;
    public Transform playerTrans;
    public bool isMoving;
    public bool isJumping;

    [Header("States")]
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        dashing
    }
    
    private void StateHandler()
    {
        // Mode - crouching
        if(Input.GetKey(crouchKey)) {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey)) {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Mode - walking
        else if(grounded) {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - dashing
        else if(dashing) {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        // Mode - air
        else {
            state = MovementState.air;

            if(desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
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

    [Header("Display")]
    public TMP_Text stateText;
    public TMP_Text speedText;

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

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        Display();
        AnimationHandler();

        //handle drag
        if(state != MovementState.dashing && state != MovementState.air) {
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
        if(Input.GetKey(jumpKey) && readyToJump && grounded) {
            readyToJump = false;
            playerAnim.SetTrigger("Jump");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Sprint");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Crouch");
            playerAnim.ResetTrigger("Crouch_walking");
            isJumping = true;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        timer =+ Time.deltaTime;

        // crouching
        if(timer >= 0.9) {
            // start crouch
            if(Input.GetKeyDown(crouchKey)) {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                charTrans.localScale = new Vector3(charTrans.localScale.x, startYScale * 2f, charTrans.localScale.z); 
                rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
            }

            //stop crouch
            if(Input.GetKeyUp(crouchKey)) {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                charTrans.localScale = new Vector3(charTrans.localScale.x, startYScale, charTrans.localScale.z);
                timer = 0;
            }
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
        readyToJump = true;
        isJumping = false;
    }

    private void AnimationHandler()
    {
        //walk animation
        if(isMoving && grounded && state == MovementState.walking && !isJumping)
        {
            playerAnim.SetTrigger("Run");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Jump");
            playerAnim.ResetTrigger("Sprint");
            playerAnim.ResetTrigger("Crouch");
            playerAnim.ResetTrigger("Crouch_walking");
        }

        //sprint animation
        else if(isMoving && grounded && state == MovementState.sprinting && !isJumping) {
            playerAnim.SetTrigger("Sprint");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Jump");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Crouch");
            playerAnim.ResetTrigger("Crouch_walking");
        }

        //crouch animation
        else if(state == MovementState.crouching && !isMoving) {
            playerAnim.SetTrigger("Crouch");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Sprint");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Jump");
            playerAnim.ResetTrigger("Crouch_walking");
        }

        //crouch movement animations
        else if(state == MovementState.crouching && isMoving) {
            playerAnim.SetTrigger("Crouch_walking");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Sprint");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Jump");
            playerAnim.ResetTrigger("Crouch");
        }

        //idle animation
        else if(!isMoving && !isJumping) {
            playerAnim.SetTrigger("Idle");
            playerAnim.ResetTrigger("Jump");
            playerAnim.ResetTrigger("Sprint");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Crouch");
            playerAnim.ResetTrigger("Crouch_walking");
        }
    }
    
    private void Display() 
    {
        stateText.SetText("State: " + state.ToString());
        speedText.SetText("Speed: " + desiredMoveSpeed.ToString());
    }

}
