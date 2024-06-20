using UnityEngine;

public class LevelTransitionController : MonoBehaviour
{
    public GameObject objectToEnable;
    public GameObject objectToDisable;
    public LayerMask playerLayer;

    private void Update()
    {
        // Cast a ray to check for the player within the defined radius
        if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, playerLayer))
        {
            if (hit.collider.CompareTag("EnterWitchLair"))
            {
                if (objectToEnable != null)
                {
                    objectToEnable.SetActive(true);
                }

                if (objectToDisable != null)
                {
                    objectToDisable.SetActive(false);
                }
            }
        }
    }
}
