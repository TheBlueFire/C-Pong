using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Blue_Debugger
{
    public class Debug
    {
        private List<string> toLog { get; set; }
        private List<string> toRemove { get; set;  }
        public string LogFile { get; set; }
        private bool doLog { get; set; }
        private bool doOK { get; set; }
        private bool doInfo { get; set; }
        private bool doFail { get; set; }
        private Thread logThread { get; set; }
        public Debug(string logfile, bool writeToFile = true, bool writeOK = true, bool writeInfo = true, bool writeFail = true)
        {
            this.toLog = new List<string>();
            this.toRemove = new List<string>();
            this.LogFile = logfile;
            this.doLog = writeToFile;
            this.doOK = writeOK;
            this.doInfo = writeInfo;
            this.doFail = writeFail;
            this.StatusMessage("OK", "-------------" + DateTime.Now.ToString("h:mm:ss tt") + "-------------");
            this.logThread = new Thread(this.Log);
            this.logThread.Start();
            this.StatusMessage("OK", "Debugger Successfully loaded");
        }
        public void Log()
        {
            this.StatusMessage("OK", "Debug Thread Successfully started");
            while (true)
            {
                try
                {
                    foreach (string log in this.toLog)
                    {
                        if (!File.Exists(this.LogFile))
                        {
                            using (StreamWriter sw = File.CreateText(this.LogFile))
                            {
                                sw.WriteLine(log);
                            }
                        }
                        else
                        {
                            using (StreamWriter sw = File.AppendText(this.LogFile))
                            {
                                sw.WriteLine(log);
                            }
                        }
                        this.toRemove.Add(log);
                    }
                    Thread.Sleep(100);
                    foreach (string toDelete in this.toRemove)
                    {
                        this.toLog.Remove(toDelete);
                    }
                    this.toRemove.Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(100);
            }
        }
        public void LogToFile(string log)
        {
            this.toLog.Add(log);
        }
        public void StatusMessage(string status, string message, bool logOnly = false)
        {
            try
            {
                switch (status)
                {
                    case "OK":
                        if(!logOnly)
                        {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("   OK   ");
                            Console.ResetColor();
                            Console.WriteLine("] " + message);
                        }
                        LogToFile(DateTime.Now.ToString("h:mm:ss tt") + "|  OK  | " + message);
                        break;
                    case "INFO":
                        if (!logOnly)
                        {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("  INFO  ");
                            Console.ResetColor();
                            Console.WriteLine("] " + message);
                        }
                        LogToFile(DateTime.Now.ToString("h:mm:ss tt") + "| INFO | " + message);
                        break;
                    case "ERROR":
                        if (!logOnly)
                        {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("  FAIL  ");
                            Console.ResetColor();
                            Console.WriteLine("] " + message);
                        }
                        LogToFile(DateTime.Now.ToString("h:mm:ss tt") + "| FAIL | " + message);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("UNHANDELD EXCEPTION - CRITICAL - " + e.ToString());
            }
        }
    }
}
