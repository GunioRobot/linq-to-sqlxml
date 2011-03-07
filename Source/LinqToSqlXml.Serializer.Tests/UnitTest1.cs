using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToSqlXml;

namespace LinqToSqlXml.Serializer.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_serialize_object()
        {
            var item = new Order
            {
                CustomerId = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                ShippingDate = DateTime.Now,
                OrderDetails = new List<OrderDetail>
                                                              {
                                                                  new OrderDetail
                                                                      {
                                                                          ItemPrice = 123,
                                                                          ProductNo = "banan",
                                                                          Quantity = 432
                                                                      },
                                                                      new OrderDetail
                                                                      {
                                                                          ItemPrice = 123,
                                                                          ProductNo = "äpple",
                                                                          Quantity = 432
                                                                      },
                                                                      new OrderDetail
                                                                      {
                                                                          ItemPrice = 123,
                                                                          ProductNo = "gurka",
                                                                          Quantity = 432
                                                                      },
                                                              },
                ShippingAddress = new Address
                {
                    City = "gdfgdf",
                    Line1 = "dfgdgdfgd",
                    ZipCode = "gdfgdfgd"
                },
                Status = OrderStatus.Shipped,
            };

            var result = DocumentSerializer.Serialize(item);


        }

    }

    public class Order
    {
        [DocumentId]
        public Guid Id { get; private set; }

        public Order()
        {
            this.Id = Guid.NewGuid();
        }

        [Indexed]
        public DateTime OrderDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public Guid CustomerId { get; set; }
        public Address ShippingAddress { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [Indexed]
        public decimal OrderTotal
        {
            get { return OrderDetails.Sum(d => d.Quantity * d.ItemPrice); }
        }

        [Indexed]
        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus
    {
        PreOrder,
        Confirmed,
        Shipped,
        Cancelled,
    }

    public class OrderDetail
    {
        public decimal Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public string ProductNo { get; set; }
    }

    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}
