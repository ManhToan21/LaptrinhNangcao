using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WepApp.Controllers
{
    public class AccountController : BaseAdminController
    {
        // GET: Account
        public ActionResult Index()
        {
            return View(Collection.ToList<Models.Account>());
        }
        public object InsertAccount(Models.Account acc)
        {
            var id = acc.Id.ToLower();
            if (Collection.Contains(id))
            {
                return null;
            }

            acc.Password = MD5Hash(id + id);
            Collection.Insert(id, acc);
            return id;
        }
        public override object ApiInsert()
        {
            var acc = GetApiObject<Models.Account>();
            var id = InsertAccount(acc);
            if (id == null)
            {
                return Error(-1);
            }
            return Success(acc);
        }
        //public ActionResult Create()
        //{
        //    return View(new Models.AccountBinding());
        //}

        //[HttpPost]
        //public ActionResult Create(Models.AccountBinding account)
        //{
        //    if (Collection.Contains(account.UserName))
        //    {
        //        ViewBag.Message = "EXISTS";
        //        return View(account);
        //    }
        //    Collection.Insert(account.UserName, account);
        //    return GoFirst();
        //}
    }
}