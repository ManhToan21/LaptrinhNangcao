using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Web
{
    class JsonObject : List<string>
    {
        public JsonObject() { }
        public void Add(string name, object value, string format)
        {
            if (value == null || value.Equals(string.Empty))
            {
                return;
            }
            if (format == null)
            {
                base.Add(string.Format("\"{0}\":\"{1}\"", name, value));
            }
            else
            {
                base.Add(string.Format("\"{0}\":\"{1:" + format + "}\"", name, value));
            }
        }
        public void Add(string name, object value)
        {
            this.Add(name, value, null);
        }
        public override string ToString()
        {
            return '{' + string.Join(",", this) + '}';
        }
    }

    public class ColumnInfo : Vst.BindingInfo
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string Comment { get; set; }

        public string ToVstFormat()
        {
            var lst = new JsonObject();
            lst.Add("tag", Input);
            lst.Add("type", Type);
            lst.Add("cls", ClassName);
            lst.Add("name", Name);
            lst.Add("caption", Caption);
            lst.Add("value", Value);
            if (!AllowNull) { lst.Add("required", true); }

            return lst.ToString();

        }
    }

    public class VstViewFormat
    {
        public object DataSource { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public string ExtendedActions { get; set; }
        public int UpdateFlag { get; set; }
        public string VstColumns
        {
            get
            {
                var lst = new List<string>();
                foreach (var col in Columns)
                {
                    lst.Add(col.ToVstFormat());
                }
                return "[" + string.Join(",", lst) + "]";
            }
        }
        public string VstObject(object value)
        {
            var lst = new JsonObject();
            var type = value.GetType();
            foreach (var col in this.Columns)
            {
                var p = type.GetProperty(col.Name);
                if (p != null)
                {
                    var v = p.GetValue(value);
                    lst.Add(col.Name, v, col.FormatString);
                }
            }

            var pKey = type.GetProperty("Id");
            if (pKey != null)
            {
                lst.Add(pKey.Name, pKey.GetValue(value));
            }
            return lst.ToString();
        }

        public HtmlString HtmlColumns
        {
            get
            {
                return new HtmlString(VstColumns);
            }
        }

        public HtmlString HtmlData
        {
            get
            {
                if (DataSource is System.Collections.IEnumerable)
                {
                    var lst = new List<string>();
                    foreach (var e in (System.Collections.IEnumerable)DataSource)
                    {
                        lst.Add(VstObject(e));
                    }
                    return new HtmlString('[' + string.Join(",", lst) + ']');
                }

                if (DataSource == null)
                {
                    return new HtmlString("[]");
                }
                return new HtmlString(VstObject(DataSource));
            }
        }
    }
}