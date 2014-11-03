﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ChefService.WebService
{
    public class ChefRunner : IDisposable
    {
        const int MaxOutputLinesReturned = 200;
        Process ps;
        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
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

        public ProcessOutput Output
        {
            get
            {
                ProcessOutput output = new ProcessOutput();
                string outstring;
                int i = 0;

                while (queue.TryDequeue(out outstring))
                {
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


        public void StartChefThread()
        {
            if (ps == null)
            {
                Thread a = new Thread(new ThreadStart(StartChef));
                a.IsBackground = true;
                a.Start();
            }
            else if (ps != null && ps.HasExited)
            {
                Thread a = new Thread(new ThreadStart(StartChef));
                a.IsBackground = true;
                a.Start();
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
                queue.Enqueue(e.Data);
                //Console.WriteLine(e.Data);
            }
        }

        private static string FindFilePath(string exe)
        {
            //Make sure my process path is updated with latest path variables
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine), EnvironmentVariableTarget.Process);

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
            ps = new Process();
            //using (ps = new Process())
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

                ps.StartInfo.LoadUserProfile = false;



                Console.WriteLine("Starting chef process");

                try
                {
                    ps.Start();
                    ps.BeginOutputReadLine();
                    ps.BeginErrorReadLine();
                    ps.WaitForExit();

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
                ps.Dispose();
                ps = null;
            }
        }
    }
}