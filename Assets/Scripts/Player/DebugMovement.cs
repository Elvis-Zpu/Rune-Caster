using UnityEngine;
using UnityEngine.InputSystem;

public class DebugMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public float lookSensitivity = 3f;
    public float verticalMovementSpeed = 3f;

    private float verticalRotation = 0f;
    public Transform cameraTransform;

    public GameObject xrOrigin;
    public GameObject debugPlayerController;

    private bool isDesktopMode = true;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        var actionMap = playerInput.actions;

        actionMap["Move"].performed += OnMove;
        actionMap["Move"].canceled += OnMove;

        actionMap["Look"].performed += OnLook;
        actionMap["Look"].canceled += OnLook;

        actionMap["Sprint"].performed += OnSprint;
        actionMap["Sprint"].canceled += OnSprint;
    }

    private void OnDisable()
    {
        var actionMap = playerInput.actions;

        actionMap["Move"].performed -= OnMove;
        actionMap["Move"].canceled -= OnMove;

        actionMap["Look"].performed -= OnLook;
        actionMap["Look"].canceled -= OnLook;

        actionMap["Sprint"].performed -= OnSprint;
        actionMap["Sprint"].canceled -= OnSprint;
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            isDesktopMode = !isDesktopMode;
            if (isDesktopMode)
                SwitchToDesktopMode();
            else
                SwitchToVRMode();
        }

        if (isDesktopMode)
        {
            MovePlayer();
            LookAround();
            HandleVerticalMovement();
        }
    }

    private void MovePlayer()
    {
        float speed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }

    private void LookAround()
    {
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        verticalRotation -= lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void SwitchToDesktopMode()
    {
        debugPlayerController.SetActive(true);
        xrOrigin.SetActive(false);
        Debug.Log("Switched to Desktop Mode");
    }

    private void SwitchToVRMode()
    {
        xrOrigin.SetActive(true);
        debugPlayerController.SetActive(false);
        Debug.Log("Switched to VR Mode");
    }

    private void HandleVerticalMovement()
    {
        if (Keyboard.current.qKey.isPressed)
        {
            // Move downward
            transform.Translate(Vector3.down * verticalMovementSpeed * Time.deltaTime);
        }
        else if (Keyboard.current.eKey.isPressed)
        {
            // Move upward
            transform.Translate(Vector3.up * verticalMovementSpeed * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Move Input Received");
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("Look Input Received");
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Debug.Log("Sprint Input Received");
        isSprinting = context.ReadValueAsButton();
    }
}
