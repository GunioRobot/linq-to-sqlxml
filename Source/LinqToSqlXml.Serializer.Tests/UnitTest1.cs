using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToSqlXml;
using ServiceStack.Text;
using System.Dynamic;
using System.Runtime.Serialization;

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
            var item = new SpecialOrder
            {
                Foo = 3,
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
            var result2 = JsonSerializer.SerializeToString(item);

        }

    }

    [DataContract]
    public class SpecialOrder : Order
    {
        [DataMember]
        public int Foo { get; set; }
    }

    [KnownType(typeof(SpecialOrder))]
    [DataContract]
    public class Order
    {
        [DocumentId]
        [DataMember]
        public Guid Id { get; private set; }

        public Order()
        {
            this.Id = Guid.NewGuid();
        }

        [Indexed]
        [DataMember]
        public DateTime OrderDate { get; set; }

        [DataMember]
        public DateTime? ShippingDate { get; set; }

        [DataMember]
        public Guid CustomerId { get; set; }

        [DataMember]
        public Address ShippingAddress { get; set; }

        [DataMember]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [Indexed]
        [DataMember]
        public decimal OrderTotal
        {
            get { return OrderDetails.Sum(d => d.Quantity * d.ItemPrice); }
        }

        [Indexed]
        [DataMember]
        public OrderStatus Status { get; set; }
    }

    [DataContract]
    public enum OrderStatus
    {
        [EnumMember]
        PreOrder,
        [EnumMember]
        Confirmed,
        [EnumMember]
        Shipped,
        [EnumMember]
        Cancelled,
    }

    [DataContract]
    public class OrderDetail
    {
        [DataMember]
        public decimal Quantity { get; set; }
        [DataMember]
        public decimal ItemPrice { get; set; }
        [DataMember]
        public string ProductNo { get; set; }
    }

    [DataContract]
    public class Address
    {
        [DataMember]
        public string Line1 { get; set; }
        [DataMember]
        public string Line2 { get; set; }
        [DataMember]
        public string Line3 { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
    }
}
