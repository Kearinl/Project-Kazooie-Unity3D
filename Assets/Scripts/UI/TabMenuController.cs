using UnityEngine;

public class TabMenuController : MonoBehaviour
{
    public Canvas tabMenuCanvas;

    private void Start()
    {
        tabMenuCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            tabMenuCanvas.gameObject.SetActive(true);
        }
        else
        {
            tabMenuCanvas.gameObject.SetActive(false);
        }
    }
}
