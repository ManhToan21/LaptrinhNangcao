using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WepApp.Controllers
{
    public class BaseAdminController : BaseController
    {
        public override string Role => "Admin";
    }
    public class AdminController : BaseAdminController
    {
        // GET: Admin
        public ActionResult Index()
        {
            return Redirect("/account");
        }
    }
}