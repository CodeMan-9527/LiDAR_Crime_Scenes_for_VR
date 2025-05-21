using UnityEngine;

public class EvidenceSpawnManager : MonoBehaviour
{
    public Transform spawnPoint; // Assign your marker object in the Inspector

    // Call this after activating or instantiating an object to snap it to the spawn
    public void SnapToSpawn(Transform objTransform)
    {
        Renderer[] renderers = objTransform.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No Renderers found on object to align.");
            return;
        }

        // Combine all bounds
        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 bottomCenter = new Vector3(combinedBounds.center.x, combinedBounds.min.y, combinedBounds.center.z);
        Vector3 offset = spawnPoint.position - bottomCenter;

        objTransform.position += offset;
    }

}
