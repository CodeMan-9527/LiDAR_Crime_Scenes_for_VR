using UnityEngine;


public class RayDragHandler : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor; // Assign the controller's XRRayInteractor in Inspector
    private Transform selectedObject;
    private Vector3 offset;
    private bool dragging;

    void Update()
    {
        if (rayInteractor == null)
            return;

        if (IsTriggerDown())
        {
            if (!dragging && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Draggable"))
                {
                    selectedObject = hit.transform;
                    Vector3 hitPoint = hit.point;
                    offset = selectedObject.position - hitPoint;
                    dragging = true;
                }
            }
        }
        else
        {
            dragging = false;
            selectedObject = null;
        }

        if (dragging && selectedObject != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit dragHit))
        {
            selectedObject.position = dragHit.point + offset;
        }
    }

    private bool IsTriggerDown()
    {
#if UNITY_OPENXR
        return Input.GetMouseButton(0); // Simulate with mouse left click in simulator
#else
        return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger); // Oculus headset
#endif
    }
}
