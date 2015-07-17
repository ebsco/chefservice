using System.Diagnostics;

namespace ChefService
{
    /// <summary>
    /// Write events to the event viewer
    /// </summary>
    static class EventWriter
    {
        private static void RegisterEventSource()
        {
            if (!EventLog.SourceExists(ChefServiceInstallerDefinition.ChefServiceName))
                EventLog.CreateEventSource(ChefServiceInstallerDefinition.ChefServiceName, "Application");
        }

        /// <summary>
        /// Wrapper around writing events
        /// </summary>
        /// <param name="Message">Message to write</param>
        /// <param name="theLevel">The EventLevel</param>
        /// <param name="SpecificID"></param>
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