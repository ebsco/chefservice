using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;

namespace ChefService.WebService
{
    /// <summary>
    /// Process manager class
    /// </summary>
    public static class Processes
    {
        public static void KillProcessTree(int pid, bool IncludingParent)
        {
            using (var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid))
            using (ManagementObjectCollection moc = searcher.Get())
            {
                foreach (ManagementObject mo in moc)
                {
                    KillProcessTree(Convert.ToInt32(mo["ProcessID"]), true);
                }
                if (IncludingParent)
                {
                    try
                    {

                        Process proc = Process.GetProcessById(pid);
                        KillProcess(proc);
                    }
                    catch (ArgumentException)
                    { /* process already exited */ }
                }
            }
        }


        private static void KillProcess(Process p)
        {
            try
            {
                p.Kill();
            }
            catch (Exception e)
            {
                if (e as OutOfMemoryException != null && e as InsufficientMemoryException == null
                    || e as ThreadAbortException != null
                    || e as AccessViolationException != null
                    || e as SEHException != null
                    || e as StackOverflowException != null)
                {
                    throw;
                }
            }
        }
    }
}
