using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Mvc;
namespace ConsoleApp.Controllers
{
    class AccountController : BaseController
    {
        public ActionResult Default()
        {
            var db = Collection;
            if (db.Count() == 0)
            {
                return Post(CreateApiContext(null, null, "/account/select"), o => {
                    foreach (var e in o.ToObject<List<Models.Account>>())
                    {
                        db.Insert(e.Id, e);
                    }
                    RedirectToAction("Default");               
                });
            }
            return View();
        }
        public ActionResult All()
        {
            return View(Collection.ToList<Models.Account>());
        }
        public ActionResult Clear()
        {
            Collection.Clear();
            return GoFirst();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Delete()
        {
            return View();
        }
        public ActionResult Update(string action, string un, string role)
        {
            var db = Collection;
            var e = new Models.Account { Id = un, Role = role };
            return Post(CreateApiContext(e, un, "account/" + action), o => {
                switch (action[0])
                {
                    case 'i': db.Insert(un, o); break;
                    case 'u': db.Update(un, o); break;
                    case 'd': db.Delete(un); break;
                }
                RedirectToAction("Default");
            });
        }
    }
}
