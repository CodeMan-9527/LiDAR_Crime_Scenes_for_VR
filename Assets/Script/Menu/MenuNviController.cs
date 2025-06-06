using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls navigation using existing layers: firstLayerUI, secondLayerUI, roomLayerUi.
/// </summary>
public class MenuNavigationController : MonoBehaviour
{
    [Header("UI Layers")]
    [Tooltip("Primary menu layer containing LoadListBtn and OpenCreateRoomBtn")]
    public GameObject firstLayerUI;
    [Tooltip("Secondary layer for load-model UI")]
    public GameObject secondLayerUI;
    [Tooltip("Secondary layer for create-room UI")]
    public GameObject roomLayerUi;

    [Header("Main Menu Buttons in firstLayerUI")]
    [Tooltip("Button under firstLayerUI to open load-model layer")]
    public Button loadListBtn;
    [Tooltip("Button under firstLayerUI to open create-room layer")]
    public Button openCreateRoomBtn;

    [Header("Back Buttons in each secondary layer")]
    [Tooltip("Back button under secondLayerUI to return to firstLayerUI")]
    public Button backFromLoadBtn;
    [Tooltip("Back button under roomLayerUi to return to firstLayerUI")]
    public Button backFromRoomBtn;

    private void Awake()
    {
        // Bind navigation from firstLayerUI
        loadListBtn.onClick.AddListener(() => ShowLayer(secondLayerUI));
        openCreateRoomBtn.onClick.AddListener(() => ShowLayer(roomLayerUi));

        // Bind back buttons
        backFromLoadBtn.onClick.AddListener(() => ShowLayer(firstLayerUI));
        backFromRoomBtn.onClick.AddListener(() => ShowLayer(firstLayerUI));
    }

    private void Start()
    {
        // Initialize by showing only the first layer
        ShowLayer(firstLayerUI);
    }

    /// <summary>
    /// Activates the specified layer and deactivates others.
    /// </summary>
    private void ShowLayer(GameObject layer)
    {
        firstLayerUI.SetActive(layer == firstLayerUI);
        secondLayerUI.SetActive(layer == secondLayerUI);
        roomLayerUi.SetActive(layer == roomLayerUi);
    }
}


