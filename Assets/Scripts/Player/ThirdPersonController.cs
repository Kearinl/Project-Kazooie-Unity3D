using UnityEngine;
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
public float JumpSoundVolume = 0.5f; // Adjust this value to control the volume of the jump sounds
private bool playJumpSound1 = true;
private float timeSinceLastJumpSound;

public float maxSlopeAngle = 60f; // Adjust this value to control the max slope the player can grip
public float minSlopeAngle = 30f; // Adjust this value to control the min slope the player can grip

public AudioClip SlowDownFallSound;
public float SlowDownFallSoundVolume = 0.5f; // Adjust this value to control the volume of the SlowDownFall sound
private int slowDownFallCount = 0;
private bool isDelayingSlowDown = false;
private bool canSlowDownFall = true; // Add this flag to control whether SlowDownFall() can be called

        // Reference to the "AudioSource" audio component
        private AudioSource _audioSource;

        // Reference to the "MeleeAttack" script component
        private MeleeAttack meleeAttack;

        // Reference to the "leftMouseButtonDown" input key down.
        private bool leftMouseButtonDown;


public GameObject banjoObject;

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // Player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // Timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
    
        // Water States   
        public bool isFloatingOnWater = false;
        public bool isUnderwater = false;
        public float floatingGravity = -2f;
        public float underwaterGravity = -1f;
        public float maxAirTime = 30.0f;
        private float currentAirTime;
        public float floatingSpeed = 2.0f; // Speed while floating on water
        private PlayerHealth _playerHealth; // Reference to PlayerHealth script
        
        // UI Images representing air meter
        public GameObject[] airImages;
        public GameObject airUIContainer; // Parent container for air UI images
        public GameObject airUIContainer2; // Parent container for air UI images

        // Animation IDs
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
            // Get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
{
    // Get the "MeleeAttack" component attached to the player GameObject
    meleeAttack = GetComponent<MeleeAttack>();

    // Check if Animator component is available
    _hasAnimator = TryGetComponent(out _animator);
    
    // Water States
    _playerHealth = GetComponent<PlayerHealth>(); // Get the PlayerHealth component
    currentAirTime = maxAirTime; // Initialize air time
    airUIContainer.SetActive(false); // Hide air UI on start
    airUIContainer2.SetActive(false); // Hide air UI on start

    // Get the AudioSource component attached to this GameObject
    _audioSource = GetComponent<AudioSource>();

    // Get the CharacterController component attached to this GameObject
    _controller = GetComponent<CharacterController>();

    // Get the StarterAssetsInputs component attached to this GameObject
    _input = GetComponent<StarterAssetsInputs>();

    // Conditional check for PlayerInput component
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    _playerInput = GetComponent<PlayerInput>();
#else
    // Log an error if Starter Assets dependencies are missing
    Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

    // Call a method to assign Animation IDs
    AssignAnimationIDs();

    // Reset our timeouts on start
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
            

    // Call PerformAttack when left mouse button is pressed
    if (_playerInput.actions["Attack"].triggered)
    {
        meleeAttack.PerformAttack();
        Debug.Log("Left mouse button pressed: Performing attack!");
    }
            
    // Call the SlowDownFall function when 'Space' key is pressed while falling
    if (!_controller.isGrounded && _verticalVelocity < 0.0f && _playerInput.actions["Jump"].triggered)
    {
        SlowDownFall();
            }
        }

        // Used for late update calling function
        private void LateUpdate()
        {


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
            // Check if the player is underwater or floating on water
            if (isUnderwater || isFloatingOnWater)
            {
                Grounded = false;
            }
            else
            {
                // Set sphere position, with offset
                Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
                Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

                // Check if the player is also colliding with the default layer
                if (Physics.CheckSphere(spherePosition, GroundedRadius, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                {
                    Grounded = true;
                }
            }

            // Update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }


        private void FloatingOnWater()
    {
        // Apply floating gravity
        if (_verticalVelocity < 0.0f)
        {
            _verticalVelocity = floatingGravity;
        }

        // Floating movement logic
        Vector3 moveDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        moveDirection = transform.TransformDirection(moveDirection) * floatingSpeed; // Apply movement speed

        // Apply movement
        _controller.Move(moveDirection * Time.deltaTime);

        // Update animator
        if (_hasAnimator)
        {
            _animator.SetBool("isFloatingOnWater", true);
        }
    }

    private void Underwater()
{
    // Apply underwater gravity
    _verticalVelocity += underwaterGravity * Time.deltaTime;

    // Underwater movement logic
    float verticalInput = 0.0f;

    // Check for Q (up) and R (down) input
if (Input.GetKey(KeyCode.Q))
{
    verticalInput = 1.0f * Time.deltaTime; // Move up over time
}
else if (Input.GetKey(KeyCode.R))
{
    verticalInput = -1.0f * Time.deltaTime; // Move down over time
}
else
{
    verticalInput = 0.0f; // No vertical movement if neither Q nor R is pressed
}

    Vector3 moveDirection = new Vector3(_input.move.x, 0.0f, _input.move.y);
    moveDirection = transform.TransformDirection(moveDirection).normalized;

    // Modify moveDirection to include vertical movement
    moveDirection += new Vector3(0.0f, verticalInput, 0.0f);

    // Apply movement
    _controller.Move(moveDirection * Time.deltaTime);

    // Air depletion
    currentAirTime -= Time.deltaTime;
    UpdateAirUI();

    // Show air UI when underwater
    airUIContainer.SetActive(true);
    airUIContainer2.SetActive(true);
    UpdateAirUI();

    if (currentAirTime <= 0)
    {
        // Handle out of air scenario
        if (_playerHealth != null)
        {
            _playerHealth.TakeDamage(_playerHealth.maxHealth); // Take damage equal to max health
        }
    }

    // Update animator
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
    if (slopeAngle > maxSlopeAngle) // maxSlopeAngle is a variable you can set to control the maximum slope the player can grip
    {
        gripFactor = 0.0f; // The player won't be able to grip on slopes above the maxSlopeAngle
    }
    else if (slopeAngle > minSlopeAngle) // minSlopeAngle is a variable you can set to control the minimum slope the player can grip
    {
        gripFactor = 1.0f - (slopeAngle - minSlopeAngle) / (maxSlopeAngle - minSlopeAngle);
    }

    // Calculate the movement direction
    Vector3 moveDirection = targetDirection * _speed * inputMagnitude;

    // Apply grip factor to the movement speed
    moveDirection *= gripFactor;
    
     if (targetSpeed == SprintSpeed)
    {
        // If so, activate the banjoObject
        if (banjoObject != null)
        {
            banjoObject.SetActive(true);
        }
    }
    else
    {
        // If targetSpeed is not equal to SprintSpeed, deactivate the banjoObject (optional)
        if (banjoObject != null)
        {
            banjoObject.SetActive(false);
        }
    }

    // Move the player
    _controller.Move(moveDirection * Time.deltaTime +
                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

    // Update animator if using character
    if (_hasAnimator)
    {
        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }
}



private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
        // Ensure player cannot jump when not grounded
                _input.jump = false;
            isFloatingOnWater = true;
            _animator.SetBool("isFloatingOnWater", true);
            currentAirTime = maxAirTime; // Reset air time when entering water
            airUIContainer.SetActive(false); // Hide air UI when floating
            airUIContainer2.SetActive(false); // Hide air UI when floating
            UpdateAirUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
        
        // Ensure player cannot jump when not grounded
                _input.jump = true;
            isFloatingOnWater = false;
            _animator.SetBool("isFloatingOnWater", false);
            isUnderwater = false; // Reset underwater state if exiting water
            _animator.SetBool("isUnderwater", false);
            currentAirTime = maxAirTime; // Reset air time when exiting water
            airUIContainer.SetActive(false); // Hide air UI when exiting water
            airUIContainer2.SetActive(false); // Hide air UI when exiting water
            UpdateAirUI();
        }
    }

    private void UpdateAirUI()
{
    // Calculate the percentage of air remaining
    float airPercentage = currentAirTime / maxAirTime;

    // Determine the number of active pairs of air images based on the air percentage
    int totalPairs = airImages.Length / 2; // Each level has 2 images
    int activePairs = Mathf.CeilToInt(airPercentage * totalPairs);

    // Activate or deactivate air images based on the current air percentage
    for (int i = 0; i < airImages.Length; i++)
    {
        // Determine which pair this image belongs to
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
            // Check if the player is grounded
            if (Grounded)
            {
                // Reset variables related to jumping and falling
                slowDownFallCount = 1;
                _animator.SetBool("SlowFall", false);

                // Handle gravity if not floating on water or underwater
                if (!isFloatingOnWater && !isUnderwater)
                {
                    if (_verticalVelocity < _terminalVelocity)
                    {
                        _verticalVelocity += Gravity * Time.deltaTime;
                    }
                }

                // Find and disable the Banjo object if it exists
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

                // Reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // Update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // Stop vertical velocity from increasing infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Handle jumping
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // Calculate vertical velocity needed for jump height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // Play jump sound alternately with adjusted volume
                    if (Time.time - timeSinceLastJumpSound >= 0.2f) // Adjust the time interval as needed
                    {
                        if (playJumpSound1)
                        {
                            AudioSource.PlayClipAtPoint(JumpSound1, transform.TransformPoint(_controller.center), JumpSoundVolume);
                        }
                        else
                        {
                            AudioSource.PlayClipAtPoint(JumpSound2, transform.TransformPoint(_controller.center), JumpSoundVolume);
                        }

                        playJumpSound1 = !playJumpSound1; // Toggle for next jump sound
                        timeSinceLastJumpSound = Time.time; // Update last jump sound time
                    }

                    // Update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // Decrease jump timeout timer
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else // Player is not grounded
            {
                // Reset jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // Decrease fall timeout timer
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // Handle freefall moment animation if using character animator
                    if (_hasAnimator)
                    {
                        // Call your freefall animation update here
                    }
                }

                // Ensure player cannot jump when not grounded
                _input.jump = false;
            }

            // Apply gravity over time
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

        // Reduce the fall speed more gently
        _verticalVelocity /= -1.2f; // Adjust this value based on the desired effect

        // Apply a small downward force
        _verticalVelocity -= 0.2f * Time.deltaTime; // Adjust this value based on the desired effect

        // Move the character slightly upwards with reduced intensity
        Vector3 upwardVelocity = Vector3.up * 3.0f; // Adjust the upward speed as needed

        // Apply the velocities
        _controller.Move(upwardVelocity * Time.deltaTime);

        // If the method has been called twice, start the delay coroutine
        if (slowDownFallCount >= 2)
        {
            StartCoroutine(DelaySlowDown());
        }
        else if (_controller.isGrounded)
        {
        // If the character becomes grounded while playing the sound, stop the sound
            _audioSource.Stop();
        }
    }
}

// Coroutine for the delay before allowing SlowDownFall() to be called again
private IEnumerator DelaySlowDown()
{
    canSlowDownFall = false; // Prevent calling SlowDownFall() during the delay phase
    isDelayingSlowDown = true;
    yield return new WaitForSeconds(2.0f); // Adjust the delay duration as needed
    isDelayingSlowDown = false;
    canSlowDownFall = true; // Allow calling SlowDownFall() again after the delay is over
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

            // When selected, draw a gizmo in the position of, and matching radius of, the grounded collider
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
                    var index = Random.Range(0, FootstepAudioClips.Length);
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
