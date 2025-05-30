using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 列表项：显示房间名并处理按钮点击加入
/// </summary>
public class RoomListItem : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("显示房间名的文本组件")]
    [SerializeField] private TMP_Text roomNameText;

    private string _roomName;

    /// <summary>
    /// 由 LobbyManager 调用，填充房间名并绑定按钮事件
    /// </summary>
    public void SetRoomInfo(string roomName)
    {
        _roomName = roomName;
        if (roomNameText != null)
            roomNameText.text = roomName;

        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnJoinButtonClicked);
        }
        else
        {
            Debug.LogWarning("RoomListItem: 需要挂载 Button 组件");
        }
    }

    /// <summary>
    /// 按钮回调：把房间名写入输入框，并触发加入流程
    /// </summary>
    private void OnJoinButtonClicked()
    {
        var manager = FindObjectOfType<LobbyManager>();
        if (manager == null)
        {
            Debug.LogWarning("RoomListItem: 未找到 LobbyManager");
            return;
        }
        // 同步输入框文本
        if (manager.joinRoomInput != null)
            manager.joinRoomInput.text = _roomName;
        // 调用加入方法
        manager.OnClickJoinFromInput();
    }
}












