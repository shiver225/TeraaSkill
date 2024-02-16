using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Rigidbody rb;
    public float moveVelocity = 0.2f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f){
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveVelocity * Time.deltaTime);
        }
        
        // if(Input.GetKey(KeyCode.A)) {
        //     transform.position += Vector3.left * moveVelocity * Time.deltaTime;
        // }
        // if(Input.GetKey(KeyCode.W)){
        //     transform.position += Vector3.forward * moveVelocity * Time.deltaTime;
        // }
        // if(Input.GetKey(KeyCode.D)){
        //     transform.position += Vector3.right * moveVelocity * Time.deltaTime;
        // }
        // if(Input.GetKey(KeyCode.S)){
        //     transform.position += Vector3.back * moveVelocity * Time.deltaTime;
        // }
    }
}
