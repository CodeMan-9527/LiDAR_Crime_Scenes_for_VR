using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Fusion.Sockets;
using System.Linq;

/// <summary>
/// Lobby manager: supports single & multi-player create/join/exit sessions,
/// scroll list UI, player count, and 3s status messages.
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
    private Coroutine _clearStatusCoroutine;
    private string _currentSessionName;
    private const string DefaultRoomName = "TestRoom123";

    void Awake()
    {
        createRoomBtn.onClick.AddListener(OnClickCreateRoom);
        joinRoomBtn.onClick.AddListener(OnClickJoinRoom);
        exitRoomBtn.onClick.AddListener(OnClickExitRoom);

        // preload default session names
        if (roomNameInput != null && string.IsNullOrEmpty(roomNameInput.text))
            roomNameInput.text = DefaultRoomName;
        if (joinRoomInput != null && string.IsNullOrEmpty(joinRoomInput.text))
            joinRoomInput.text = DefaultRoomName;

        RefreshRoomListUI();
        EnsureRunner();
        UpdatePlayerCount(0);
    }

    private void EnsureRunner()
    {
        if (_runner != null) return;
        var go = new GameObject("NetworkRunner");
        _runner = go.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);
        DontDestroyOnLoad(go);
        Debug.Log("[Lobby] Runner created");
    }

    public async void OnClickCreateRoom()
    {
        if (_isOperationRunning) return;
        EnsureRunner();
        if (_runner.SessionInfo.IsValid)
        {
            ShowStatus("Already in a session, exit first.");
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

        Debug.Log($"[Lobby] Creating room: {name}");
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = name
        });
        if (result.Ok)
        {
            Debug.Log($"[Lobby] Created & joined: {name}");
            ShowStatus("Room created & joined.");
        }
        else
        {
            Debug.LogError($"[Lobby] Create failed: {result.ShutdownReason}");
            _roomNames.Remove(name);
            RefreshRoomListUI();
            ShowStatus($"Create failed: {result.ShutdownReason}");
        }

        createRoomBtn.interactable = true;
        _isOperationRunning = false;
    }

    public async void OnClickJoinRoom()
    {
        if (_isOperationRunning) return;
        EnsureRunner();
        if (_runner.SessionInfo.IsValid)
        {
            ShowStatus("Already in a session, exit first.");
            return;
        }
        string name = joinRoomInput.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            ShowStatus("Room name cannot be empty.");
            return;
        }
        // allow join even if not in list
        _isOperationRunning = true;
        joinRoomBtn.interactable = false;

        _currentSessionName = name;
        if (!_roomNames.Contains(name))
        {
            _roomNames.Add(name);
            RefreshRoomListUI();
        }

        Debug.Log($"[Lobby] Joining room: {name}");
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = name
        });
        if (result.Ok)
        {
            Debug.Log($"[Lobby] Joined: {name}");
            ShowStatus("Joined room.");
        }
        else
        {
            Debug.LogError($"[Lobby] Join failed: {result.ShutdownReason}");
            ShowStatus($"Join failed: {result.ShutdownReason}");
            _roomNames.Remove(name);
            RefreshRoomListUI();
        }

        joinRoomBtn.interactable = true;
        _isOperationRunning = false;
    }

    public void OnClickExitRoom()
    {
        if (_isOperationRunning) return;
        if (_runner == null || !_runner.SessionInfo.IsValid)
        {
            ShowStatus("No active session.");
            return;
        }
        ShowStatus("Exiting room...");
        Debug.Log($"[Lobby] Exiting: {_currentSessionName}");
        _runner.Shutdown();
    }

    private void RefreshRoomListUI()
    {
        if (roomListContent == null || roomListItemPrefab == null) return;
        for (int i = roomListContent.childCount - 1; i >= 0; i--)
            Destroy(roomListContent.GetChild(i).gameObject);
        foreach (var r in _roomNames)
        {
            var go = Instantiate(roomListItemPrefab, roomListContent);
            go.GetComponent<RoomListItem>()?.SetRoomInfo(r);
        }
    }

    private void ShowStatus(string message)
    {
        if (statusText == null) return;
        if (_clearStatusCoroutine != null) StopCoroutine(_clearStatusCoroutine);
        statusText.text = message;
        _clearStatusCoroutine = StartCoroutine(ClearStatusAfter(3f));
    }
    private IEnumerator ClearStatusAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (statusText != null) statusText.text = string.Empty;
    }

    private void UpdatePlayerCount(int count)
    {
        if (playerCountText != null)
            playerCountText.text = $"Players online: {count}";
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        if (runner != _runner) return;
        UpdatePlayerCount(runner.ActivePlayers.Count());
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner != _runner) return;
        UpdatePlayerCount(runner.ActivePlayers.Count());
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner != _runner) return;
        UpdatePlayerCount(runner.ActivePlayers.Count());
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        if (runner != _runner) return;
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
        _isOperationRunning = false;
    }
    // Unused callbacks
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest req, byte[] tok) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress addr, NetConnectFailedReason reason) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnUserSimulationMessage(NetworkRunner runner, Fusion.SimulationMessagePtr msg) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef p) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef p) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey channel, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey channel, float prog) { }

}
