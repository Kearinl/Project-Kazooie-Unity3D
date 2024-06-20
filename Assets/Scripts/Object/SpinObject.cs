using UnityEngine;

public class SpinObject : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public RotationAxis rotationAxis = RotationAxis.Z; // Default rotation axis is Z
    public float rotationSpeed = 100f; // Speed of rotation

    void Update()
    {
        // Rotate the object around the selected axis at the specified speed
        switch (rotationAxis)
        {
            case RotationAxis.X:
                transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
                break;
            case RotationAxis.Y:
                transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                break;
            case RotationAxis.Z:
                transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
                break;
        }
    }
}

