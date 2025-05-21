using UnityEngine;
using System.IO;
using Dummiesman; // Runtime OBJ Importer
using SimpleFileBrowser; // File browser

public class OBJLoaderTest : MonoBehaviour
{
    public void OpenFileBrowser()
    {
        // Allow only .obj files
        FileBrowser.SetFilters(false, new FileBrowser.Filter("OBJ Files", ".obj"));

        // Start the file browser
        FileBrowser.ShowLoadDialog(
            OnFileSelected,
            OnCancel,
            FileBrowser.PickMode.Files,
            false,
            null,
            null,
            "Select 3D Model",
            "Load"
        );
    }

    private void OnFileSelected(string[] paths)
    {
        string path = paths[0];
        Debug.Log("Selected: " + path);

        // Load the .obj file
        GameObject loadedObj = new OBJLoader().Load(path);
        loadedObj.transform.position = Vector3.zero;
    }

    private void OnCancel()
    {
        Debug.Log("User canceled file selection");
    }
}
