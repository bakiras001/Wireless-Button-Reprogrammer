using HidLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WBR
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        private List<HidDevice> DevicesBefore;
        public ObservableCollection<Device> Devices { get; } = new();
        public DebugWindow()
        {
            InitializeComponent();

            SetDevicesBefore();
            DataContext = this;

            // Example: Add a device
            // Devices.Add(new Device("My Gamepad", 0x046D, 0xC534));
            var t = new Thread(() =>
            {
                ScanForNewDevicesLoop();
            });
            t.Start();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SetDevicesBefore()
        {
            DevicesBefore = HidDevices.Enumerate().ToList();
        }

        /*                Application.Current.Dispatcher.Invoke(() =>
                {
                    Devices.Clear();
                });
        */

        private void ScanForNewDevicesLoop()
        {
            SetDevicesBefore();
            while (true)
            {

                var devicesAfter = HidDevices.Enumerate().ToList();
                var devicesAfterHash = new Dictionary<Device, HidDevice>();
                //Console.WriteLine("Adding devices to dict..");
                foreach (var d in devicesAfter)
                {
                    var key = new Device(d);
                    if (!devicesAfterHash.ContainsKey(key))
                    {
                        int h = d.GetHashCode();
                        devicesAfterHash.Add(key, d);
                    }
                }


                var devicesBeforeHash = new Dictionary<Device, HidDevice>();
                foreach (var d in DevicesBefore)
                {
                    var key = new Device(d);
                    if (!devicesBeforeHash.ContainsKey(key))
                    {
                        int h = d.GetHashCode();
                        devicesBeforeHash.Add(key, d);
                    }
                }


                foreach (Device device in devicesBeforeHash.Keys)
                {
                    if (!devicesAfterHash.ContainsKey(device))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Devices.Add(device);
                        });
                    }
                }

                SetDevicesBefore();
                System.Threading.Thread.Sleep(1000); // Wait for 5 seconds before scanning again
            }
        }

        private void DeviceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
