using UnityEngine;
using Meta.XR.MultiplayerBlocks.Shared;
using System.Threading.Tasks;

public class SimpleCustomMatchmaker : MonoBehaviour, CustomMatchmaking.ICustomMatchmakingBehaviour
{
    public string ConnectedRoomToken => "";
    public bool IsConnected => true;
    public bool SupportsRoomPassword => false;

    public Task<CustomMatchmaking.RoomOperationResult> CreateRoom(CustomMatchmaking.RoomCreationOptions options)
    {
        var result = new CustomMatchmaking.RoomOperationResult { RoomToken = "mock_token" };
        return Task.FromResult(result);
    }

    public Task<CustomMatchmaking.RoomOperationResult> JoinRoom(string roomToken, string roomPassword = null)
    {
        var result = new CustomMatchmaking.RoomOperationResult { RoomToken = roomToken };
        return Task.FromResult(result);
    }

    public Task<CustomMatchmaking.RoomOperationResult> JoinOpenRoom(string lobbyName)
    {
        var result = new CustomMatchmaking.RoomOperationResult { RoomToken = "lobby_token" };
        return Task.FromResult(result);
    }

    public void LeaveRoom()
    {
        // No-op
    }
}
