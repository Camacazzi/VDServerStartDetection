using System;
using System.Diagnostics;
using System.Threading;
using System.Management;

namespace VDServerStartDetection
{
    class WMIEvent
    {
        public bool IsRunning = false;

        public static void Main()
        {
            WMIEvent we = new WMIEvent();
            ManagementEventWatcher w = null;
            WqlEventQuery q;
            try
            {
                q = new WqlEventQuery();
                q.EventClassName = "Win32_ProcessStartTrace";
                w = new ManagementEventWatcher(q);
                w.EventArrived += new EventArrivedEventHandler(we.ProcessStartEventArrived);
                w.Start();
                Console.ReadLine(); // block main thread for test purposes
            }
            finally
            {
                w.Stop();
            }
        }

        public void ProcessStartEventArrived(object sender, EventArrivedEventArgs e)
        {
            foreach (PropertyData pd in e.NewEvent.Properties)
            {
                if(pd.Value?.ToString() == "VirtualDesktop.Server.exe")
                {
                    Console.WriteLine("Virtual Desktop connection detected. Running SteamVR.");
                    using (Process p = new Process())
                    {
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.FileName = "C:\\Program Files (x86)\\Steam\\steam.exe";
                        p.StartInfo.Arguments = "steam://rungameid/250820";
                        Thread.Sleep(2000);
                        p.Start();
                        IsRunning = true;
                        p.WaitForExit();
                        IsRunning = false;
                    }
                    break;
                }
            }
        }
    }
}
