﻿using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float walkingSpeed = 12f;
    [SerializeField] private float crouchSpeed = 4f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 3f;

    private float originalCenterY;
    private float originalHeight;
    private float coliderRadious;
    private float height;

    public CharacterController controller;
    public MouseNavigation cameraMovement;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask playerMask;
    int layerMaskAllButThePlayer;
    private Vector3 moveDirection;
    private bool isGrounded;
    private bool isCrouched;

    private void Start()
    {
        coliderRadious = controller.radius;
        height = originalHeight = controller.height;
        originalCenterY = controller.center.y;
        layerMaskAllButThePlayer = ~playerMask.value;
    }

    // Update is called once per frame
    private void Update()
    {
        Crouch();
        ApplyAxisMovement();
        GroundCheck();
        Jump();
        ApplyGravity();
        MovePlayer();
    }

    private void Crouch()
    {
        if (!Input.GetKeyDown("c"))
            return;

        // TODO: Right now the Crouch happens instantly to prove the concept.
        // 1. This part of the code has to be cleaned
        // 2. a smooth transition has to be implemented between for the height/center change and the camera - LaMaSu 29/03/2020
        if (!isCrouched)
        {
            // We change the heigh & center of the colider
            controller.height = height = 0.5f * originalHeight;
            controller.center = Vector3.up * originalCenterY * 0.5f;
            speed = crouchSpeed;
            isCrouched = true;
        }
        else
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, coliderRadious, Vector3.up, out hit, originalHeight, layerMaskAllButThePlayer))
                return;
            // We change the heigh & center of the colider
            controller.height = height = originalHeight;
            controller.center = Vector3.up * originalCenterY;
            speed = walkingSpeed;
            isCrouched = false;
        }
        cameraMovement.UpdateCameraHeightTarget(isCrouched);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y += -2f;
        }
    }

    private void ApplyAxisMovement()
    {
        if (controller.isGrounded)
        {
            moveDirection = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
            moveDirection *= speed;
        }
    }
    private void ApplyGravity()
    {
        moveDirection.y += gravity * Time.deltaTime;
    }

    private void MovePlayer()
    {
        controller.Move(moveDirection * Time.deltaTime);
    }


}
