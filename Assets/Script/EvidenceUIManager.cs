using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EvidenceUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject firstLayerUI;
    public GameObject secondLayerUI;
    public Transform toggleParent; // Content 对象
    public GameObject togglePrefab; // Toggle 预制体

    public Button loadEvidenceButton;
    public Button unloadEvidenceButton;

    [Header("Settings")]
    public string evidenceObjectName = "evidence";

    private Transform evidenceTransform;
    private bool hasLoadedEvidenceList = false;

    // 用于记录每个 Toggle 及其绑定的物体
    private Dictionary<Toggle, Transform> toggleMap = new Dictionary<Toggle, Transform>();
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

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
            // 创建 Toggle
            GameObject toggleGO = Instantiate(togglePrefab, toggleParent);
            Toggle toggle = toggleGO.GetComponent<Toggle>();
            TextMeshProUGUI label = toggleGO.GetComponentInChildren<TextMeshProUGUI>();
            label.text = child.name;
            toggle.isOn = false; // 默认未选中

            toggleMap.Add(toggle, child);
            originalPositions[child] = child.position;
        }

        hasLoadedEvidenceList = true;
    }

    public void LoadSelectedEvidence()
    {
        float spacing = 0.2f; // 物体间的间距
        float startPos = 0.0f; // 起始位置

        int counter = 0; // 计数器，用于沿直线排列

        foreach (var pair in toggleMap)
        {
            Toggle toggle = pair.Key;
            Transform evidence = pair.Value;

            if (toggle.isOn)
            {
                // 沿着X轴按照导入顺序排列
                evidence.position = new Vector3(startPos + (counter * spacing),0.9f,0.3f);
                counter++;
            }

            // 设置 toggle 为不可选
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
            Transform evidence = pair.Value;

            if (originalPositions.ContainsKey(evidence))
            {
                evidence.position = originalPositions[evidence];
            }

            // 恢复 toggle 为可交互
            toggle.interactable = true;
            toggle.isOn = false;
        }

        unloadEvidenceButton.gameObject.SetActive(false);
        loadEvidenceButton.gameObject.SetActive(true);
    }
}
