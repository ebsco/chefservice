using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;

namespace ChefService
{
    public partial class ChefService : ServiceBase
    {
        ServiceHost sh;
        public ChefService()
        {
            InitializeComponent();
        }

        internal void mystart()
        {
            //sh = new ServiceHost(typeof(ChefService));
            sh = new ServiceHost(typeof(WebService.ChefWebService));
            sh.Open();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Trace.WriteLine("Start Onstart event.");
                mystart();
                Trace.WriteLine("End onStart event.");
            }
            catch (Exception e)
            {
                EventWriter.Write("Exception in On Start Event:" + e, EventLevel.Error, 1234);
                throw;
            }
        }
        protected override void OnStop()
        {
            try
            {
                Trace.WriteLine("Start OnStop event");

                if (sh != null)
                {
                    if (sh.State != CommunicationState.Opened)
                        return;

                    sh.Close();
                    ((IDisposable)sh).Dispose();
                    sh = null;
                }

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
