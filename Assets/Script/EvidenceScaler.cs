using UnityEngine;
using UnityEngine.UI;

public class EvidenceScaler : MonoBehaviour
{
    public Transform evidenceParent; // Assign the "evidence" GameObject in Inspector
    public Transform roomParent;     // Assign the "room" GameObject in Inspector
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
        ScaleChildren(evidenceParent);
        ScaleChildren(roomParent);
    }

    void ScaleDown()
    {
        ScaleChildren(evidenceParent, scaleDown: true);
        ScaleChildren(roomParent, scaleDown: true);
    }

    void ScaleChildren(Transform parent, bool scaleDown = false)
    {
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf)
            {
                if (scaleDown)
                {
                    Vector3 newScale = child.localScale - Vector3.one * scaleStep;
                    child.localScale = Vector3.Max(newScale, Vector3.one * 0.1f);
                }
                else
                {
                    child.localScale += Vector3.one * scaleStep;
                }
            }
        }
    }
}
