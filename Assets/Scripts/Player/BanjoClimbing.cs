using UnityEngine;

public class BanjoClimbing : MonoBehaviour
{
    public string climbableTag = "Climbable";
    public float climbSpeed = 2f;

    private bool isClimbing;
    private Transform climbableObject;

    private void Update()
    {
        if (!isClimbing)
        {
            // Check for climbable surfaces when the player presses the climb input button
            // (Input.GetButtonDown("E"))
            if (Input.GetKeyDown(KeyCode.Space) && climbableObject != null)
            {
                StartClimbing();
            }
        }
        else
        {
            // Climb up while the player is pressing the climb input button
            if (Input.GetAxis("Vertical") > 0)
            {
                ClimbUp();
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                ClimbDown();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(climbableTag))
        {
            // Check if the collided object has the climbable tag
            climbableObject = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (climbableObject == other.transform)
        {
            // Reset climbableObject when leaving the climbable object's trigger
            climbableObject = null;
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        // Disable player movement controls here
    }

    private void ClimbUp()
    {
        // Move the player upwards along the climbable surface
        transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
    }

    private void ClimbDown()
    {
        // Move the player downwards along the climbable surface
        transform.Translate(Vector3.down * climbSpeed * Time.deltaTime);
    }
}
