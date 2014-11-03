using System.Diagnostics;

namespace ChefService
{
    static class EventWriter
    {
        const string EventName = "ChefService";
        private static void RegisterEventSource()
        {
            if (!EventLog.SourceExists(EventName))
                EventLog.CreateEventSource(EventName, "Application");
        }

        public static void Write(string Message, EventLevel theLevel = EventLevel.Information, int SpecificID = 1)
        {
            switch(theLevel){
                case EventLevel.Error:
                    EventLog.WriteEntry(EventName, Message, EventLogEntryType.Error, SpecificID);
                    break;

                case EventLevel.Information:
                    EventLog.WriteEntry(EventName, Message, EventLogEntryType.Information, SpecificID);
                    break;

                case EventLevel.Warning:
                    EventLog.WriteEntry(EventName, Message, EventLogEntryType.Warning, SpecificID);
                    break;
            }
        }

        
    }
    public enum EventLevel
    {
        Error,
        Information,
        Warning
    }
}
