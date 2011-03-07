Linq to SqlXml
==============
A document database emulator on top of Microsoft SQL Server XML columns.

Version Alpha 0.1


Linq to SqlXml allows you to persist and read POCO entities to and from the Sql Server using XML type column.

Entities can be queried and projected using Linq.

This Linq query:

    var query = (
        from order in ctx.GetCollection<Order>().AsQueryable()
        where order.OrderTotal > 0
        where order.ShippingDate != null
        where order.ShippingAddress.Line1 != "foo" && order.ShippingAddress.Line1 != "bar"
        where order.Status == OrderStatus.Confirmed
        select order
        ).Take(100);

Will be transformed into:

    select top 100 Id,DocumentData 
    from Documents 
    where CollectionName = 'Order' and 
    (documentdata.exist('/document[
    
         Status[. = xs:int(1)] and 
         ShippingAddress/Line1[. != "foo" and . != "bar"] and 
         ShippingDate/@type[.!="null"] and 
         OrderTotal[. > xs:decimal(0)]

    ]')) = 1 

