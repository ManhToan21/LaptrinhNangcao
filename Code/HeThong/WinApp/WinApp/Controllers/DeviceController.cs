using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    using DEV = Models.DeviceViewModel;

    class DeviceController : BaseController
    {
        static List<DEV> _devices;
        public static List<DEV> Devices => _devices;

        public ActionResult Default()
        {
            if (_devices == null)
            {
                return Post(CreateApiContext(new { }, null, "device/select"), o => {
                    _devices = o.ToObject<List<DEV>>();

                    MqttController.Connected += (broker) => { 
                        foreach (var device in _devices)
                        {
                            broker.Subscribe(new string[] { "status/" + device.Id }, new byte[] { 0 });
                        }
                    };
                    Engine.CreateThread(MqttController.Connect);

                    GoFirst();
                });
            }
            return View(_devices);
        }

        public ActionResult Status(string id, JObject o)
        {
            foreach(var Device in _devices)
            {
                if (Device.Id == id)
                {
                    Device.UpdateStatus(o.Get<int>("value"));
                    break;
                }    
            }    
            return Done();
        }
    }
}
