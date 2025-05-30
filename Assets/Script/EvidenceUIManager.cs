using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EvidenceUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject firstLayerUI;
    public GameObject secondLayerUI;
    public Transform toggleParent; // Reference to ScrollView > Content
    public GameObject togglePrefab;

    public Button loadEvidenceButton;
    public Button unloadEvidenceButton;

    [Header("Settings")]
    public string evidenceObjectName = "evidence";

    private Transform evidenceTransform;
    private bool hasLoadedEvidenceList = false;

    private Dictionary<Toggle, Transform> toggleMap = new Dictionary<Toggle, Transform>();
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

    private readonly Vector3 hiddenPosition = new Vector3(9999, -9999, 9999);

    void Start()
    {
        firstLayerUI.SetActive(true);
        secondLayerUI.SetActive(false);
        unloadEvidenceButton.gameObject.SetActive(false);
        loadEvidenceButton.gameObject.SetActive(true);
    }

    public void ShowSecondUI()
    {
        firstLayerUI.SetActive(false);
        secondLayerUI.SetActive(true);

        if (!hasLoadedEvidenceList)
        {
            LoadEvidenceList();
        }
    }

    void LoadEvidenceList()
    {
        GameObject evidenceObj = GameObject.Find(evidenceObjectName);
        if (evidenceObj == null)
        {
            Debug.LogError("Can't find GameObject named 'evidence'");
            return;
        }

        evidenceTransform = evidenceObj.transform;
        toggleMap.Clear();

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

        hasLoadedEvidenceList = true;
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
    }
}
