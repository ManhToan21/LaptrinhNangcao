using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace WepApp
{
    public enum ApiError { SERVER = -404 };
    [RoutePrefix("api/access")]
    public class AccessController : ApiController
    {
        static Dictionary<string, Type> _controllerMap;
        BaseController GetController(string key)
        {
            key = key.ToLower();
            if (_controllerMap == null)
            {
                _controllerMap = new Dictionary<string, Type>();
                foreach (var type in GetType().Assembly.GetTypes())
                {
                    var name = type.Name.ToLower();
                    if (name.EndsWith("controller"))
                    {
                        _controllerMap.Add(name.Substring(0, name.Length - 10), type);
                    }
                }
            }
            Type ctype = null;
            if (_controllerMap.TryGetValue(key, out ctype))
            {
                return (BaseController)Activator.CreateInstance(ctype);
            }
            return null;
        }

        [HttpPost]
        [Route("post")]
        public object Post(ApiRequest args)
        {
            string message = null;
            try
            {
                args.Split();
                var controller = GetController(args.Controller);
                if (controller != null)
                {
                    return controller.ExecuteApi(args.Action, args);
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return new { Code = ApiError.SERVER, Message = message };
        }
    }
}
