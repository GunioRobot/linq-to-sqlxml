using System;
using System.Collections.Generic;
using System.Linq;
using LinqToSqlXml;
using System.Diagnostics;

namespace ProjectionSample
{
    public class Projection
    {
        public decimal OrderTotal { get; set; }
        public Guid CustomerId { get; set; }
        public IEnumerable<ProjectionD> OrderDetails { get; set; }
    }
    
    public class ProjectionD
    {
        public decimal LineTotal { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var ctx = new DocumentContext("main");
            ctx.EnsureDatabaseExists();

            var query = (
                from order in ctx.GetCollection<Order>().AsQueryable()
                where order.OrderTotal > 0
                //  where order.OrderDate < DateTime.Now
                where order.Status == OrderStatus.Shipped
                select order
                );

            //touch the db
            var x = query.Take(1).ToList();

            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Starting...");
            sw.Start();
            var result = query.ToList();
            sw.Stop();


            Console.WriteLine("feteched {0} records", result.Count);
            //foreach (var order in result)
            //{
            //    Console.WriteLine("{0} {1}", order.OrderTotal, order.OrderDetails.Count());
            //}

            Console.WriteLine(sw.Elapsed);
            Console.ReadLine();
            return;

            for (int i = 0; i < 5000; i++)
            {
                Console.WriteLine(i);
                var someCompany = new Customer
                                  {
                                      Address = new Address
                                                    {
                                                        City = "Stora mellösa",
                                                        Line1 = "Linfrövägen " + i,
                                                        State = "T",
                                                        ZipCode = "71572"
                                                    },
                                      Name = "Precio" + i,

                                  };

                ctx.GetCollection<Customer>().Add(someCompany);

                var someOrder = new Order
                                       {
                                           CustomerId = Guid.NewGuid(),
                                           OrderDate = DateTime.Now,
                                           ShippingDate = DateTime.Now,
                                           OrderDetails = new List<OrderDetail>() ,
                                           ShippingAddress = new Address
                                                                 {
                                                                     City = "a",
                                                                     Line1 = "b",
                                                                     ZipCode = "c"
                                                                 },
                                           Status = OrderStatus.Shipped,
                                       };
                for (int j = 0; j < 1; j++)
                {
                    someOrder.OrderDetails.Add(new OrderDetail
                    {
                        ItemPrice = i,
                        ProductNo = "x" + i,
                        Quantity = i,
                    });
                }

                ctx.GetCollection<Order>().Add(someOrder);
                //var result = DocumentSerializer.Serialize(specialOrder);
                //Console.WriteLine(result.ToString());
                //ctx.GetCollection<Order>().Add(specialOrder);
                //ctx.SaveChanges();
                //var des = DocumentDeserializer.Deserialize(result);






                //    var address = new Address()
                //                      {
                //                          City = "Örebro",
                //                          Line1 = "blabla",
                //                          ZipCode = "" + i ,
                //                      };

                //    ctx.GetCollection<Address>().Add(address);
                //}
                
            }
            ctx.SaveChanges();
        //    Console.ReadLine();
        }
    }

    public interface IAmDocument
    {
    }

    public class SpecialOrder : Order, IAmDocument
    {
    }

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

        public Order NewOrder()
        {
            return new Order
            {
                CustomerId = this.Id,
            };
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