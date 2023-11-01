using System;

public static class EventManager
{
   public static event Action OnStartGameEvent;

   public static void OnStartGameInvoker()
   {
      OnStartGameEvent?.Invoke();
   }
}
