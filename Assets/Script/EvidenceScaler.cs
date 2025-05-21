using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EvidenceScaler : MonoBehaviour
{
    public Transform evidenceParent; // Drag the "evidence" GameObject here
    public Button scaleUpButton;
    public Button scaleDownButton;
    public float scaleStep = 0.1f;

    private void Start()
    {
        scaleUpButton.onClick.AddListener(ScaleUp);
        scaleDownButton.onClick.AddListener(ScaleDown);
    }

    void ScaleUp()
    {
        foreach (Transform child in evidenceParent)
        {
            if (child.gameObject.activeSelf)
            {
                child.localScale += Vector3.one * scaleStep;
            }
        }
    }

    void ScaleDown()
    {
        foreach (Transform child in evidenceParent)
        {
            if (child.gameObject.activeSelf)
            {
                Vector3 newScale = child.localScale - Vector3.one * scaleStep;
                child.localScale = Vector3.Max(newScale, Vector3.one * 0.1f);
            }
        }
    }
}
