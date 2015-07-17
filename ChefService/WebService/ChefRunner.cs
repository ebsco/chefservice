using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ChefService.WebService
{
    //Only one instance of this class will be created at once
    public class ChefRunner : IDisposable
    {
        private static object lockobj = new object();
        private static object disposelockobj = new object();
        private const int MaxOutputLinesReturned = 50;
        private Process ps;

        //All interactions with the queue need to be thread safe.  
        //The ConcurrentQueue would require .NET 4.0 -until only 2k12 support, will not happen.  So create very quick thread safe methods below
        private Queue<string> queue = new Queue<string>();
        private string ChefArgs;

        /// <summary>
        /// Constructor for class to execut chef-client
        /// </summary>
        /// <param name="someChefArgs">Args to be passed to the chef-client command</param>
        public ChefRunner(string someChefArgs)
        {
            ChefArgs = someChefArgs;
        }

        /// <summary>
        /// Determine if the process has exited
        /// </summary>
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

        /// <summary>
        /// Get the process output.  Will not give all output, only up to the maximum lines.
        /// </summary>
        public ProcessOutput Output
        {
            get
            {
                ProcessOutput output = new ProcessOutput();
                //EventWriter.Write("Getting Process Output", EventLevel.Information);

                while (GetQueueCountSafely() > 0 && output.Lines.Count <= MaxOutputLinesReturned)
                {
                   var str=DequeueSafely();
                   output.Lines.Add(str);
                }

                //EventWriter.Write("Returning Process Output", EventLevel.Information);
                return output;
            }
        }

        /// <summary>
        /// Get the exit code of chef-client run
        /// </summary>
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

        /// <summary>
        /// Starts the chef-client process on a seperate thread.
        /// </summary>
        public void StartChef()
        {
            if (ps == null)
            {
                StartThread();
            }
            else
            {
                throw new EntryPointNotFoundException("Invalid call to start chef - already running");
            }
        }


        public void Kill()
        {
            if (ps != null)
            {
                EventWriter.Write("Found Chef-client still running, configured to kill", EventLevel.Warning, 1);

                try
                {
                    Processes.KillProcessTree(ps.Id, true);
                }
                catch (System.ComponentModel.Win32Exception e)
                {
                    EventWriter.Write("Win32Exception when killing chef-client:"+e, EventLevel.Warning, 1);
                    //     The associated process could not be terminated. -or- The process is terminating.
                    //      -or- The associated process is a Win16 executable.
                }
                catch (System.InvalidOperationException e)
                {
                    EventWriter.Write("InvalidOperationException when killing chef-client:" + e, EventLevel.Warning, 1);
                    //     The process has already exited. -or- There is no process associated with
                    //     this System.Diagnostics.Process object.
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            lock (disposelockobj)
            {
                if (ps != null)
                {
                    if (!ps.HasExited)
                    {
                        Console.WriteLine("Detected Chef-client not done running yet, waiting for it to finish");
                        ps.WaitForExit();
                    }

                    try
                    {
                        int count = GetQueueCountSafely();
                        //If the full output hasnt been fully read, then throw an exception.  This usually would mean something bad happened in client code, or a user interrupted a chef-client run.
                        if (count != 0)
                        {
                            queue.Clear();
                            throw new ImproperException("The output has not been fully read.  Please investigate the client code, because there are at least " + count + " lines left to read.  Clearing for now.");
                        }
                    }
                    finally
                    {
                        ps.Dispose();
                        ps = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the total queue count in a thread safe way.
        /// </summary>
        /// <returns></returns>
        private int GetQueueCountSafely()
        {
            lock (lockobj)
            {
                return queue.Count;
            }
        }

        /// <summary>
        /// Dequeues a string from the process output queue in a thread safe way.
        /// </summary>
        /// <returns></returns>
        private string DequeueSafely()
        {
            lock (lockobj)
            {
                return queue.Dequeue();
            }
        }

        /// <summary>
        /// Enqueue a line of output from the chef-client run in a thread safe way
        /// </summary>
        /// <param name="arg"></param>
        private void EnqueueSafely(string arg)
        {
            lock (lockobj)
            {
                queue.Enqueue(arg);
            }
        }

        /// <summary>
        /// Starts the thread to run chef-client
        /// </summary>
        private void StartThread()
        {
            //Refresh environment variable path
            //Make sure my process path is updated with latest path variables
            //This was a bug in Chef-client installer.  The path isnt updated after a bootstrap
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine), EnvironmentVariableTarget.Process);
            EnqueueSafely("Machine PATH:" + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine));
            EnqueueSafely("Process PATH:" + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process));


            ps = new Process();
            ps.StartInfo = new ProcessStartInfo();
            
            try
            {
                ps.StartInfo.FileName = FindFilePath("chef-client.bat");
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine("Couldn't find chef-client");
                EventWriter.Write("Exception finding Chef-Client:" + fnfe, EventLevel.Error);
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

            try
            {
                EnqueueSafely("Starting chef process");
                ps.Start();
                ps.BeginOutputReadLine();
                ps.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                EventWriter.Write("Exception running Chef-Client:" + e, EventLevel.Error, 3);
                throw e;
            }
        }

        /// <summary>
        /// Process redirected output into the queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                // append the new data to the data already read-in
                EnqueueSafely(e.Data);
                //Console.WriteLine(e.Data);
            }
        }

        /// <summary>
        /// Find the chef-client file path via the PATH environment variable.
        /// TODO: Might want to change this to be config driven instead
        /// </summary>
        /// <param name="exe"></param>
        /// <returns></returns>
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
    }
}