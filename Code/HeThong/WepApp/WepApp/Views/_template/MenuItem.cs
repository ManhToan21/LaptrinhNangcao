using System;
using System.Collections.Generic;
using System.Linq;


namespace System.Web
{
    public class InfoPair
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
    public class MenuItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public string ClassName { get; set; }
        public string IconName { get; set; }
        public bool BeginGroup { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public InfoPair Notify { get; set; }
        public InfoPair DeadLine { get; set; }
        public bool Disabled { get; set; }
        public bool IsActive { get; set; }
        public List<MenuItem> Childs { get; set; }

        public MenuItem() { }
        public MenuItem(string text, string url)
        {
            Text = text;
            Url = url;
        }
    }

    public class MenuItemCollection : Dictionary<string, MenuItem> { }
}