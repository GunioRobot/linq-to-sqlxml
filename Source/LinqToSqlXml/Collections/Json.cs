using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToSqlXml
{
    public static class Json<T>
    {
        public static readonly ServiceStack.Text.JsonSerializer<T> serializer = new ServiceStack.Text.JsonSerializer<T> ();
    }
}
