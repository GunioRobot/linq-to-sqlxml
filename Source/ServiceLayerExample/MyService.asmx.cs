using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LinqToSqlXml;

namespace ServiceLayerExample
{
    /// <summary>
    /// Summary description for MyService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MyService : System.Web.Services.WebService
    {
        [WebMethod]
        public string[] GetCustomers(int count,string city)
        {
            var ctx = new LinqToSqlXml.DocumentContext("main");
            var customers = ctx.GetCollection<Customer>().AsQueryable()
                .Where(c => c.Address.City == city)
                .Take(count)
                .AsJson();

            return customers.ToArray();
        }

        [WebMethod]
        public string GetObjectByName(string name)
        {
            var ctx = new LinqToSqlXml.DocumentContext("main");
            var customer = ctx.GetCollection<Customer>().AsQueryable()
                .Where(c => c.Name == name)
                .AsJson()
                .First();

            return customer;
        }

        [WebMethod]
        public string GetObject(string id)
        {
            var gid = Guid.Parse(id);
            var ctx = new LinqToSqlXml.DocumentContext("main");
            var customer = ctx.GetCollection<Customer>().AsQueryable()
                .Where(c => c.Id == gid)
                .Take(1) //first() is not yet implemented in the linq provider
                .AsJson()
                .ToList()
                .First();

            return customer;
        }

        [WebMethod]
        public void Save(Customer customer)
        {
            var ctx = new LinqToSqlXml.DocumentContext("main");
            ctx.GetCollection<Customer>().Add(customer);
            ctx.SaveChanges();
        }

        [WebMethod]
        public void SaveFromJson(string json)
        {
            var ctx = new LinqToSqlXml.DocumentContext("main");
            ctx.GetCollection<Customer>().Add(json);
            ctx.SaveChanges();
        }
    }

    [Serializable]
    public class Customer
    {
        [DocumentId]
        public Guid Id { get; private set; }

        public Customer()
        {
            this.Id = Guid.NewGuid();
        }

        [Indexed]
        public string Name { get; set; }

        [Indexed]
        public Address Address { get; set; }        
    }

    [Serializable]
    public class Address
    {
        [Indexed]
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string State { get; set; }
        [Indexed]
        public string City { get; set; }
        [Indexed]
        public string ZipCode { get; set; }
    }
}
