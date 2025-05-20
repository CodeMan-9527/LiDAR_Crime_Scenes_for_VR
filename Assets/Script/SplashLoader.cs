using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLoader : MonoBehaviour
{
    public float delay = 3.0f; // 显示Logo持续时间（秒）
    public string nextSceneName = "MainScene"; // 替换为你的主场景名称

    void Start()
    {
        Invoke("LoadNextScene", delay);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
