using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToSqlXml
{
    public static class Json
    {
        public static T Deserialize<T>(string json)
        {
            return (T)fastJSON.JSON.Instance.ToObject(json);
        }

        public static string Serialize(object obj)
        {
            return fastJSON.JSON.Instance.ToJSON(obj);
        }
    }
}
