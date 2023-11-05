using System;

public static class EventManager
{
    public static event Action OnStartGameEvent;
    public static event Action OnMoveItemEvent;
    public static event Action OnFindMatch;

    public static void OnStartGameInvoker()
    {
        OnStartGameEvent?.Invoke();
    }

    public static void OnFindMatchInvoker()
    {
        OnFindMatch?.Invoke();
    }

    private static void OnMoveItemEventInvoker()
    {
        OnMoveItemEvent?.Invoke();
    }
}