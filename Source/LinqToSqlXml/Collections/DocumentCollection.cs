﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Text.Json;
using System.Data.SqlClient;
namespace LinqToSqlXml
{
    public class DocumentCollection
    {
        protected string collectionName = "";
        protected DocumentContext owner;


        public string CollectionName
        {
            get { return collectionName; }
        }

        internal IEnumerable<ReadDocument> ExecuteQuery(string query)
        {
            return Database.ExecuteReader(this.owner.DB.Connection as SqlConnection,query);
        }
    }

    public class DocumentCollection<T> : DocumentCollection
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