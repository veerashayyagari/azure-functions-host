using System.Diagnostics.Tracing;

namespace eventsource
{
  public class LilianEventSource : EventSource
  {
      public static LilianEventSource Log = new LilianEventSource();

      [Event(1, Message = "Lilian: starting up.", Level = EventLevel.Informational)]
      public void Startup() { WriteEvent(1); }

      [Event(2, Message = "Lilian: shutting down.", Level = EventLevel.Warning)]
      public void Shutdown() { WriteEvent(2); }
  }
}