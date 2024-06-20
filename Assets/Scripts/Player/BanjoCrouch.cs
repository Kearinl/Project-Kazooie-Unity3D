using UnityEngine;
using UnityEngine.InputSystem;


public class BanjoCrouch : MonoBehaviour
{
    public float rotationSpeed = 8f; // The rotation speed when crouching
    public float backflipForceUp = 10f; // The upward force for the backflip
    public float backflipForceBackward = 5f; // The backward force for the backflip
    public float backflipDuration = 1f; // The duration of the backflip

    private Animator _animator;
    private CharacterController _characterController;
    private bool isCrouching = false;

    
    private PlayerInput _playerInput; // Reference to the PlayerInput component

    private Vector3 backflipVelocity; // The calculated velocity for the backflip
    private float backflipTimer; // The timer to track backflip duration
    
    private bool isPerformingBackflip = false; // Flag to track if backflip is being performed

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        
    }

    private void Update()
    {
    
    
    
    // Use the crouch action from the input system
    float crouchInputValue = _playerInput.actions["Crouch"].ReadValue<float>();
        // Check if the "C" key is being held down
        //if (Input.GetKey(KeyCode.C))
         if (crouchInputValue > 0.0f) // Check if the crouch input value is greater than 0
        {
            if (!isCrouching)
            {
                ToggleCrouch(true);
            }
        }
        else if (isCrouching)
        {
            ToggleCrouch(false);
        }

        // Check for backflip input
        if (_playerInput.actions["Jump"].triggered)
        {
            if (isCrouching)
            {
            
                PerformBackflip();
            }
        }

        // Update backflip motion if active
        if (backflipTimer > 0f)
        {
            UpdateBackflipMotion();
        }
        
       // Check if the move action was triggered
    if (_playerInput.actions["Move"].triggered && isCrouching)
    {
        Vector2 moveInputVector = _playerInput.actions["Move"].ReadValue<Vector2>();
        RotateCharacter(moveInputVector);
    }
    
    
}

void RotateCharacter(Vector2 moveInputVector)
{
    // Extract the horizontal component of the move input vector
    float horizontalInput = moveInputVector.x;

    // Rotate the character based on the horizontal input
    transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
}

    private void ToggleCrouch(bool crouching)
    {
        // Set crouch state
        isCrouching = crouching;

        // Handle crouch animation and character controller movement
        if (isCrouching)
        {
            _animator.SetTrigger("Crouch");
            _animator.SetBool("Crouching", true);
            _characterController.enabled = false;
        }
        else
        {
            _animator.ResetTrigger("Crouch");
            _animator.SetBool("Crouching", false);
            _characterController.enabled = true;
        }
    }

    private void PerformBackflip()
    {
        // Toggle off crouching before performing backflip
         _characterController.enabled = true;

 // Trigger backflip animation
        _animator.SetTrigger("PerformBackflip");
        
        isPerformingBackflip = true;
        
        // Calculate initial backflip velocity
        backflipVelocity = transform.up * backflipForceUp - transform.forward * backflipForceBackward;
        backflipTimer = backflipDuration;
    }

    private void UpdateBackflipMotion()
    {
     _characterController.enabled = true;
        // Update position based on calculated velocity
        Vector3 displacement = backflipVelocity * Time.deltaTime;
        _characterController.Move(displacement);

        // Calculate new velocity to simulate deceleration
        backflipVelocity -= transform.up * backflipForceUp * Time.deltaTime;

        // Update backflip timer
        backflipTimer -= Time.deltaTime;

        // If backflip duration is over, reset variables
        if (backflipTimer <= 0f)
        {
            backflipVelocity = Vector3.zero;
            backflipTimer = 0f;
            isPerformingBackflip = false;
        }
     ToggleCrouch(false);
    }
}
