using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using HidLibrary;

namespace WBR
{
    /// <summary>
    /// Handles input from a HID device based on Vendor and Product ID
    /// </summary>
    public class DeviceHandler
    {
        private Device device;
        private List<HidDevice> devices;
        private List<Thread> Threads;
        private bool Abort = false;

        public DeviceHandler(int vid, int pid, string name)
        {
            device = new Device(vid, pid, name);
        }

        public void Stop()
        {
            Abort = true;
            if (Threads == null) return;
            for(int i = 0; i < Threads.Count; i++)
            {
                if (Threads[i] != null) Threads[i].Interrupt();
                Threads.RemoveAt(i);
            }
        }

        /// <summary>
        /// Initializes HID device and opens a thread for each "sub-hid"
        /// </summary>
        public void Init()
        {
            Abort = false;
            devices = HidDevices.Enumerate(device.Vid, device.Pid).ToList();

            for (int i = 0; i < devices.Count(); i++)
            {
                HidDevice device = devices[i];
                if (device == null)
                {
                    devices.RemoveAt(i);
                    break;
                }

                device.OpenDevice();
                //device.MonitorDeviceEvents = true;
            }

            // Creating threads
            Threads = new List<Thread>(new Thread[devices.Count]);
            for (int i = 0; i < devices.Count(); i++)
            {
                HidDevice device = devices[i];
                Threads[i] = new Thread(() =>
                {
                    while (device != null && !Abort)
                    {
                        ReportHandler(device);
                    }
                });
                Threads[i].Start();
            }
        }

        /// <summary>
        /// Handles the bytes send by the device
        /// </summary>
        private void ReportHandler(HidDevice device)
        {
            byte[] data = device.ReadReport().Data.ToArray();
            Console.WriteLine(data);
            if (data.Length < 1)
                return;

            if (DevicePresets.Contains(this.device.DeviceName, data) && !Abort)
            {
                ClickHandler.HandleClick();
            }

        }
    }
}
