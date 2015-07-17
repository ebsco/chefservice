using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;

namespace ChefService
{
    /// <summary>
    /// Handles the logic of the Service Manager events.
    /// </summary>
    public partial class ChefService : ServiceBase
    {
        ServiceHost sh;
        public ChefService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starts the Service Host for the WebService
        /// </summary>
        internal void StartWebService()
        {
            sh = new ServiceHost(typeof(WebService.ChefWebService));
            sh.Open();
        }

        /// <summary>
        /// Stops the ServiceHost for the WebService
        /// </summary>
        internal void StopWebService()
        {
            if (sh != null)
            {
                if (sh.State != CommunicationState.Opened)
                    return;

                sh.Close();
                ((IDisposable)sh).Dispose();
                sh = null;
            }
        }

        /// <summary>
        /// Starts the webservice as part of service start.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                Trace.WriteLine("Start Onstart event.");
                StartWebService();
                Trace.WriteLine("End onStart event.");
            }
            catch (Exception e)
            {
                EventWriter.Write("Exception in On Start Event:" + e, EventLevel.Error, 1234);
                throw;
            }
        }

        /// <summary>
        /// Stops the web service.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                Trace.WriteLine("Start OnStop event");
                StopWebService();
                Trace.WriteLine("END OnStop event");
            }
            catch (Exception e)
            {
                EventWriter.Write("Failed in on Stop - " + e, EventLevel.Error, 3);
                throw;
            }
        }
    }
}
