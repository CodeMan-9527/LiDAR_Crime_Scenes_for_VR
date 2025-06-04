using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EvidenceUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject firstLayerUI;
    public GameObject secondLayerUI;
   
    public Transform toggleParent; 
    public GameObject togglePrefab;

    public Button loadEvidenceButton;
    public Button unloadEvidenceButton;
    public Button backButton;

    [Header("Settings")]
   
    public string evidenceObjectName ;

    private Transform evidenceTransform;
    

    private Dictionary<Toggle, Transform> toggleMap = new Dictionary<Toggle, Transform>();
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

    private readonly Vector3 hiddenPosition = new Vector3(9999, -9999, 9999);

    void Start()
    {
        firstLayerUI.SetActive(true);
        secondLayerUI.SetActive(false);
    
        unloadEvidenceButton.gameObject.SetActive(false);
        loadEvidenceButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    public void loadEvidence() {
        evidenceObjectName = "evidence";
        ShowSecondUI();

    }
    public void loadRoom() {
        evidenceObjectName = "room";
        ShowSecondUI();

    }

  

    private string currentLoadedName = null;

    public void ShowSecondUI()
    {
        firstLayerUI.SetActive(false);
        secondLayerUI.SetActive(true);

        if (currentLoadedName != evidenceObjectName)
        {
            LoadEvidenceList(evidenceObjectName);
            currentLoadedName = evidenceObjectName;
        }
    }


    public void ShowFirstUI()
    {
        firstLayerUI.SetActive(true);

        secondLayerUI.SetActive(false);

    }

    void LoadEvidenceList(string evidenceObjectName)
    {
        // clear Toggle UI 
        foreach (Transform child in toggleParent)
        {
            Destroy(child.gameObject);
        }

        toggleMap.Clear();
        originalPositions.Clear();

        GameObject evidenceObj = GameObject.Find(evidenceObjectName);
        if (evidenceObj == null)
        {
            Debug.LogError($"Can't find GameObject named '{evidenceObjectName}'");
            return;
        }

        evidenceTransform = evidenceObj.transform;

        foreach (Transform child in evidenceTransform)
        {
            GameObject toggleGO = Instantiate(togglePrefab, toggleParent);
            Toggle toggle = toggleGO.GetComponent<Toggle>();
            TextMeshProUGUI label = toggleGO.GetComponentInChildren<TextMeshProUGUI>();
            label.text = child.name;
            toggle.isOn = false;

            toggleMap.Add(toggle, child);
            originalPositions[child] = child.position;

            child.gameObject.SetActive(true);
            child.position = hiddenPosition;
        }
    }




    public void LoadSelectedEvidence()
    {
        foreach (var pair in toggleMap)
        {
            Toggle toggle = pair.Key;
            Transform obj = pair.Value;

            if (toggle.isOn)
            {
                FindObjectOfType<EvidenceSpawnManager>()?.SnapToSpawn(obj);
            }
            else
            {
                obj.position = hiddenPosition;
            }

            toggle.interactable = false;
        }

        loadEvidenceButton.gameObject.SetActive(false);
        unloadEvidenceButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
    }

    public void UnloadEvidence()
    {
        foreach (var pair in toggleMap)
        {
            Toggle toggle = pair.Key;
            Transform obj = pair.Value;

            obj.position = hiddenPosition;

            toggle.isOn = false;
            toggle.interactable = true;
        }

        unloadEvidenceButton.gameObject.SetActive(false);
        loadEvidenceButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }
}
