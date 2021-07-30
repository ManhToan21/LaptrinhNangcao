using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WepApp
{
    public partial class BaseController : Controller
    {
        #region Database
        static BsonData.DataBase _mainDB;
        static public void Register(string dataPath)
        {
            _mainDB = new BsonData.DataBase(dataPath, "MainDB");

            // Create Admin account if not exists
            var acc = new Controllers.AccountController();
            acc.InsertAccount(new Models.Account { Id = "admin", Role = Roles.Admin });
        }
        public static BsonData.DataBase MainDB => _mainDB;
        protected virtual BsonData.Collection GetCollectionCore(string name)
        {
            if (name == null)
            {
                name = this.GetType().Name;
                name = name.Substring(0, name.Length - 10);
            }
            return MainDB.GetCollection(name);
        }
        public BsonData.Collection Collection => GetCollectionCore(null);

        #endregion

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var u = User;
            if (u != null)
            {
                ViewBag.Account = u.Account;
            }
            return base.View(viewName, masterName, model);
        }

        #region Mã hóa
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
        #endregion

        #region Điều hướng
        protected virtual ActionResult GoFirst()
        {
            return RedirectToAction("Index");
        }
        protected virtual ActionResult GoHome()
        {
            var u = User;
            if (u == null)
            {
                return Redirect("/home");
            }
            return Redirect("/" + u.Account.Role);
        }
        #endregion
    }
}

namespace WepApp
{
    public class Roles
    {
        public const string Admin = "Admin";
    }
    partial class BaseController
    {
        new public Models.UserInfo User
        {
            get
            {
                if (Session == null) return null;
                return (Models.UserInfo)Session["user"];
            }
            set
            {
                Session["user"] = value;
            }
        }
        public virtual string Role => null;
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.Role != null)
            {
                var u = this.User;
                if (u == null || u.Account.Role != Role)
                {
                    var v = filterContext.RouteData.Values;
                    v["controller"] = "home";
                    v["action"] = "login";
                }
            }

            base.OnAuthorization(filterContext);
        }
    }
}

namespace WepApp
{
    public class ApiRequest
    {
        public Models.UserInfo User { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ObjectId { get; set; }
        public JObject Value { get; set; }
        public void Split()
        {
            if (Url != null)
            {
                var v = Url.Split('/');
                Controller = v[1];
                Action = v.Length > 2 ? v[2] : "index";
                if (v.Length > 3)
                {
                    ObjectId = v[3];
                }
            }
        }
    }

    public partial class BaseController
    {
        public MethodInfo GetApiAction(string name)
        {
            name = "api" + name;
            foreach (var m in this.GetType().GetMethods())
            {
                if (m.GetParameters().Length == 0 && m.Name.ToLower() == name)
                {
                    return m;
                }
            }
            return null;
        }
        ApiRequest _apiArgument;
        protected ApiRequest ApiData => _apiArgument;
        public object ExecuteApi(string actionName, ApiRequest argument)
        {
            // Kiểm tra quyền gọi API
            if (this.Role != null)
            {
                var u = this.User;
                if (u == null)
                {
                    u = (new Controllers.UserController().Collection)
                        .FindById<Models.UserInfo>(argument.Token);
                }
                if (u == null || u.Account.Role != this.Role) return Error(-1);
                argument.User = u;
            }

            // Gọi phương thức API
            var method = GetApiAction(actionName.ToLower());
            if (method == null) {
                return new { Code = ApiError.SERVER };
            }

            _apiArgument = argument;
            return method.Invoke(this, new object[] { });
        }
        protected JObject GetApiObject()
        {
            return _apiArgument.Value;
        }
        protected T GetApiObject<T>()
        {
            return GetApiObject().ToObject<T>();
        }
        protected T GetApiObject<T>(string key)
        {
            return GetApiObject().Get<T>(key);
        }
        protected object Error(int code, string message)
        {
            return new { Code = code, Message = message };
        }
        protected object Error(int code)
        {
            return new { Code = code };
        }
        protected object Success(object value)
        {
            return new { Code = 0, Value = value };
        }
        protected object Success()
        {
            return new { Code = 0 };
        }
        public virtual object ApiInsert()
        {
            var id = _apiArgument.ObjectId ?? new BsonData.ObjectId();
            var o = GetApiObject();
            this.Collection.Insert(id, o);

            o.SetObject("Id", id);
            return Success(o);
        }
        public virtual object ApiUpdate()
        {
            var src = GetApiObject();
            this.Collection.FindAndUpdate(_apiArgument.ObjectId, o =>
            {
                foreach (var p in o.Properties())
                {
                    var v = src.Get(p.Name);
                    if (v != null)
                    {
                        o.SetObject(p.Name, v);
                    }
                }
                src = o;
            });
            return Success(src);
        }
        public virtual object ApiDelete()
        {
            this.Collection.Delete(_apiArgument.ObjectId);
            return Success();
        }
        public virtual object ApiSelect()
        {
            return Success(Collection.ToList());
        }
    }
}