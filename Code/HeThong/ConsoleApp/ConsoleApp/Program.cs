using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVC = System.Mvc.Engine;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MVC.Register(new Program(), result => {
            });

            MVC.Execute("home/login");
            while (true) { }
        }
    }
}
