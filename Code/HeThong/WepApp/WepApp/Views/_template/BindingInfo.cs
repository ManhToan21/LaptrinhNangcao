using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Vst
{
    public class BindingInfo
    {
        public string BindingName { get; set; }
        public string Caption { get; set; }
        public string Input { get; set; }
        public bool AllowNull { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
        public string FormatString { get; set; }
        public int Width { get; set; }
    }
}