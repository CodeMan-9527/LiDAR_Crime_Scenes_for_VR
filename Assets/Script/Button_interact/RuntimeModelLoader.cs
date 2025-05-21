using System.Collections;
using System.IO;
using UnityEngine;

public class ModelLoader : MonoBehaviour
{
    public string objRelativePath = "Models/Model_1/morrisChair.obj";
    public Vector3 spawnPosition = new Vector3(0, 0, 2);

    private GameObject currentModel;

    public void LoadModel()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, objRelativePath);
        if (File.Exists(fullPath))
            StartCoroutine(LoadOBJ(fullPath));
    }

    public void UnloadModel()
    {
        if (currentModel != null) Destroy(currentModel);
    }

    IEnumerator LoadOBJ(string path)
    {
        yield return null;
        var loader = new Dummiesman.OBJLoader();
        currentModel = loader.Load(path);
        currentModel.transform.position = spawnPosition;
    }
}
