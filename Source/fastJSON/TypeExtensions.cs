using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fastJSON
{
    public static class TypeExtensions
    {
        public static string SerializedName(this Type self)
        {
            return string.Format("{0}, {1}", self.FullName,
                                 self.Assembly.FullName.Substring(0, self.Assembly.FullName.IndexOf(",")));
        }
    }
}
