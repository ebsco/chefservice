using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace ChefService.WebService
{
    public class ChefRunner : IDisposable
    {
        static object lockobj = new object();
        const int MaxOutputLinesReturned = 50;
        Process ps;
        Queue<string> queue = new Queue<string>();
        string ChefArgs;

        public ChefRunner(string someChefArgs)
        {
            ChefArgs = someChefArgs;
        }

        public bool HasExited
        {
            get
            {
                if (ps != null)
                {
                    return ps.HasExited;
                }

                throw new NotSupportedException();
            }
        }

        //Make the part of the queue I need thread safe.
        private int GetQueueCountSafely()
        {
            lock (lockobj)
            {
                return queue.Count;
            }
        }
        private string DequeueSafely()
        {
            lock (lockobj)
            {
                return queue.Dequeue();
            }
        }
        private void EnqueueSafely(string arg)
        {
            lock (lockobj)
            {
                queue.Enqueue(arg);
            }
        }

        public ProcessOutput Output
        {
            get
            {
                ProcessOutput output = new ProcessOutput();
                string outstring;
                int i = 0;

                //while (queue.TryDequeue(out outstring))
                while (GetQueueCountSafely() > 0)
                {
                    outstring = DequeueSafely();

                    i++;
                    output.Lines.Add(outstring);
                    if (i >= MaxOutputLinesReturned)
                    {
                        break;
                    }
                }
                return output;
            }
        }

        public int ExitCode
        {
            get
            {
                if (ps != null)
                {
                    if (ps.HasExited)
                    {
                        return ps.ExitCode;
                    }
                    else
                    {
                        throw new Exception("not exited yet");
                    }
                }

                throw new NotSupportedException();
            }
        }

        private void WaitForProcessStart()
        {
            EnqueueSafely("Waiting for Chef Process to Start");
            bool started = false;
            int totalsleep = 0;
            while (!started)
            {
                try
                {
                    if (ps != null)
                    {
                        bool abc = ps.HasExited;
                        EnqueueSafely("Process Started");
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                        totalsleep++;
                    }
                }
                catch
                {
                    Thread.Sleep(100);
                    totalsleep++;
                }
                //If the process hasnt started in 10 seconds error out
                if (totalsleep > 100)
                {
                    EnqueueSafely("Process Failed to start in 10 seconds");
                    throw new Exception("Waited for 10 seconds for process to start, and it never did");
                }
            }
        }
        private void Startme()
        {
            Thread a = new Thread(new ThreadStart(StartChef));
            a.IsBackground = true;
            a.Start();

            WaitForProcessStart();
        }

        public void StartChefThread()
        {
            if (ps == null)
            {
                Startme();
            }
            else if (ps != null && ps.HasExited)
            {
                Startme(); // TODO Maybe remove
            }
            else
            {
                throw new EntryPointNotFoundException("Invalid call to start chef - already running");
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                // append the new data to the data already read-in
                EnqueueSafely(e.Data);
                //Console.WriteLine(e.Data);
            }
        }

        private static string FindFilePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }

        private void StartChef()
        {
            //Refresh environment variable path
            //Make sure my process path is updated with latest path variables
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine), EnvironmentVariableTarget.Process);
            EnqueueSafely("Machine PATH:"+Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine));
            EnqueueSafely("Process PATH:" + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process));


            ps = new Process();
            {
                ps.StartInfo = new ProcessStartInfo();
                try
                {
                    ps.StartInfo.FileName = FindFilePath("chef-client.bat");
                }
                catch (FileNotFoundException fnfe)
                {
                    Console.WriteLine("Couldn't find chef-client");
                    EventWriter.Write("Exception finding Chef-Client:" + fnfe, EventLevel.Error, 2);
                    throw fnfe;
                }

                //TESTING
                //ps.StartInfo.FileName = "powershell";
                //ps.StartInfo.Arguments = "$i=0; while($true){ write-host Test;  $i++; if($i -gt 4000){break;}}";

                ps.StartInfo.Arguments = ChefArgs;

                ps.StartInfo.UseShellExecute = false;
                ps.StartInfo.RedirectStandardError = true;
                ps.StartInfo.RedirectStandardOutput = true;
                ps.OutputDataReceived += DataReceived;
                ps.ErrorDataReceived += DataReceived;
                ps.StartInfo.CreateNoWindow = true;
                ps.StartInfo.LoadUserProfile = false;



                Console.WriteLine("Starting chef process");

                try
                {
                    ps.Start();
                    ps.BeginOutputReadLine();
                    ps.BeginErrorReadLine();
                    ps.WaitForExit();
                    Console.WriteLine("Chef process finished");

                }
                catch (Exception e)
                {
                    EventWriter.Write("Exception running Chef-Client:" + e, EventLevel.Error, 3);
                    throw e;
                }
                Console.WriteLine();
            }


        }

        public void Dispose()
        {
            if (ps != null)
            {
                if (!ps.HasExited)
                {
                    Console.WriteLine("Detected Chef-client not done running yet, waiting for it to finish");
                    ps.WaitForExit();
                }
                int count = GetQueueCountSafely();
                if (count != 0)
                {
                    queue.Clear();
                    throw new Exception("The output has not been fully read.  Please investigate the client code, because there are at least " + count + " lines left to read");
                }
                ps.Dispose();
                ps = null;
            }
        }
    }
}