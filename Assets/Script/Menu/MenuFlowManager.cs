using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using Fusion.Sockets;
using System;

/// <summary>
/// Independent UI manager: shows creation/join status messages and player count.
/// Status messages auto-clear after 3 seconds; player count remains.
/// Also notifies when any player joins or leaves.
/// </summary>
public class MenuFlowManager: MonoBehaviour, INetworkRunnerCallbacks
{
    public static MenuFlowManager Instance;

    [Header("UI Components")]
    [Tooltip("Status message text (auto-clear)")]
    public TMP_Text statusText;
    [Tooltip("Player count text (persistent)")]
    public TMP_Text playerCountText;

    [Header("Network Runner Reference (optional)")]
    [Tooltip("If empty, will find the first NetworkRunner in scene")]
    public NetworkRunner runner;

    private int _playerCount;
    private Coroutine _clearCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        // Ensure callbacks registration in case Start occurred before UI setup
        if (runner == null)
            runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
            runner.AddCallbacks(this);
    }

    private void Start()
    {
        // Initialize player count display
        UpdatePlayerCount(0);

        // Also register callbacks if not done in OnEnable
        if (runner == null)
            runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
            runner.AddCallbacks(this);
    }

    /// <summary>
    /// Display a status message in English and clear after 3 seconds.
    /// </summary>
    public void ShowStatus(string message)
    {
        if (statusText == null) return;
        if (_clearCoroutine != null)
            StopCoroutine(_clearCoroutine);
        statusText.text = message;
        if (!string.IsNullOrEmpty(message))
            _clearCoroutine = StartCoroutine(ClearAfterDelay(3f));
    }

    private IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        statusText.text = string.Empty;
        _clearCoroutine = null;
    }

    /// <summary>
    /// Update the player count text in English (persistent).
    /// </summary>
    private void UpdatePlayerCount(int count)
    {
        _playerCount = count;
        if (playerCountText != null)
            playerCountText.text = $"Players online: {_playerCount}";
    }

    // === Fusion callbacks ===

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        UpdatePlayerCount(_playerCount + 1);
        ShowStatus("A player joined the session");

        if (EvidenceUIManager.Instance != null)
        {
            EvidenceUIManager.Instance.ShowMessage("A new player has joined!", 3f);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        UpdatePlayerCount(Mathf.Max(0, _playerCount - 1));
        ShowStatus("A player left the session");
    }

    /// <summary>
    /// Called on client when connected to a session (join success).
    /// </summary>
    public void OnConnectedToServer(NetworkRunner runner)
    {
        ShowStatus("Successfully joined the session");
    }

    /// <summary>
    /// Called when connection to session fails (join failure).
    /// </summary>
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        ShowStatus($"Failed to join session: {reason}");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey channel, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey channel, float progress) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
}


