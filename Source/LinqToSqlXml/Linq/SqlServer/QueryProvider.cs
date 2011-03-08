﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using LinqToSqlXml.SqlServer;
using ServiceStack.Text.Json;
namespace LinqToSqlXml
{
    public class SqlServerQueryProvider : IQueryProvider
    {
        private readonly DocumentCollection documentCollection;

        public SqlServerQueryProvider(DocumentCollection documentCollection)
        {
            this.documentCollection = documentCollection;
        }

        #region IQueryProvider Members

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DocumentQuery<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return default(TResult);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static IEnumerable<TResult> DocumentEnumerator<TResult>(IEnumerable<ReadDocument> documents)
        {            
            
            return documents
                .Select(document => document.Json)
                .Select(json => JsonSerializer<TResult>.Default.DeserializeFromString(json))
                .Where(result => result != null);
        }

        public IEnumerable<TResult> ExecuteQuery<TResult>(Expression expression)
        {
            var queryBuilder = new QueryBuilder();
            queryBuilder.Visit(expression);

            var sql = string.Format(@"
select {0} JsonData 
from Documents 
where CollectionName = '{2}' {3} 
{4}", 
queryBuilder.limit, 
"", 
documentCollection.CollectionName, 
queryBuilder.Where, 
queryBuilder.orderby);


            IEnumerable<ReadDocument> result = documentCollection.ExecuteQuery(sql);

            if (typeof(TResult) == typeof(string))
                return (IEnumerable<TResult>)StringEnumerator(result);
            else
                return DocumentEnumerator<TResult>(result);
        }

        private IEnumerable<string> StringEnumerator(IEnumerable<ReadDocument> result)
        {
            return result.Select(d => d.Json);
        }


    }

    public static class QueryableExtensions
    {
        public static IQueryable<string> AsJson<T>(this IQueryable<T> self)
        {
            return self.Select(i => i.ToJson());
        }

        private static string ToJson<T>(this T self)
        {
            throw new Exception("");
            return JsonSerializer<T>.Default.SerializeToString(self);
        }
    }
}