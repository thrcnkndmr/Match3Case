using System;

public static class EventManager
{
   public static event Action OnStartGameEvent;
   public static event Action OnMoveItemEvent;

   public static void OnStartGameInvoker()
   {
      OnStartGameEvent?.Invoke();
   }

   private static void OnMoveItemEventInvoker()
   {
      OnMoveItemEvent?.Invoke();
   }
}
