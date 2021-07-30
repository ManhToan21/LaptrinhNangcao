using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WepApp.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string id)
        {
            if (id != null)
            {
                var u = new UserController().Collection.FindById<Models.UserInfo>(id);
                this.User = u;

                var cookie = new HttpCookie("token", id)
                {
                    Expires = DateTime.Now.AddMinutes(10),
                };
                Response.SetCookie(cookie);
                return GoHome();
            }

            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult Login()
        {
            return View(new Models.AccountBinding());
        }

        public ActionResult Logout()
        {
            new UserController().Collection.Delete(User.Id);
            Session.Abandon();
            return GoFirst();
        }

        public object ApiLogin()
        {
            var acc = GetApiObject<Models.AccountBinding>();
            var id = acc.UserName.ToLower();
            var e = new AccountController()
                .Collection
                .FindById<Models.Account>(id);

            if (e == null) { return Error(-1); }
            if (e.Password != MD5Hash(id + acc.Password)) { return Error(-2); }

            var token = MD5Hash(id + DateTime.Now);
            var u = new Models.UserInfo
            {
                Account = e,
                Name = id,
            };

            new UserController().Collection.Insert(token, u);
            
            u.Id = token;
            return Success(u);
        }

        [HttpPost]
        public ActionResult Login(Models.AccountBinding account)
        {
            var e = new AccountController()
                .Collection
                .FindById<Models.Account>(account.UserName);

            var msg = "UN";
            if (e != null)
            {
                msg = "PW";
                if (e.Password == account.Password)
                {
                    User = new Models.UserInfo
                    {
                        Account = e,
                        Name = account.UserName,
                    };
                    return Redirect("/" + e.Role);
                }
            }
            ViewBag.Message = msg;
            return View(account);
        }
    }
}
