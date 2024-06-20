using UnityEngine;

public class WheatOptimization : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Collider collider;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();

        // Apply optimizations to improve performance
        OptimizeForPerformance();
    }

    private void OptimizeForPerformance()
    {
        // Disable unnecessary components
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false; // Disable mesh rendering
        }

        if (collider != null)
        {
            collider.enabled = false; // Disable collider
        }

        // Disable unnecessary scripts
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        // Adjust LOD settings if available
        LODGroup lodGroup = GetComponent<LODGroup>();
        if (lodGroup != null)
        {
            lodGroup.enabled = false; // Disable LOD group
        }

        // If needed, disable particle systems or other components
        // Example: ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        // if (particleSystem != null)
        // {
        //     particleSystem.Stop();
        // }
    }
}
