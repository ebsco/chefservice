using System.Diagnostics;

namespace ChefService
{
    static class EventWriter
    {
        private static void RegisterEventSource()
        {
            if (!EventLog.SourceExists(ChefServiceInstallerDefinition.ChefServiceName))
                EventLog.CreateEventSource(ChefServiceInstallerDefinition.ChefServiceName, "Application");
        }

        public static void Write(string Message, EventLevel theLevel = EventLevel.Information, int SpecificID = 1)
        {
            switch (theLevel)
            {
                case EventLevel.Error:
                    EventLog.WriteEntry(ChefServiceInstallerDefinition.ChefServiceName, Message, EventLogEntryType.Error, SpecificID);
                    break;

                case EventLevel.Information:
                    EventLog.WriteEntry(ChefServiceInstallerDefinition.ChefServiceName, Message, EventLogEntryType.Information, SpecificID);
                    break;

                case EventLevel.Warning:
                    EventLog.WriteEntry(ChefServiceInstallerDefinition.ChefServiceName, Message, EventLogEntryType.Warning, SpecificID);
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