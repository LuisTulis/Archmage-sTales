using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour {
    public static CameraControl Instance { get; private set; }

    private CameraControlActions cameraActions;
    private InputAction movement;
    private Transform cameraTransform;
    public Transform cameraTarget;
    private float keyRotationDirection = 0f;


    [Header("Horizontal Motion")]
    [SerializeField] private float maxSpeed = 5f;
    private float speed;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float damping = 15f;

    [Header("Verical Motion - Zoom")]
    [SerializeField] private float stepSize = 2f;
    [SerializeField] private float zoomDampening = 7.5f;
    [SerializeField] private float minHeight = 5f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private float zoomSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private float maxRotationSpeed = 1f;
    [SerializeField] private float minVerticalRotation = 0f;
    [SerializeField] private float maxVerticalRotation = 60f;

    [Header("Screen Edge Motion")]
    [SerializeField][Range(0f, 0.1f)] private float edgeTolerance = 0.05f;
    [SerializeField] private bool useScreenEdge = true;

    private Vector3 targetPosition;

    private float zoomHeight;

    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;

    private void Awake() {

        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        cameraActions = new CameraControlActions();
        cameraTransform = this.GetComponentInChildren<Camera>().transform;
    }

    private void OnEnable() {
        zoomHeight = cameraTransform.localPosition.y;
        cameraTransform.LookAt(this.cameraTransform);

        lastPosition = this.transform.position;
        movement = cameraActions.Camera.Movement;

        cameraActions.Camera.RotateCamera.performed += RotateCamera;
        cameraActions.Camera.ZoomCamera.performed += ZoomCamera;

        cameraActions.Camera.RotateCameraKeys.performed += OnRotateKeyPressed;
        cameraActions.Camera.RotateCameraKeys.canceled += OnRotateKeyReleased;
        cameraActions.Camera.Enable();
    }

    private void OnDisable() {
        cameraActions.Camera.RotateCamera.performed -= RotateCamera;
        cameraActions.Camera.ZoomCamera.performed -= ZoomCamera;

        cameraActions.Camera.RotateCameraKeys.performed -= OnRotateKeyPressed;
        cameraActions.Camera.RotateCameraKeys.canceled -= OnRotateKeyReleased;

        cameraActions.Disable();
    }

    private void LateUpdate() {
        if (Keyboard.current.fKey.wasPressedThisFrame) {
            TrySelectTarget();
        }

        if (cameraTarget != null) {
            transform.position = cameraTarget.position;
            lastPosition = cameraTarget.position;
        } else {
            GetKeyboardMovement();

            if (useScreenEdge) {
                CheckMouseAtScreenEdge();
            }

            UpdateVelocity();
            UpdateCameraPosition();
            UpdateBasePosition();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            cameraTarget = null;
            transform.position = lastPosition;
        }

        if (Mathf.Abs(keyRotationDirection) > 0.01f) {
            transform.Rotate(0f, keyRotationDirection * maxRotationSpeed * Time.deltaTime * 100f, 0f);
        }
    }

    private void UpdateVelocity() {
        horizontalVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = this.transform.position;
    }

    private void GetKeyboardMovement() {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight()
                                + movement.ReadValue<Vector2>().y * GetCameraForward();

        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f) {
            targetPosition += inputValue;
        }
    }

    private Vector3 GetCameraRight() {
        Vector3 right = cameraTransform.right;
        right.y = 0;
        return right;
    }

    private Vector3 GetCameraForward() {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        return forward;
    }

    private void UpdateBasePosition() {
        if (targetPosition.sqrMagnitude > 0.1f) {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * speed * Time.deltaTime;
        } else {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }

        targetPosition = Vector3.zero;
    }


    private void RotateCamera(InputAction.CallbackContext inputValue) {
        if (Mouse.current.rightButton.isPressed && inputValue.control.device is Mouse) {
            Vector2 delta = inputValue.ReadValue<Vector2>();

            float horizontalRotation = delta.x * maxRotationSpeed;
            float verticalRotation = -delta.y * maxRotationSpeed;

            transform.Rotate(0f, horizontalRotation, 0f);

            float currentXRotation = transform.rotation.eulerAngles.x;
            float clampedXRotation = Mathf.Clamp(currentXRotation + verticalRotation, minVerticalRotation, maxVerticalRotation);

            transform.rotation = Quaternion.Euler(clampedXRotation, transform.rotation.eulerAngles.y, 0f);
        }
    }

    private void OnRotateKeyPressed(InputAction.CallbackContext ctx) {
        if (ctx.control == Keyboard.current.qKey) {
            keyRotationDirection = 1f;
        } else if (ctx.control == Keyboard.current.eKey) {
            keyRotationDirection = -1f;
        }
    }

    private void OnRotateKeyReleased(InputAction.CallbackContext ctx) {
        keyRotationDirection = 0f;
    }




    private void ZoomCamera(InputAction.CallbackContext inputValue) {
        float value = -inputValue.ReadValue<Vector2>().y / 100f;

        if (Mathf.Abs(value) > 0.1f) {
            zoomHeight = cameraTransform.localPosition.y + value * stepSize;
            if (zoomHeight < minHeight) {
                zoomHeight = minHeight;
            } else if (zoomHeight > maxHeight) {
                zoomHeight = maxHeight;
            }
        }
    }

    private void UpdateCameraPosition() {
        Vector3 zoomTarget = new Vector3(cameraTransform.localPosition.x, zoomHeight, cameraTransform.localPosition.z);
        zoomTarget -= zoomSpeed * (zoomHeight - cameraTransform.localPosition.y) * Vector3.forward;

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * zoomDampening);
        cameraTransform.LookAt(this.transform);
    }

    private void CheckMouseAtScreenEdge() {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        if (mousePosition.x < edgeTolerance * Screen.width) {
            moveDirection += -GetCameraRight();
        } else if (mousePosition.x > (1f - edgeTolerance) * Screen.width) {
            moveDirection += GetCameraRight();
        }

        if (mousePosition.y < edgeTolerance * Screen.height) {
            moveDirection += -GetCameraForward();
        } else if (mousePosition.y > (1f - edgeTolerance) * Screen.height) {
            moveDirection += GetCameraForward();
        }

        targetPosition += moveDirection;
    }

    private void TrySelectTarget() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            var character = hit.collider.GetComponent<CharacterComponent>();
            Debug.Log($"Hit {hit.collider.name}");
            if (character != null) {
                cameraTarget = character.transform;
            }
        }
    }

}