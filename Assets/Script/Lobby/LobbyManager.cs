using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Fusion.Sockets;

/// <summary>
/// Lobby manager: handles create/join/exit room, updates UI list, player count and status messages.
/// Prevents invalid operations, logs each step to Console, and supports re-creating sessions after exit.
/// </summary>
public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("UI References")]
    public TMP_InputField roomNameInput;
    public Button createRoomBtn;
    public TMP_InputField joinRoomInput;
    public Button joinRoomBtn;
    public Button exitRoomBtn;
    public Transform roomListContent;
    public GameObject roomListItemPrefab;

    [Header("Status & Count UI")]
    public TMP_Text statusText;
    public TMP_Text playerCountText;

    private List<string> _roomNames = new List<string>();
    private bool _isOperationRunning;
    private NetworkRunner _runner;
    private Coroutine _clearCoroutine;
    private string _currentSessionName;

    private void Awake()
    {
        // Bind button events
        createRoomBtn.onClick.AddListener(OnClickCreateRoom);
        joinRoomBtn.onClick.AddListener(OnClickJoinFromInput);
        exitRoomBtn.onClick.AddListener(OnClickExitRoom);

        // Initialize defaults
        const string defaultName = "TestRoom123";
        if (string.IsNullOrEmpty(roomNameInput.text)) roomNameInput.text = defaultName;
        if (string.IsNullOrEmpty(joinRoomInput.text)) joinRoomInput.text = defaultName;

        RefreshRoomListUI();

        // Ensure runner and update initial player count
        EnsureRunner();
        UpdatePlayerCount(_runner.SessionInfo.IsValid ? _runner.SessionInfo.PlayerCount : 0);
    }

    private void EnsureRunner()
    {
        if (_runner != null) return;
        var go = new GameObject("NetworkRunner");
        _runner = go.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);
        DontDestroyOnLoad(go);
        Debug.Log("[Lobby] NetworkRunner created and callbacks registered");
    }

    public async void OnClickCreateRoom()
    {
        if (_isOperationRunning) return;
        EnsureRunner();

        if (_runner.SessionInfo.IsValid)
        {
            ShowStatus("Already in a session, please exit first.");
            Debug.LogWarning("[Lobby] CreateRoom aborted: already in session");
            return;
        }

        string name = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            ShowStatus("Room name cannot be empty.");
            return;
        }
        if (_roomNames.Contains(name))
        {
            ShowStatus($"Room '{name}' already exists.");
            return;
        }

        _isOperationRunning = true;
        createRoomBtn.interactable = false;

        _currentSessionName = name;
        _roomNames.Add(name);
        RefreshRoomListUI();

        Debug.Log($"[Lobby] Attempting to create room: {name}");
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = name
        });

        if (result.Ok)
        {
            Debug.Log($"[Lobby] Room created successfully: {name}");
            ShowStatus("Room created successfully.");
        }
        else
        {
            Debug.LogError($"[Lobby] Failed to create room: {result.ShutdownReason}");
            ShowStatus($"Failed to create room: {result.ShutdownReason}");
        }

        createRoomBtn.interactable = true;
        _isOperationRunning = false;
    }

    public async void OnClickJoinFromInput()
    {
        if (_isOperationRunning) return;
        EnsureRunner();

        if (_runner.SessionInfo.IsValid)
        {
            ShowStatus("Already in a session, please exit first.");
            Debug.LogWarning("[Lobby] JoinRoom aborted: already in session");
            return;
        }

        string name = joinRoomInput.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            ShowStatus("Join room name cannot be empty.");
            return;
        }
        if (!_roomNames.Contains(name))
        {
            ShowStatus($"Room '{name}' does not exist.");
            Debug.LogWarning($"[Lobby] JoinRoom aborted: '{name}' not in list");
            return;
        }

        _isOperationRunning = true;
        joinRoomBtn.interactable = false;

        _currentSessionName = name;
        Debug.Log($"[Lobby] Attempting to join room: {name}");
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = name
        });

        if (result.Ok)
        {
            Debug.Log($"[Lobby] Joined room successfully: {name}");
            ShowStatus("Joined room successfully.");
        }
        else
        {
            Debug.LogError($"[Lobby] Failed to join room: {result.ShutdownReason}");
            ShowStatus($"Failed to join room: {result.ShutdownReason}");
        }

        joinRoomBtn.interactable = true;
        _isOperationRunning = false;
    }

    public void OnClickExitRoom()
    {
        if (_runner == null || !_runner.SessionInfo.IsValid)
        {
            ShowStatus("No active session to exit.");
            Debug.LogWarning("[Lobby] ExitRoom aborted: no active session");
            return;
        }
        ShowStatus("Exiting room...");
        Debug.Log($"[Lobby] Exiting room: {_currentSessionName}");
        _runner.Shutdown(); // triggers OnShutdown
    }

    private void RefreshRoomListUI()
    {
        for (int i = roomListContent.childCount - 1; i >= 0; i--)
            Destroy(roomListContent.GetChild(i).gameObject);
        foreach (var room in _roomNames)
        {
            var go = Instantiate(roomListItemPrefab, roomListContent);
            var item = go.GetComponent<RoomListItem>();
            item?.SetRoomInfo(room);
        }
    }

    #region UI Helpers
    private void ShowStatus(string message)
    {
        if (statusText == null) return;
        if (_clearCoroutine != null) StopCoroutine(_clearCoroutine);
        statusText.text = message;
        _clearCoroutine = StartCoroutine(ClearAfterDelay(3f));
    }
    private IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        statusText.text = string.Empty;
    }
    private void UpdatePlayerCount(int count)
    {
        if (playerCountText != null)
            playerCountText.text = $"Players online: {count}";
    }
    #endregion

    #region Fusion Callbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("[Lobby] OnConnectedToServer");
        UpdatePlayerCount(runner.ActivePlayers.Count());
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress addr, NetConnectFailedReason reason)
    {
        Debug.LogError($"[Lobby] OnConnectFailed: {reason}");
        ShowStatus($"Connection failed: {reason}");
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Lobby] OnPlayerJoined: {player}");
        UpdatePlayerCount(runner.ActivePlayers.Count());
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Lobby] OnPlayerLeft: {player}");
        UpdatePlayerCount(runner.ActivePlayers.Count());
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        Debug.Log($"[Lobby] OnShutdown: reason={reason}");
        ShowStatus("Exited session.");
        UpdatePlayerCount(0);
        if (!string.IsNullOrEmpty(_currentSessionName))
        {
            _roomNames.Remove(_currentSessionName);
            RefreshRoomListUI();
            _currentSessionName = null;
        }
        Destroy(_runner.gameObject);
        _runner = null;
    }
    // Unused callbacks
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
    #endregion
}