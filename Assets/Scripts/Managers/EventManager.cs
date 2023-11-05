using System;

public static class EventManager
{
    public static event Action OnStartGameEvent;
    public static event Action OnMovedItem;
    public static event Action OnFindMatch;

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
}