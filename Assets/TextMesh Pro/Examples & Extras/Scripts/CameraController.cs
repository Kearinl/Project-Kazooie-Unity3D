using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace TMPro.Examples
{
    public class CameraController : MonoBehaviour
    {
        public enum CameraModes { Follow, Isometric, Free }

    private Transform cameraTransform;
    private Transform dummyTarget;
    private Transform CameraTarget; // Declare the CameraTarget variable

        private PlayerInput _playerInput;

        public float FollowDistance = 30.0f;
        public float MaxFollowDistance = 100.0f;
        public float MinFollowDistance = 2.0f;

        public float ElevationAngle = 30.0f;
        public float MaxElevationAngle = 85.0f;
        public float MinElevationAngle = 0f;

        public float OrbitalAngle = 0f;

        public CameraModes CameraMode = CameraModes.Follow;

        public bool MovementSmoothing = true;
        public bool RotationSmoothing = false;
        private bool previousSmoothing;

        public float MovementSmoothingValue = 25f;
        public float RotationSmoothingValue = 5.0f;

        public float MoveSensitivity = 2.0f;

        private Vector3 currentVelocity = Vector3.zero;
        private Vector3 desiredPosition;
        private float mouseX;
        private float mouseY;
        private Vector3 moveVector;
        private float mouseWheel;

        // Controls for Touches on Mobile devices
        //private float prev_ZoomDelta;

        private const string event_SmoothingValue = "Slider - Smoothing Value";
        private const string event_FollowDistance = "Slider - Camera Zoom";

        void Awake()
        {
            if (QualitySettings.vSyncCount > 0)
                Application.targetFrameRate = 60;
            else
                Application.targetFrameRate = -1;

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
                Input.simulateMouseWithTouches = false;

            _playerInput = GetComponent<PlayerInput>();

            cameraTransform = transform;
            previousSmoothing = MovementSmoothing;
        }

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player"); // Change "Player" to the actual tag of your player GameObject

            if (player != null)
            {
                CameraTarget = player.transform;
            }
            else
            {
                // If no player GameObject is found, create a dummy target in the center of the scene.
                dummyTarget = new GameObject("Camera Target").transform;
                CameraTarget = dummyTarget;
            }
        }

        void LateUpdate()
        {
            GetPlayerInput();

            if (CameraTarget != null)
            {
                if (CameraMode == CameraModes.Isometric)
                {
                    desiredPosition = CameraTarget.position + Quaternion.Euler(ElevationAngle, OrbitalAngle, 0f) * new Vector3(0, 0, -FollowDistance);
                }
                else if (CameraMode == CameraModes.Follow)
                {
                    desiredPosition = CameraTarget.position + CameraTarget.TransformDirection(Quaternion.Euler(ElevationAngle, OrbitalAngle, 0f) * new Vector3(0, 0, -FollowDistance));
                }
                else
                {
                    // Free Camera implementation
                    mouseX = Input.GetAxis("Mouse X");
                    mouseY = Input.GetAxis("Mouse Y");

                    if (mouseX != 0 || mouseY != 0)
                    {
                        Vector3 moveDirection = new Vector3(mouseX, 0, mouseY);
                        moveDirection = cameraTransform.TransformDirection(moveDirection);
                        moveDirection.y = 0;

                        desiredPosition += moveDirection * MoveSensitivity;
                    }
                }

                RaycastHit hit;
                if (Physics.Linecast(CameraTarget.position, desiredPosition, out hit))
                {
                    desiredPosition = hit.point;
                }

                if (MovementSmoothing == true)
                {
                    cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, desiredPosition, ref currentVelocity, MovementSmoothingValue * Time.fixedDeltaTime);
                }
                else
                {
                    cameraTransform.position = desiredPosition;
                }

                if (RotationSmoothing == true)
                {
                    cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.LookRotation(CameraTarget.position - cameraTransform.position), RotationSmoothingValue * Time.deltaTime);
                }
                else
                {
                    cameraTransform.LookAt(CameraTarget);
                }
            }
        }

        void GetPlayerInput()
        {
            moveVector = Vector3.zero;

            Vector2 scrollInput = _playerInput.actions["Scroll"].ReadValue<Vector2>();
            mouseWheel = scrollInput.y * 10f; // Adjust the multiplier to control the zoom speed

            float touchCount = Input.touchCount;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || touchCount > 0)
            {
                mouseWheel *= 10;

                if (Input.GetKeyDown(KeyCode.I))
                    CameraMode = CameraModes.Isometric;

                if (Input.GetKeyDown(KeyCode.F))
                    CameraMode = CameraModes.Follow;

                if (Input.GetKeyDown(KeyCode.S))
                    MovementSmoothing = !MovementSmoothing;

                if (Input.GetMouseButton(1))
                {
                    mouseY = Input.GetAxis("Mouse Y");
                    mouseX = Input.GetAxis("Mouse X");

                    if (mouseY > 0.01f || mouseY < -0.01f)
                    {
                        ElevationAngle -= mouseY * MoveSensitivity;
                        ElevationAngle = Mathf.Clamp(ElevationAngle, MinElevationAngle, MaxElevationAngle);
                    }

                    if (mouseX > 0.01f || mouseX < -0.01f)
                    {
                        OrbitalAngle += mouseX * MoveSensitivity;
                        if (OrbitalAngle > 360)
                            OrbitalAngle -= 360;
                        if (OrbitalAngle < 0)
                            OrbitalAngle += 360;
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 300, 1 << 10 | 1 << 11 | 1 << 12 | 1 << 14))
                    {
                        if (hit.transform == CameraTarget)
                        {
                            OrbitalAngle = 0;
                        }
                        else
                        {
                            CameraTarget = hit.transform;
                            OrbitalAngle = 0;
                            MovementSmoothing = previousSmoothing;
                        }
                    }
                }

                if (Input.GetMouseButton(2))
                {
                    if (dummyTarget == null)
                    {
                        dummyTarget = new GameObject("Camera Target").transform;
                        dummyTarget.position = CameraTarget.position;
                        dummyTarget.rotation = CameraTarget.rotation;
                        CameraTarget = dummyTarget;
                        previousSmoothing = MovementSmoothing;
                        MovementSmoothing = false;
                    }
                    else if (dummyTarget != CameraTarget)
                    {
                        dummyTarget.position = CameraTarget.position;
                        dummyTarget.rotation = CameraTarget.rotation;
                        CameraTarget = dummyTarget;
                        previousSmoothing = MovementSmoothing;
                        MovementSmoothing = false;
                    }

                    mouseY = Input.GetAxis("Mouse Y");
                    mouseX = Input.GetAxis("Mouse X");

                    moveVector = cameraTransform.TransformDirection(mouseX, mouseY, 0);

                    dummyTarget.Translate(-moveVector, Space.World);
                }
            }

            if (Mathf.Abs(mouseWheel) > 0.01f)
            {
                FollowDistance -= mouseWheel * 5.0f;
                FollowDistance = Mathf.Clamp(FollowDistance, MinFollowDistance, MaxFollowDistance);
            }
        }
    }
}
