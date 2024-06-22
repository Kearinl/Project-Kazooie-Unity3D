using UnityEngine;
using System.Diagnostics;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using System.Collections;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        public AudioClip JumpSound1;
        public AudioClip JumpSound2;
        public float JumpSoundVolume = 0.5f;
        private bool playJumpSound1 = true;
        private float timeSinceLastJumpSound;

        public float maxSlopeAngle = 60f;
        public float minSlopeAngle = 30f;

        public AudioClip SlowDownFallSound;
        public float SlowDownFallSoundVolume = 0.5f;
        private int slowDownFallCount = 0;
        private bool isDelayingSlowDown = false;
        private bool canSlowDownFall = true;

        private AudioSource _audioSource;
        private MeleeAttack meleeAttack;

        public GameObject banjoObject;

        [Header("Player")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        public bool isFloatingOnWater = false;
        public bool isUnderwater = false;
        public float floatingGravity = -2f;
        public float underwaterGravity = -1f;
        public float maxAirTime = 30.0f;
        private float currentAirTime;
        public float floatingSpeed = 2.0f;
        private PlayerHealth _playerHealth;

        public GameObject[] airImages;
        public GameObject airUIContainer;
        public GameObject airUIContainer2;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            meleeAttack = GetComponent<MeleeAttack>();
            _hasAnimator = TryGetComponent(out _animator);
            _playerHealth = GetComponent<PlayerHealth>();
            currentAirTime = maxAirTime;
            airUIContainer.SetActive(false);
            airUIContainer2.SetActive(false);
            _audioSource = GetComponent<AudioSource>();
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();

            if (isFloatingOnWater)
            {
                FloatingOnWater();
            }
            else if (isUnderwater)
            {
                Underwater();
            }

            if (isFloatingOnWater && Input.GetButton("Fire2"))
            {
                isFloatingOnWater = false;
                isUnderwater = true;
                _animator.SetBool("isUnderwater", true);
                currentAirTime = maxAirTime;
                UpdateAirUI();
            }

            if (_playerInput.actions["Attack"].triggered)
            {
                meleeAttack.PerformAttack();
                UnityEngine.Debug.Log("Left mouse button pressed: Performing attack!");
            }

            if (!_controller.isGrounded && _verticalVelocity < 0.0f && _playerInput.actions["Jump"].triggered)
            {
                SlowDownFall();
            }

            UpdateAnimatorParameters();
        }

        private void LateUpdate()
        {
            // Use for late update actions if necessary
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            if (isUnderwater || isFloatingOnWater)
            {
                Grounded = false;
            }
            else
            {
                Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
                Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

                if (Physics.CheckSphere(spherePosition, GroundedRadius, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                {
                    Grounded = true;
                }
            }

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void FloatingOnWater()
        {
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = floatingGravity;
            }

            Vector3 moveDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            moveDirection = transform.TransformDirection(moveDirection) * floatingSpeed;

            _controller.Move(moveDirection * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetBool("isFloatingOnWater", true);
            }
        }

        private void Underwater()
        {
            _verticalVelocity += underwaterGravity * Time.deltaTime;

            float verticalInput = 0.0f;

            if (Input.GetKey(KeyCode.Q))
            {
                verticalInput = 1.0f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.R))
            {
                verticalInput = -1.0f * Time.deltaTime;
            }

            Vector3 moveDirection = new Vector3(_input.move.x, 0.0f, _input.move.y);
            moveDirection = transform.TransformDirection(moveDirection).normalized;

            moveDirection += new Vector3(0.0f, verticalInput, 0.0f);

            _controller.Move(moveDirection * Time.deltaTime);

            currentAirTime -= Time.deltaTime;
            UpdateAirUI();

            airUIContainer.SetActive(true);
            airUIContainer2.SetActive(true);
            UpdateAirUI();

            if (currentAirTime <= 0)
            {
                if (_playerHealth != null)
                {
                    _playerHealth.TakeDamage(_playerHealth.maxHealth);
                }
            }

            if (_hasAnimator)
            {
                _animator.SetBool("isUnderwater", true);
            }
        }

        private void Move()
        {
            // Set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // A simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
            // Note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // If there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // A reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // Accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // Creates curved result rather than a linear one giving a more organic speed change
                // Note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // Round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // Normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // Note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // If there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // Rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // Get the normal of the ground to calculate the slope angle
            Vector3 slopeNormal = Vector3.up;
            if (Grounded)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, GroundedRadius + 0.1f, GroundLayers))
                {
                    slopeNormal = hit.normal;
                }
            }

            // Calculate the slope angle in degrees
            float slopeAngle = Vector3.Angle(Vector3.up, slopeNormal);

            // Calculate the grip factor based on the slope angle (you can adjust the thresholds as needed)
            float gripFactor = 1.0f;
            if (slopeAngle > maxSlopeAngle)
            {
                gripFactor = 0.0f;
            }
            else if (slopeAngle > minSlopeAngle)
            {
                gripFactor = 1.0f - (slopeAngle - minSlopeAngle) / (maxSlopeAngle - minSlopeAngle);
            }

            // Calculate the movement direction
            Vector3 moveDirection = targetDirection * _speed * inputMagnitude;

            // Apply grip factor to the movement speed
            moveDirection *= gripFactor;

            if (targetSpeed == SprintSpeed)
            {
                if (banjoObject != null)
                {
                    banjoObject.SetActive(true);
                }
            }
            else
            {
                if (banjoObject != null)
                {
                    banjoObject.SetActive(false);
                }
            }

            _controller.Move(moveDirection * Time.deltaTime +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void UpdateAnimatorParameters()
        {
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, _input.move.magnitude);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _input.jump = false;
                isFloatingOnWater = true;
                _animator.SetBool("isFloatingOnWater", true);
                currentAirTime = maxAirTime;
                airUIContainer.SetActive(false);
                airUIContainer2.SetActive(false);
                UpdateAirUI();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _input.jump = true;
                isFloatingOnWater = false;
                _animator.SetBool("isFloatingOnWater", false);
                isUnderwater = false;
                _animator.SetBool("isUnderwater", false);
                currentAirTime = maxAirTime;
                airUIContainer.SetActive(false);
                airUIContainer2.SetActive(false);
                UpdateAirUI();
            }
        }

        private void UpdateAirUI()
        {
            float airPercentage = currentAirTime / maxAirTime;

            int totalPairs = airImages.Length / 2;
            int activePairs = Mathf.CeilToInt(airPercentage * totalPairs);

            for (int i = 0; i < airImages.Length; i++)
            {
                int pairIndex = i / 2;

                if (pairIndex < activePairs)
                {
                    airImages[i].SetActive(true);
                }
                else
                {
                    airImages[i].SetActive(false);
                }
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                slowDownFallCount = 1;
                _animator.SetBool("SlowFall", false);

                if (!isFloatingOnWater && !isUnderwater)
                {
                    if (_verticalVelocity < _terminalVelocity)
                    {
                        _verticalVelocity += Gravity * Time.deltaTime;
                    }
                }

                if (banjoObject == null)
                {
                    Transform playerArmature = GameObject.Find("Banjo")?.transform;
                    if (playerArmature != null)
                    {
                        Transform banjo1 = playerArmature.Find("Banjo_1")?.transform;
                        if (banjo1 != null)
                        {
                            banjoObject = banjo1.Find("Banjo.mo.001")?.gameObject;
                        }
                    }
                }

                if (banjoObject != null)
                {
                    banjoObject.SetActive(false);
                }

                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (Time.time - timeSinceLastJumpSound >= 0.2f)
                    {
                        if (playJumpSound1)
                        {
                            AudioSource.PlayClipAtPoint(JumpSound1, transform.TransformPoint(_controller.center), JumpSoundVolume);
                        }
                        else
                        {
                            AudioSource.PlayClipAtPoint(JumpSound2, transform.TransformPoint(_controller.center), JumpSoundVolume);
                        }

                        playJumpSound1 = !playJumpSound1;
                        timeSinceLastJumpSound = Time.time;
                    }

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        public void SlowDownFall()
        {
            if (!_controller.isGrounded && _verticalVelocity < 0.0f && canSlowDownFall)
            {
                AudioSource.PlayClipAtPoint(SlowDownFallSound, transform.TransformPoint(_controller.center), SlowDownFallSoundVolume);

                slowDownFallCount++;

                if (_hasAnimator)
                {
                    if (banjoObject == null)
                    {
                        Transform playerArmature = GameObject.Find("PlayerArmature")?.transform;
                        if (playerArmature != null)
                        {
                            Transform banjo1 = playerArmature.Find("Banjo_1")?.transform;
                            if (banjo1 != null)
                            {
                                banjoObject = banjo1.Find("Banjo.mo.001")?.gameObject;
                            }
                        }
                    }

                    if (banjoObject != null)
                    {
                        banjoObject.SetActive(true);
                    }
                    _animator.SetBool("SlowFall", true);
                }

                _verticalVelocity /= -1.2f;
                _verticalVelocity -= 0.2f * Time.deltaTime;
                Vector3 upwardVelocity = Vector3.up * 3.0f;

                _controller.Move(upwardVelocity * Time.deltaTime);

                if (slowDownFallCount >= 2)
                {
                    StartCoroutine(DelaySlowDown());
                }
                else if (_controller.isGrounded)
                {
                    _audioSource.Stop();
                }
            }
        }

        private IEnumerator DelaySlowDown()
        {
            canSlowDownFall = false;
            isDelayingSlowDown = true;
            yield return new WaitForSeconds(2.0f);
            isDelayingSlowDown = false;
            canSlowDownFall = true;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}
