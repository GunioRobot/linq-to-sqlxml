using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Text.Json;
using System.Data.SqlClient;
namespace LinqToSqlXml
{
    public class DocumentCollectionBase
    {
        protected string collectionName = "";
        protected DocumentContext owner;


        public string CollectionName
        {
            get { return collectionName; }
        }

        internal IEnumerable<string> ExecuteQuery(string query)
        {
            return Database.ExecuteReader(this.owner.DB.Connection as SqlConnection,query);
        }
    }

    public class DocumentCollection : DocumentCollectionBase
    {
        public DocumentCollection(DocumentContext owner,string collectionName)
        {
            this.owner = owner;
            this.collectionName = collectionName;
        }

        public IQueryable<T> AsQueryable<T>()
        {
            var queryProvider = new SqlServerQueryProvider(this);
            return new DocumentQuery<T>(queryProvider);
        }

        public void Add(object item)
        {
            
            

            Guid documentId;

            PropertyInfo idproperty = item.GetType().GetDocumentIdProperty();
            if (idproperty != null)
            {
                var propertyvalue = (Guid)idproperty.GetValue(item, null);
                if (propertyvalue == Guid.Empty)
                {
                    documentId = Guid.NewGuid();
                    idproperty.SetValue(item, documentId, null);
                }
                else
                    documentId = propertyvalue;
            }
            else
            {
                documentId = Guid.NewGuid();
            }

            var doc = new Document();
            doc.Id = documentId;
            doc.XmlIndex = DocumentSerializer.Serialize(item);


            doc.JsonData = ServiceStack.Text.JsonSerializer.SerializeToString(item, item.GetType());
            doc.CollectionName = collectionName;
            doc.DbName = owner.DbInstance;

            owner.DB.Documents.InsertOnSubmit(doc);
        }
    }

    public class DocumentCollection<T> : DocumentCollectionBase
    {
        public DocumentCollection(DocumentContext owner)
        {
            this.owner = owner;
            collectionName = typeof (T).Name;
        }

        internal DocumentContext Owner
        {
            get { return owner; }
        }

        public void Add(string json)
        {
            var obj = JsonSerializer<T>.Default.DeserializeFromString(json);
            this.Add(obj);
        }

        public void Add(T item)
        {
            Guid documentId;

            PropertyInfo idproperty = item.GetType().GetDocumentIdProperty();
            if (idproperty != null)
            {
                var propertyvalue = (Guid) idproperty.GetValue(item, null);
                if (propertyvalue == Guid.Empty)
                {
                    documentId = Guid.NewGuid();
                    idproperty.SetValue(item, documentId, null);
                }
                else
                    documentId = propertyvalue;
            }
            else
            {
                documentId = Guid.NewGuid();
            }

            var doc = new Document();
            doc.Id = documentId;
            doc.XmlIndex = DocumentSerializer.Serialize(item);


            doc.JsonData = JsonSerializer<T>.Default.SerializeToString(item);
            doc.CollectionName = collectionName;
            doc.DbName = owner.DbInstance;

            owner.DB.Documents.InsertOnSubmit(doc);
        }

        public IQueryable<T> AsQueryable()
        {
            var queryProvider = new SqlServerQueryProvider(this);
            return new DocumentQuery<T>(queryProvider);
        }
    }
}