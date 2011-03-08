using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace LinqToSqlXml
{
    public static class Database
    {
        public static IEnumerable<string> ExecuteReader(SqlConnection conn, string sql)
        {
            if (conn.State == System.Data.ConnectionState.Closed)
                conn.Open();
            
            
            
            using (var cmd = conn.CreateCommand())
            {
                var queue = new ConcurrentQueue<string>();
                bool done = false;
                cmd.CommandText = sql;
                var result = cmd.BeginExecuteReader( r => 
                    {
                        using (SqlDataReader reader = cmd.EndExecuteReader(r))
                        {
                            int count = 0;
                            while (reader.Read())
                            {
                                var doc = reader.GetString(0);
                                queue.Enqueue(doc);
                                count++;
                            }
                            done = true;
                        }
                    },null);
                
                while (!done || queue.Count >0)
                {
                    string doc = null;
                    if (queue.TryDequeue(out doc))
                        yield return doc;
                }
                
                //cmd.CommandText = sql;              
                //using (var reader = cmd.ExecuteReader())
                //{               
                //    while (reader.Read())
                //    {
                //        yield return new ReadDocument()
                //        {
                //            Json = reader.GetString(0),
                //        };
                //    }
                //}
            }
        }
    }
}
