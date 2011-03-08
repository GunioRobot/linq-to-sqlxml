using System.Data;
using System;
using System.Data.SqlClient;
namespace LinqToSqlXml
{
    public class DocumentContext
    {
        private readonly DocumentDataContext db = new DocumentDataContext();
        private readonly string dbInstance;
        private readonly DataTable insertedDocuments;

        public DocumentContext(string dbInstance)
        {
            this.dbInstance = dbInstance;
            insertedDocuments = new DataTable();
            insertedDocuments.Columns.Add("Id", typeof(Guid));
            insertedDocuments.Columns.Add("DbName",typeof(string));
            insertedDocuments.Columns.Add("CollectionName",typeof(string));
            insertedDocuments.Columns.Add("XmlIndex",typeof(string));
            insertedDocuments.Columns.Add("JsonData",typeof(string));
        }

        public string DbInstance
        {
            get { return dbInstance; }
        }

        internal DocumentDataContext DB
        {
            get { return db; }
        }

        public void EnsureDatabaseExists()
        {
            if (!db.DatabaseExists())
                db.CreateDatabase();
            db.Connection.Open();
        }

        public DocumentCollection GetCollection(string collectionName)
        {
            return new DocumentCollection(this, collectionName);
        }

        public DocumentCollection<T> GetCollection<T>() where T : class
        {
            return new DocumentCollection<T>(this);
        }

        public void SaveChanges()
        {
            if (db.Connection.State == ConnectionState.Closed)
                db.Connection.Open();

            using (SqlBulkCopy copy = new SqlBulkCopy(db.Connection as SqlConnection))
            {
                copy.ColumnMappings.Add(0, 0);
                copy.ColumnMappings.Add(1, 1);
                copy.ColumnMappings.Add(2, 2);
                copy.ColumnMappings.Add(3, 3);
                copy.ColumnMappings.Add(4, 4);
                copy.SqlRowsCopied += new SqlRowsCopiedEventHandler(copy_SqlRowsCopied);
                copy.NotifyAfter = 100;
                copy.BatchSize = 1000;
                copy.DestinationTableName = "Documents";

                copy.WriteToServer(insertedDocuments);
            }
            insertedDocuments.Clear();
            //db.SubmitChanges();
        }

        void copy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
        //    Console.Write(".");
        }

        
        public void InsertDocument(Guid id, string collectionName, string xmlIndex, string json)
        {
            insertedDocuments.Rows.Add(id, dbInstance, collectionName, xmlIndex, json);
        }
    }
}