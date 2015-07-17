using System.Diagnostics;

namespace ChefService
{
    /// <summary>
    /// Write events to the event viewer
    /// </summary>
    public static class EventWriter
    {
        private static void RegisterEventSource(string AppName = ChefServiceInstallerDefinition.ChefServiceName)
        {
            if (!EventLog.SourceExists(AppName))
                EventLog.CreateEventSource(AppName, "Application");
        }

        /// <summary>
        /// Wrapper around writing events
        /// </summary>
        /// <param name="Message">Message to write</param>
        /// <param name="theLevel">The EventLevel</param>
        /// <param name="SpecificID"></param>
        public static void Write(string Message, EventLevel theLevel = EventLevel.Information, int SpecificID = 1, string AppName = ChefServiceInstallerDefinition.ChefServiceName)
        {
            RegisterEventSource(AppName);
            switch (theLevel)
            {
                case EventLevel.Error:
                    EventLog.WriteEntry(AppName, Message, EventLogEntryType.Error, SpecificID);
                    break;

                case EventLevel.Information:
                    EventLog.WriteEntry(AppName, Message, EventLogEntryType.Information, SpecificID);
                    break;

                case EventLevel.Warning:
                    EventLog.WriteEntry(AppName, Message, EventLogEntryType.Warning, SpecificID);
                    break;
            }
        }
    }

    /// <summary>
    /// Which Levels of an event that we want to support
    /// </summary>
    public enum EventLevel
    {
        Error,
        Information,
        Warning
    }
}