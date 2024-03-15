using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Camera playerCamera;
    public Rigidbody rb;
    private CharacterController characterController;
    [Header("User interface")]
    public float lookSpeed = 2f;
    float defWalkSpeed = 14f;
    float defRunSpeed = 7f;
    float jumpPower = 8f;
    public float gravity = 25f;
    float lookXLimit = 80f;
    float defaultHeight = 2f;
    float crouchHeight = 1f;
    float crouchSpeed = 3f;
    float walkSpeed;
    float runSpeed;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float accumulatedRotation;
    private float curSpeedX;
    private float curSpeedY;
    float acceleration = 80f;
    float deceleration = 80f;
float maxSpeed = 10f;
    public bool canMove = true;
    Vector3 restPosition;
    float bobSpeed = 10f;
    float bobAmount = 0.15f;
    float defaultCameraY;
    Vector3 newPosition = new Vector3(0,0.7f,0);

    private float timer = Mathf.PI / 2;

    float rotationZ;
    bool canStartBobbing;
    float initialCameraY;

    void Start()
    {
        defaultCameraY = 0.7f;
        playerCamera.transform.localPosition = new Vector3(0, 0.7f, 0);
        walkSpeed = defWalkSpeed;
        runSpeed = defRunSpeed;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetSpeedX = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float targetSpeedY = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal");
        // Smoothly interpolate the current speed towards the target speed
        curSpeedX = Mathf.MoveTowards(curSpeedX, targetSpeedX, (isRunning ? acceleration : deceleration) * Time.deltaTime);
        curSpeedY = Mathf.MoveTowards(curSpeedY, targetSpeedY, (isRunning ? acceleration : deceleration) * Time.deltaTime);
        // Limit the speed to the maximum speed
        curSpeedX = Mathf.Clamp(curSpeedX, -maxSpeed, maxSpeed);
        curSpeedY = Mathf.Clamp(curSpeedY, -maxSpeed, maxSpeed);
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = defWalkSpeed;
            runSpeed = defRunSpeed;
        }
        characterController.Move(moveDirection * Time.deltaTime);
        /* ROTATION BY SPEED
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, rb.velocity.magnitude);
*/
/* ROTATION BY PRESSED A / D
float targetZRotation = Input.GetAxis("Horizontal") < 0 ? -maxTiltAngle : maxTiltAngle;
        float smoothedZRotation = Mathf.Lerp(playerCamera.transform.localRotation.eulerAngles.z, targetZRotation, tiltSmoothing);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, Input.GetAxis("Horizontal") < 0 ? -maxTiltAngle : maxTiltAngle);
*/
float smoothSpeed = 8.0f; // You can adjust this value to control the smoothness
if (!characterController.isGrounded) {
    // Smoothly interpolate the camera position
    playerCamera.transform.localPosition = Vector3.Lerp(
        playerCamera.transform.localPosition,
        new Vector3(
            playerCamera.transform.localPosition.x,
            Mathf.Clamp(newPosition.y + rb.velocity.y / 10, 0.2f, 1.2f),
            playerCamera.transform.localPosition.z
        ),
        smoothSpeed * Time.deltaTime * 3
    );
} else {
    // Gradually interpolate the camera position when grounded
    newPosition.y = 0.7f + Mathf.Abs((Mathf.Sin(timer) * bobAmount));
playerCamera.transform.localPosition = Vector3.Lerp(
    playerCamera.transform.localPosition,
    new Vector3(
        playerCamera.transform.localPosition.x,
        newPosition.y,
        playerCamera.transform.localPosition.z
    ),
    smoothSpeed * Time.deltaTime
);
}
        if (canMove)
        {

            float targetRotation = Input.GetAxis("Horizontal") * -100 * Time.deltaTime * lookSpeed; // -<number> changes horizontal value multiplication
            accumulatedRotation = Mathf.Lerp(accumulatedRotation, targetRotation, 0.08f); // Smoothness
            rotationZ = Mathf.Clamp(accumulatedRotation, -6, 6); // Maximum rotation
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && characterController.isGrounded)
        {
            if (!canStartBobbing)
            {
                initialCameraY = playerCamera.transform.localPosition.y;
                canStartBobbing = true;
            }
            timer += bobSpeed * Time.deltaTime;
            newPosition = new Vector3(
                Mathf.Cos(timer) * bobAmount,
                initialCameraY + Mathf.Abs((Mathf.Sin(timer) * bobAmount)),
                restPosition.z
            );
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                newPosition,
                smoothSpeed * Time.deltaTime
            );
        }
        else
        {
            canStartBobbing = false;
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                new Vector3(
                    playerCamera.transform.localPosition.x,
                    newPosition.y ,
                    playerCamera.transform.localPosition.z
                ),
                smoothSpeed * Time.deltaTime
            );
        }
             /* SETS CAMERA POSITION (when pressing move a few times in short time, makes a lagging effect)
        else
        {
            timer = Mathf.PI / 2;
        }
        */
            if (timer > Mathf.PI * 2)
            {
                timer = 0;
            }
            }
            
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, rotationZ);

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }


}