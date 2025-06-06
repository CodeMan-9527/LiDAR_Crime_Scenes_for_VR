using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RequestOwnershipOnGrab : MonoBehaviour
{
    private NetworkObject networkObject;

    void Awake()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    public void OnSelectEntered()
    {
        if (networkObject != null && networkObject.HasStateAuthority == false)
        {
            networkObject.RequestStateAuthority();
        }
    }
}
