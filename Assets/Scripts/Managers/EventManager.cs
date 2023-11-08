using System;

public static class EventManager
{
    public static event Action OnStartGameEvent;
    public static event Action OnLevelSuccess;
    public static event Action OnLevelFail;
    public static event Action OnNextLevel;
    public static event Action OnLevelStart;
    public static event Action OnMovedItem;
    public static event Action OnFindMatch;

    public static event Action OnRestartLevel;

    public static void OnStartGameInvoker()
    {
        OnStartGameEvent?.Invoke();
    }


    public static void OnFindMatchInvoker()
    {
        OnFindMatch?.Invoke();
    }

    public static void OnMovedItemEventInvoker()
    {
        OnMovedItem?.Invoke();
    }

    public static void OnLevelStartInvoker()
    {
        OnLevelStart?.Invoke();
    }

    public static void OnLevelFailInvoker()
    {
        OnLevelFail?.Invoke();
    }

    public static void OnLevelSuccessInvoker()
    {
        OnLevelSuccess?.Invoke();
    }

    public static void OnNextLevelInvoker()
    {
        OnNextLevel?.Invoke();
    }

    public static void OnRestartLevelInvoker()
    {
        OnRestartLevel?.Invoke();
    }
}