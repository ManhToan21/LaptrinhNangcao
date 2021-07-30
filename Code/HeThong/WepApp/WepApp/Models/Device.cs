using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WepApp.Models
{
    public class DeviceStatus : Dictionary<string, int>
    {
    }
    public class Device : BsonData.Document
    {
        public string Name { get; set; }
        public DeviceStatus Status { get; set; } = new DeviceStatus();
    }
}