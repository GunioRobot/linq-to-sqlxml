using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics;

namespace LinqToSqlXml
{
    public class ReadDocument
    {
        public Guid Id { get; set; }
        public string Json { get; set; }
    }

    public static class Database
    {
        public static IEnumerable<ReadDocument> ExecuteReader(DbConnection conn, string sql)
        {
            if (conn.State == System.Data.ConnectionState.Closed)
                conn.Open();
            
            using (var cmd = conn.CreateCommand())
            {                
                cmd.CommandText = sql;              
                using (var reader = cmd.ExecuteReader())
                {               
                    while (reader.Read())
                    {
                        yield return new ReadDocument()
                        {
                            Id = reader.GetGuid(0),
                            Json = reader.GetString(1),
                        };
                    }
                }
            }
        }
    }
}
