using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ConsoleApp.Views
{
    class BaseView<T> : System.Mvc.IView
    {
        protected virtual T ConvertModel(object value)
        {
            return (T)value;
        }
        public System.Mvc.ViewDataDictionary ViewBag { get; private set; }
        public T Model { get; set; }
        public BaseController Controller { get; private set; }
        public void Render(System.Mvc.Controller controller)
        {
            Controller = (BaseController)controller;
            ViewBag = controller.ViewData;

            var v = ViewBag.Model;
            if (v != null)
            {
                Model = this.ConvertModel(v);
            }

            this.RenderBody();
        }
        protected virtual void Caption(string text)
        {
            Console.WriteLine("\n>>> {0} >>>", text);
        }
        protected string Input(string caption, int space = 10)
        {
            Console.Write(" - ");
            Console.Write(caption + ':');
            for (int i = caption.Length; i < space; i++) Console.Write(' ');
            return Console.ReadLine();
        }
        protected void Message(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        protected void Error(string text)
        {
            Message(text, ConsoleColor.Red);
        }
        protected void Success(string text)
        {
            Message(text, ConsoleColor.Green);
        }
        protected void Info(string text)
        {
            Message(text, ConsoleColor.Cyan);
        }
        protected void Menu(Dictionary<string, string> items)
        {
            int i = 0;
            foreach (var p in items)
            {
                Console.WriteLine("{0}. {1}", ++i, p.Value);
            }
            while (true)
            {
                Console.Write(">> ");
                var cmd = Console.ReadKey();

                if (char.IsDigit(cmd.KeyChar))
                {
                    i = cmd.KeyChar & 15;
                    foreach (var p in items)
                    {
                        if (--i == 0)
                        {
                            Console.WriteLine();
                            Controller.Execute(p.Key);
                        }
                    }
                }
            }
        }
        protected void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
        protected void Redirect(string url, params object[] values)
        {
            System.Mvc.Engine.Execute(url, values);
        }
        protected virtual void RenderBody() { }
    }

    class BaseView : BaseView<JObject>
    {
        protected override JObject ConvertModel(object value)
        {
            return JObject.FromObject(value);
        }
    }
}
