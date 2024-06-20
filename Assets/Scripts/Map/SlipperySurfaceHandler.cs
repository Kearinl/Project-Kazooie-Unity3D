using UnityEngine;

public class SlipperySurfaceHandler : MonoBehaviour
{
    public LayerMask slipperyLayers; // Layers that are slippery without Kazooie
    public bool isKazooieOut; // Variable to check if Kazooie is out
    public float slipStrength = 5f; // Strength of the slipping effect

    private CharacterController characterController;
    private Vector3 slipDirection;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Constantly apply slipping effect if Banjo is on a slippery surface and Kazooie is not out
        if (IsOnSlipperySurface() && !isKazooieOut)
        {
            ApplySlip();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if Banjo hits a slippery surface
        if (IsOnSlipperySurface(hit))
        {
            // If Kazooie is out, Banjo can stand on the surface
            if (isKazooieOut)
            {
                // Handle normal movement or interaction here if necessary
            }
            else
            {
                // Calculate slip direction based on hit normal
                slipDirection = new Vector3(hit.normal.x, 0.0f, hit.normal.z).normalized;
            }
        }
    }

    private bool IsOnSlipperySurface()
    {
        // Check if the character is on a slippery surface
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2 + 0.1f))
        {
            var hitLayerMask = 1 << hit.collider.gameObject.layer;
            return (hitLayerMask & slipperyLayers.value) != 0;
        }
        return false;
    }

    private bool IsOnSlipperySurface(ControllerColliderHit hit)
    {
        // Check if the hit surface is slippery
        var hitLayerMask = 1 << hit.collider.gameObject.layer;
        return (hitLayerMask & slipperyLayers.value) != 0;
    }

    private void ApplySlip()
    {
        // Apply slipping effect
        Vector3 slipForce = slipDirection * slipStrength * Time.deltaTime;
        characterController.Move(slipForce);
    }
}

