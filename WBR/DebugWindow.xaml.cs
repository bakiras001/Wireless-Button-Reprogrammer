using HidLibrary;
using System;
using System.Collections;
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

    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[]? x, byte[]? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;
            return x.SequenceEqual(y);
        }

        public int GetHashCode(byte[] obj)
        {
            // einfacher Hash: Länge + Bytes kombinieren
            unchecked
            {
                int hash = 17;
                foreach (var b in obj)
                    hash = hash * 31 + b;
                return hash;
            }
        }
    }
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        private List<HidDevice> DevicesBefore;
        public ObservableCollection<Device> Devices { get; set; } = new();
        public string DeviceName { get; set; }
        private DeviceHandler? selectedDevice = null;

        private Dictionary<byte[], int> bytes = new Dictionary<byte[], int>(new ByteArrayComparer());
        private int SelectedVid = 0;
        private int SelectedPid = 0;
        private int Threshold = 1;

        private Action SaveAction;
        private Config ConfigRef;
        public DebugWindow(Action action, ref Config config)
        {
            ConfigRef = config;
            InitializeComponent();

            SaveAction = action;

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
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void HandleBytes(byte[] data)
        {
            if (selectedDevice == null) return;
            if (!bytes.ContainsKey(data))
            {
                bytes[data] = 0;
            }
            bytes[data] += 1;
            Application.Current.Dispatcher.Invoke(() =>
            {
                BytesTextBlock.Text = "";
                var bytesList = bytes.ToList();
                int l = bytesList.Count;
                int i = 0;
                foreach (var pair in bytesList)
                {
                    if (pair.Value > Threshold)
                    {
                        BytesTextBlock.Text += $"{ByteArrToStr(pair.Key)} ({pair.Value})";
                        if (i < l - 1)
                            BytesTextBlock.Text += "\n";
                    }
                    i++;
                }
            });
        }
        
        private void SavePreset(object sender, RoutedEventArgs e)
        {
            if (SelectedVid + SelectedPid == 0) return;
            var keys = bytes.Keys.ToList();
            List<List<byte>> bytesCleaned = new List<List<byte>>(keys.Count());
            foreach(var b in keys)
            {
                if (bytes[b] > Threshold)
                {
                    bytesCleaned.Add(new List<byte>(b));
                }
            }
            var t = new SaveWindow(SelectedVid, SelectedPid, bytesCleaned);
            t.Show();
        }
        private void Select(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            var device = button?.DataContext as Device; // Typ anpassen
            if (device != null)
            {
                selectedDevice = new DeviceHandler(device.Vid, device.Pid, device.DeviceName);
                selectedDevice.Init(HandleBytes);
                SelectedVid = device.Vid;
                SelectedPid = device.Pid;
                ConfigRef.VendorID = SelectedVid;
                ConfigRef.ProductID = SelectedPid;
                SaveAction();
            }
        }

        private void ClearDevices(object sender, RoutedEventArgs e)
        {
            Devices.Clear();
            bytes.Clear();
            BytesTextBlock.Text = "";
            SelectedVid = 0;
            SelectedPid = 0;
        }

        private void DeviceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public static string ByteArrToStr(byte[] data)
        {
            string res = "[";
            for (int i = 0; i < data.Length - 1; i++)
            {
                res += data[i] + ",";
            }
            res += data[data.Length - 1] + "]";
            return res;
        }
    }
}
