using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabMonitor : MonoBehaviour
{
    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener((args) => {
                Debug.Log($"[GrabMonitor] 抓取开始：{args.interactorObject.transform.name}");
            });

            grab.selectExited.AddListener((args) => {
                Debug.Log($"[GrabMonitor] 抓取结束：{args.interactorObject.transform.name}");
            });
        }
        else
        {
            Debug.LogWarning("[GrabMonitor] XRGrabInteractable 没有挂载！");
        }
    }
}
