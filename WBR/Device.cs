using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBR
{
    public class Device
    {
        public int Vid { get; private set; } // Vendor ID
        public int Pid { get; private set; } // Product ID
        public string DeviceName { get; private set; }


        public Device(int vid, int pid)
        {
            Vid = vid;
            Pid = pid;
        }
        public Device(int vid, int pid, string deviceName)
        {
            Vid = vid;
            Pid = pid;
            DeviceName = deviceName;
        }


        
        public Device(HidDevice device)
        {
            if (!device.DevicePath.Contains("vid_") || !device.DevicePath.Contains("pid_"))
            {
                Vid = 0;
                Pid = 0;
                return;
            }

            //Console.WriteLine("Path: " + device.DevicePath);
            string _v = device.DevicePath.Split("vid_")[1];
            string _p = _v.Split("pid_")[1];
            string v = _v.Substring(0, 4);
            string p = _p.Substring(0, 4);
            string _id = _p.Contains("col") ? _p.Split("col")[1].Substring(0, 2) : "0";
            int vid = Convert.ToInt32(v, 16);
            int pid = Convert.ToInt32(p, 16);
            int id = Convert.ToInt32(_id);

            Vid = vid;
            Pid = pid;

            DeviceName = device.Description;
        }




        public override bool Equals(object? obj)
        {
            if (obj is Device)
            {
                Device other = (Device)obj;
                return Vid == other.Vid && Pid == other.Pid;// && Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Vid.GetHashCode();
                hash = hash * 23 + Pid.GetHashCode();
                //hash = hash * 23 + Id.GetHashCode();
                return hash;
            }
        }
    }
}
