using System;

public static class MiniSceneSignals
{
    // 小场景完成时触发
    public static event Action OnRequestExit;

    public static void RequestExit()
    {
        OnRequestExit?.Invoke();
    }
}
