using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardControllerManual : MonoBehaviour
{
    public List<Tile> Tiles = new List<Tile>();
    public Transform Player;
    public float MoveTimePerStep = 0.25f;
    public float YOffset = 0.4f;

    private int _index = 0;
    private bool _isMoving = false;
    private bool _miniSceneOpen = false;
    private string _loadedMiniSceneName = null;

    void Awake()
    {
        Debug.Log($"Board Awake函数");
        if (Tiles.Count > 0 && Player != null)
            Player.position = Tiles[_index].transform.position + Vector3.up * YOffset;

        // 订阅：小场景主动“请求退出”
        MiniSceneSignals.OnRequestExit += HandleMiniSceneRequestExit;

        // 兜底：如果有人在别处卸载了，也把状态复位
        SceneManager.sceneUnloaded += (s) =>
        {
            if (s.name == _loadedMiniSceneName)
            {
                _loadedMiniSceneName = null;
                _miniSceneOpen = false;
            }
        };
    }

    void OnDestroy()
    {
        MiniSceneSignals.OnRequestExit -= HandleMiniSceneRequestExit;
    }

    void Update()
    {
        if (_miniSceneOpen || _isMoving) return;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("update 条件满足");
            StartCoroutine(StepAndMaybeEnter());
        }
    }

    IEnumerator StepAndMaybeEnter()
    {
        if (Tiles.Count == 0 || Player == null) yield break;

        _isMoving = true;
        int next = (_index + 1) % Tiles.Count;
        Vector3 from = Player.position;
        Vector3 to = Tiles[next].transform.position + Vector3.up * YOffset;
        Debug.Log($"Board函数");
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(MoveTimePerStep, 0.0001f);
            Player.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        Player.position = to;
        _index = next;
        _isMoving = false;

        var tile = Tiles[_index];
        if (!string.IsNullOrEmpty(tile.SceneName))
        {
            Debug.Log($"有Scene Name");
            _miniSceneOpen = true;
            _loadedMiniSceneName = tile.SceneName;

            var op = SceneManager.LoadSceneAsync(tile.SceneName, LoadSceneMode.Additive);
            while (!op.isDone) yield return null;
            // 等待小场景发“完成”信号期间，棋盘不做事
        }
    }

    private void HandleMiniSceneRequestExit()
    {
        if (!_miniSceneOpen || string.IsNullOrEmpty(_loadedMiniSceneName)) return;
        StartCoroutine(UnloadMiniSceneAndResume());
    }

    IEnumerator UnloadMiniSceneAndResume()
    {
        var op = SceneManager.UnloadSceneAsync(_loadedMiniSceneName);
        while (op != null && !op.isDone) yield return null;
        _loadedMiniSceneName = null;
        _miniSceneOpen = false; // 现在可以继续点击前进了
        Debug.Log($"可以继续点击");
    }
}
