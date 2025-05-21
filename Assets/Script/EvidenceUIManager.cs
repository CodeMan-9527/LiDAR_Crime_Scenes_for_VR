using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EvidenceUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject firstLayerUI;
    public GameObject secondLayerUI;

    public TMP_Dropdown dropdown;
    public Button loadEvidenceButton;
    public Button unloadEvidenceButton;

    [Header("Settings")]
    public string evidenceObjectName = "evidence";

    private Transform evidenceTransform;
    private bool hasLoadedEvidenceList = false;

    void Start()
    {
        // 初始状态
        firstLayerUI.SetActive(true);
        secondLayerUI.SetActive(false);
        unloadEvidenceButton.gameObject.SetActive(false);
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

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (Transform child in evidenceTransform)
        {
            options.Add(new TMP_Dropdown.OptionData(child.name));
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        hasLoadedEvidenceList = true;
    }

    public void LoadSelectedEvidence()
    {
        int index = dropdown.value;
        if (evidenceTransform == null || index < 0 || index >= evidenceTransform.childCount)
            return;

        // 全部先关闭
        foreach (Transform child in evidenceTransform)
        {
            child.gameObject.SetActive(false);
        }

        // 激活选中项
        evidenceTransform.GetChild(index).gameObject.SetActive(true);

        loadEvidenceButton.gameObject.SetActive(false);
        unloadEvidenceButton.gameObject.SetActive(true);
    }

    public void UnloadEvidence()
    {
        if (evidenceTransform == null) return;

        foreach (Transform child in evidenceTransform)
        {
            child.gameObject.SetActive(false);
        }

        unloadEvidenceButton.gameObject.SetActive(false);
        loadEvidenceButton.gameObject.SetActive(true);
    }
}
