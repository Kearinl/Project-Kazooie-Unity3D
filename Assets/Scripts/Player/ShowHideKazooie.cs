using UnityEngine;

public class ShowHideKazooie : MonoBehaviour
{
    public GameObject KazooieObject;

    private void Update()
    {
        Debug.Log("Update method called.");
        
        // Check if the Shift key is held down
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
         if (KazooieObject == null)
    {
        Transform playerArmature = GameObject.Find("Banjo")?.transform;
        if (playerArmature != null)
        {
            Transform banjo1 = playerArmature.Find("Banjo_1")?.transform;
            if (banjo1 != null)
            {
                KazooieObject = banjo1.Find("Banjo.mo.001")?.gameObject;
            }
        }
    }

    // Check if the GameObject is found, then enable it
   if (KazooieObject != null)
{
    KazooieObject.SetActive(false);
}
    }
}
