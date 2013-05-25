using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace StartMenuWin8
{
    class Utilities
    {
    }

    public class StringEnumValue : EnumValue
    {
        public StringEnumValue(string value) : base(value) { }

        public static string GetStringValue(Enum value)
        {
            return GetObjectValue(value, value.GetType()).ToString();
        }
    }

    public class EnumValue : System.Attribute
    {
        private object _value;

        public EnumValue(object value)
        {
            _value = value;
        }

        public object Value
        {
            get { return _value; }
        }

        public static object GetObjectValue(Enum value, Type enumType)
        {
            object output = null;

            //Check first in our cached results...
            //Look for our 'ObjectValueAttribute' 
            //in the field's custom attributes
            FieldInfo fi = enumType.GetField(value.ToString());
            EnumValue[] attrs =
               fi.GetCustomAttributes(typeof(EnumValue),
                                       false) as EnumValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }
}
