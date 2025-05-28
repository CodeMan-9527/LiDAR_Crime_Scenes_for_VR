using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using Fusion.Sockets;
using System.Linq;

/// <summary>
/// Independent UI manager: shows creation/join status messages and player count.
/// Tracks player count via ActivePlayers to avoid API mismatches.
/// Status messages auto-clear after 3 seconds; player count remains.
/// </summary>
public class MenuFlowManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static MenuFlowManager Instance;

    [Header("UI Components")]
    public TMP_Text statusText;
    public TMP_Text playerCountText;

    [Header("Network Runner Reference (optional)")]
    public NetworkRunner runner;

    private Coroutine _clearCoroutine;
    private int _playerCount;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Early registration of callbacks
        if (runner == null)
            runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
            runner.AddCallbacks(this);
    }

    private void Start()
    {
        // Ensure callbacks are registered
        if (runner != null)
            runner.AddCallbacks(this);

        // Initialize count from active players
        if (runner != null)
            _playerCount = runner.ActivePlayers.Count();
        else
            _playerCount = 0;
        UpdatePlayerCount(_playerCount);
    }

    /// <summary>Displays a status message and clears it after 3 seconds.</summary>
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

    /// <summary>Updates the persistent player count display.</summary>
    private void UpdatePlayerCount(int count)
    {
        if (playerCountText == null) return;
        playerCountText.text = $"Players online: {count}";
        Debug.Log($"[UI] Updated player count to {count}");
    }

    // === Fusion callbacks ===

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("[UI] OnConnectedToServer");
        // Refresh count from active players
        _playerCount = runner.ActivePlayers.Count();
        UpdatePlayerCount(_playerCount);
        ShowStatus("Successfully joined the session");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogWarning($"[UI] OnConnectFailed: {reason}");
        ShowStatus($"Failed to join session: {reason}");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("[UI] OnPlayerJoined");
        // Update count from active players to ensure consistency
        _playerCount = runner.ActivePlayers.Count();
        UpdatePlayerCount(_playerCount);
        ShowStatus("A player joined the session");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("[UI] OnPlayerLeft");
        _playerCount = Mathf.Max(0, runner.ActivePlayers.Count());
        UpdatePlayerCount(_playerCount);
        ShowStatus("A player left the session");
    }

    // Unused callbacks
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey channel, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey channel, float progress) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
}
