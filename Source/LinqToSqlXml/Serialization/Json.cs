using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToSqlXml
{
    public static class JsonSerializer<T>
    {
        public static readonly ServiceStack.Text.JsonSerializer<T> Default = new ServiceStack.Text.JsonSerializer<T> ();
    }
}
