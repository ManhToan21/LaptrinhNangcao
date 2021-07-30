using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WepApp.Controllers
{
    public class DeviceController : BaseAdminController
    {
        // GET: Device
        public ActionResult Index()
        {
            var db = Collection;
            var lst = db.ToList<Models.Device>();
            if (lst.Count == 0)
            {
                var signals = CreateSignals();
                for (int i = 0; i < 4; i++)
                {
                    var key = "LED" + i;
                    signals.Add(key, 0);
                }
                for (int i = 0; i < 10; i++)
                {
                    string id = string.Format("LTNC{0:0000}", i + 1);
                    var device = new Models.Device { Name = id, Status = signals };

                    db.Insert(id, device);
                    device.Id = id;
                    lst.Add(device);
                }
            }    
            return View(lst);
        }
        Models.DeviceStatus CreateSignals()
        {
            var signals = new Models.DeviceStatus();
            for (int i = 0; i < 4; i++)
            {
                var key = "LED" + i;
                signals.Add(key, 0);
            }
            return signals;
        }

        public override object ApiInsert()
        {
            var device = GetApiObject<Models.Device>();
            device.Status = CreateSignals();

            Collection.Insert(device.Id, device);
            return Success(device);
        }
    }
}