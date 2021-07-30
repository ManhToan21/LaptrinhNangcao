using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WinApp.Models
{
    public class DeviceStatus : Dictionary<string, int> 
    { 
    }
    public class Device : BsonData.Document
    {
        public string Name { get; set; }
        public DeviceStatus Status { get; set; } = new DeviceStatus();
    }
    public class DeviceViewModel : Device
    {
        public void UpdateStatus(int value)
        {
            int i = 0;
            bool changed = false;

            var s = new DeviceStatus();
            foreach (var p in Status)
            {
                var b = (value & (1 << i)) == 0 ? 0 : 1;
                i++;
                if (b != p.Value)
                {
                    changed = true;
                }
                s.Add(p.Key, b);
            }
            if (changed)
            {
                Status = s;
                Changed?.Invoke(this, value);
            }
        }
        public event Action<Device, int> Changed;
    }

}