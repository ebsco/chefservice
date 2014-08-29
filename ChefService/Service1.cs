using System.Diagnostics;
using System.ServiceProcess;

namespace ChefService
{
    public partial class ChefService : ServiceBase
    {
        public ChefService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Trace.WriteLine("Start Onstart event.");
            ChefRunner.ChefWrap.PerformChefCall();
            Trace.WriteLine("End onStart event.  Calling Stop");
            this.Stop(); //Stop as soon as we are started.
        }
        protected override void OnStop()
        {
            Trace.WriteLine("Start OnStop event");
            ChefRunner.ChefWrap.ResetRegistryEntry();
            Trace.WriteLine("END OnStop event");
        }
    }
}
