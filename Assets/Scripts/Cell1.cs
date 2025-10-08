using UnityEngine;
using UnityEngine.UI;

public class Cell1 : MonoBehaviour
{
    [Header("展示文字（可选）")]
    public Text UILabel;
    public string DisplayName = "格子";

    [Header("完成方式：指定秒后自动退出，或其他退出方式。要退出调用RequestExit即可")]
    public bool AutoExitAfterSeconds = true;
    public float HoldSeconds = 1f;

    void Start()
    {
        Debug.Log($"Cell1函数");
        if (UILabel != null)
        {
            Debug.Log($"UILable有效");
            UILabel.alignment = TextAnchor.MiddleCenter;
            UILabel.fontSize = 48;
            UILabel.text = $"场景：{DisplayName}";
        }
    
        if (AutoExitAfterSeconds)
            Invoke(nameof(RequestExit), HoldSeconds);
    }

    void RequestExit()
    {
        MiniSceneSignals.RequestExit();
    }
}
