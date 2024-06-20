using UnityEngine;

public class AnimatorTriggerController : MonoBehaviour
{
    public Animator animator;
    public string triggerTag = "Bottlestest";
    public GameObject banjoObject;
    
    private bool isInsideTrigger { get; set; }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            isInsideTrigger = true;
            
            animator.SetBool("TriggerFlag", true);
            Debug.Log($"{gameObject.name} entered trigger ({other.tag})");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            isInsideTrigger = false;
            
            animator.SetBool("TriggerFlag", false);
            Debug.Log($"{gameObject.name} exited trigger ({other.tag})");
            
            if (banjoObject != null)
            {
                banjoObject.SetActive(true);
                Debug.Log($"{gameObject.name} set active for trigger ({other.tag})");
            }
        }
    }
    
    void LateUpdate()
    {
        if (isInsideTrigger)
        {
            animator.SetBool("TriggerFlag", true);
            Debug.Log($"{gameObject.name} is in the trigger");
        }
    }
}

