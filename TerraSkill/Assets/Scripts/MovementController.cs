using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;

public class MovementController : MonoBehaviour
{

     [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float dashSpeed;
    public float dashSpeedChangeFactor;

    public float maxYSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    private bool isCrouching = false;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public float minSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.E;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Other")]
    public bool dashing;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    public Transform orientation;
    public Transform playerCam;
    float horizontalInput;
    float verticalInput;
    Vector3 movedirection;
    Rigidbody rb;
    public Transform charTrans;
    public float timer;

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
        if(isCrouching) {
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

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
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
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
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
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        Display();
        AnimationHandler();

        //handle drag
        if(state != MovementState.dashing && state != MovementState.air && grounded) {
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
            playerAnim.ResetTrigger("Attack");
            isJumping = true;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //attack check
        if (Input.GetMouseButtonDown(0) && (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching || state == MovementState.air))
        {
            playerAnim.SetTrigger("Attack");
        }

        timer += Time.deltaTime;
        // TODO: fix crouch timer (crouch spam)
        // crouching
        if (timer >= 0.3 ) {
            // start crouch
            if(Input.GetKeyDown(crouchKey)) {
                isCrouching = true;
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                charTrans.localScale = new Vector3(charTrans.localScale.x, startYScale * 2f, charTrans.localScale.z); 
                rb.AddForce(Vector3.down * 100f, ForceMode.Impulse);
            }

            //stop crouch
            if(Input.GetKeyUp(crouchKey)) {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                charTrans.localScale = new Vector3(charTrans.localScale.x, startYScale, charTrans.localScale.z);
                isCrouching = false;
                timer = 0;
                timer += Time.deltaTime;
            }
        }
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing) return;

        movedirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        isMoving = movedirection.magnitude > 0;

        //on slope
        if(OnSlope() && !exitingSlope) {
            Debug.Log("On slope!");
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

         // on ground
        else if(grounded)
            rb.AddForce(movedirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(movedirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //limit speed on slope
        if(OnSlope() && !exitingSlope) {
            if(rb.velocity.magnitude > moveSpeed) {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        
        // limit speed on ground and air
        else {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //velocity limit
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

            if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed) {
                rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
            }
        }

        
    }

    private void Jump()
    {
        exitingSlope = true;

        //velocity (y) reset
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        isJumping = false;
        exitingSlope = false;
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
            playerAnim.ResetTrigger("Attack");
        }

        //sprint animation
        else if(isMoving && grounded && state == MovementState.sprinting && !isJumping) {
            playerAnim.SetTrigger("Sprint");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Jump");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Crouch");
            playerAnim.ResetTrigger("Crouch_walking");
            playerAnim.ResetTrigger("Attack");
        }

        //crouch animation
        else if(state == MovementState.crouching && !isMoving) {
            playerAnim.SetTrigger("Crouch");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Sprint");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Jump");
            playerAnim.ResetTrigger("Crouch_walking");
            playerAnim.ResetTrigger("Attack");
        }

        //crouch movement animations
        else if(state == MovementState.crouching && isMoving) {
            playerAnim.SetTrigger("Crouch_walking");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Sprint");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Jump");
            playerAnim.ResetTrigger("Crouch");
            playerAnim.ResetTrigger("Attack");
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

        //TODO: Set up running + attack animation
        //TODO: Set up crouching + attack animation
        //TODO: Set up jumping + attack animation
    }
    
    private void Display() 
    {   if(stateText != null)
            stateText.SetText("State: " + state.ToString());
        if (speedText != null)
            speedText.SetText("Speed: " + desiredMoveSpeed.ToString());
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle >= minSlopeAngle && angle <= maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(movedirection, slopeHit.normal).normalized;
    }

    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_ySpeed;
    public TextMeshProUGUI text_mode;
    private void TextStuff()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (OnSlope())
            text_speed.SetText("Speed: " + Round(rb.velocity.magnitude, 1) + " / " + Round(moveSpeed, 1));

        else
            text_speed.SetText("Speed: " + Round(flatVel.magnitude, 1) + " / " + Round(moveSpeed, 1));

        //float yVel = rb.velocity.y;
        //float yMax = maxYSpeed == 0 ? 0 : maxYSpeed;
        //text_ySpeed.SetText("YSpeed: " + Round(yVel, 0) + " / " + yMax);

        text_mode.SetText(state.ToString());
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

}
