using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WBR
{
    /// <summary>
    /// Handles input from a HID device based on Vendor and Product ID
    /// </summary>
    public class DeviceHandler
    {
        private Device device;
        private List<HidDevice> devices;
        private List<Task> tasks;
        private CancellationTokenSource cts;

        public DeviceHandler(int vid, int pid, string name)
        {
            device = new Device(vid, pid, name);
        }

        /// <summary>
        /// Stops all running device tasks safely
        /// </summary>
        public void Stop()
        {
            if (cts == null)
                return;

            cts.Cancel();

            foreach (var d in devices)
            {
                try { d.CloseDevice(); } catch { }
            }
        }

        /// <summary>
        /// Initializes HID device and starts one task per device
        /// </summary>
        public void Init(Action<byte[]> action)
        {
            Stop();

            cts = new CancellationTokenSource();

            devices = HidDevices
                .Enumerate(device.Vid, device.Pid)
                .Where(d => d != null)
                .ToList();

            foreach (var d in devices)
            {
                d.OpenDevice();
            }

            tasks = new List<Task>();

            foreach (var hid in devices)
            {
                var localDevice = hid;

                tasks.Add(Task.Run(() =>
                {
                    RunDeviceLoop(localDevice, action, cts.Token);
                }, cts.Token));
            }
        }

        /// <summary>
        /// Device read loop (replaces busy thread loop)
        /// </summary>
        private void RunDeviceLoop(HidDevice device, Action<byte[]> action, CancellationToken token)
        {
            const int inactivityTimeoutMS = 50;
            while (!token.IsCancellationRequested && device != null)
            {
                try
                {
                    var report = device.ReadReport();
                    var data = report.Data?.ToArray(); 

                    if (data != null && data.Length < 1)
                        throw new Exception("HID-Device data was empty!");
                    
                    action?.Invoke(data);
                }
                catch
                { 
                    // optionally log or ignore read errors
                }
                finally
                {
                    Thread.Sleep(inactivityTimeoutMS); // Reduce CPU usage; Sleep even when recieving bytes due to garbage bytes
                }
            }
        }

        public void ActionHandler(byte[] data)
        {
            if (DevicePresets.Contains(this.device.DeviceName, data))
            {
                ClickHandler.HandleClick();
            }
        }
    }
}