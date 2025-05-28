using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 列表项：点击后自动填写并触发加入流程
/// </summary>
public class RoomListItem : MonoBehaviour
{
    public TMP_Text roomNameText;
    private string _roomName;

    /// <summary>刷新条目时调用，赋值并绑定按钮</summary>
    public void SetRoomInfo(string roomName)
    {
        _roomName = roomName;
        if (roomNameText)
            roomNameText.text = roomName;

        var btn = GetComponent<Button>();
        if (btn)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnJoinButtonClicked);
        }
    }

    private void OnJoinButtonClicked()
    {
        var manager = FindObjectOfType<LobbyManager>();
        if (manager == null) return;

        // 填充 join 输入框并调用加入
        if (manager.joinRoomInput != null)
            manager.joinRoomInput.text = _roomName;
        manager.OnClickJoinRoom();
    }
}



