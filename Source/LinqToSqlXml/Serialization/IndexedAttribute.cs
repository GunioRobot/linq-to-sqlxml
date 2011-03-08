using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToSqlXml
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class IndexedAttribute : Attribute
    {
    }
}
