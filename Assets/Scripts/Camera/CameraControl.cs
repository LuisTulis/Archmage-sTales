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
    [SerializeField] private float minHeight = 1f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private float zoomSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float moveSpeed = 0.05f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private Transform cameraPivot;

    private float yaw;
    private float pitch;
    private float pivotHeight = 0f;

    [Header("Screen Edge Motion")]
    [SerializeField][Range(0f, 0.1f)] private float edgeTolerance = 0.05f;
    [SerializeField] private bool useScreenEdge = true;

    [Header("Map Limits")]
    [SerializeField] private GameObject map;

    private Vector3 targetPosition;

    private float zoomHeight;

    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;
    private Bounds mapBounds;

    private void Awake() {

        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        cameraActions = new CameraControlActions();
        cameraTransform = this.GetComponentInChildren<Camera>().transform;
    }


    private void Start() {
        if (map != null) {
            Renderer mapRenderer = map.GetComponent<Renderer>();
            if (mapRenderer != null) {
                mapBounds = mapRenderer.bounds;
            } else {
                Collider mapCollider = map.GetComponent<Collider>();
                if (mapCollider != null) {
                    mapBounds = mapCollider.bounds;
                }
            }
        }
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
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            TrySelectTarget();
        }

        if (cameraTarget != null) {
            Vector3 targetPos = cameraTarget.position;
            cameraRoot.position = new Vector3(targetPos.x, cameraRoot.position.y, targetPos.z);

            lastPosition = cameraRoot.position;
        } else {
            GetKeyboardMovement();

            if (useScreenEdge) {
                CheckMouseAtScreenEdge();
            }

            UpdateVelocity();
            UpdateCameraPosition();
            UpdateBasePosition();

            ClampToMapBounds();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            cameraTarget = null;
            cameraRoot.position = new Vector3(lastPosition.x, cameraRoot.position.y, lastPosition.z);
        }

        if (Mathf.Abs(keyRotationDirection) > 0.01f) {
            transform.Rotate(0f, keyRotationDirection * rotationSpeed * Time.deltaTime * 100f, 0f);
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

            // Yaw en Y (horizontal)
            yaw += delta.x * rotationSpeed;

            // Pitch en X (vertical)
            pitch -= delta.y * rotationSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // Aplicar rotaciones
            cameraRoot.localRotation = Quaternion.Euler(0f, yaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
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
            pivotHeight += value * stepSize;
            pivotHeight = Mathf.Clamp(pivotHeight, minHeight, maxHeight);
        }
    }

    private void UpdateCameraPosition() {
        // Aplicar zoom moviendo el pivot en Y
        Vector3 targetPos = new Vector3(
            cameraPivot.localPosition.x,
            pivotHeight,
            cameraPivot.localPosition.z
        );

        cameraPivot.localPosition = Vector3.Lerp(
            cameraPivot.localPosition,
            targetPos,
            Time.deltaTime * zoomDampening
        );

        cameraTransform.LookAt(cameraPivot.position);
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

    private void ClampToMapBounds() {
        if (map == null) return;

        Vector3 pos = transform.position;

        // Usamos los bounds del mapa
        float minX = mapBounds.min.x;
        float maxX = mapBounds.max.x;
        float minZ = mapBounds.min.z;
        float maxZ = mapBounds.max.z;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }

}