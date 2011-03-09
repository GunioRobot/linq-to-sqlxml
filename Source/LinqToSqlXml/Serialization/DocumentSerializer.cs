using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace LinqToSqlXml
{
    public static class DocumentSerializer
    {
        private static readonly XNamespace ns = XNamespace.Get("urn:LinqToSqlXml");
        public static XElement Serialize(object item)
        {
            var root = new XElement(ns+"document",new XAttribute(XNamespace.Xmlns + "x", ns));
            Serialize(item, root);
            return root;
        }


        private static void Serialize(object value, XElement ownerTag)
        {
            if (value == null)
            {
                ownerTag.Add(new XAttribute(ns+"type", "null"));
                return;
            }
            if (value is string)
            {
                SerializeString((string) value,ownerTag);
                return;
            }
            if (value is Guid || value is Guid?)
            {
                SerializeGuid((Guid)value, ownerTag);
                return;
            }
            if (value is bool || value is bool?)
            {
                SerializeBool((bool) value,ownerTag);
                return;
            }
            if (value is decimal || value is decimal?)
            {
                SerializeDecimal((decimal) value,ownerTag);
                return;
            }
            if (value is double || value is double?)
            {
                SerializeDouble((double) value,ownerTag);
                return;
            }
            if (value is int || value is int?)
            {
                SerializeInt((int)value, ownerTag);
                return;
            }
            if (value is DateTime || value is DateTime?)
            {
                SerializeDateTime((DateTime)value,ownerTag);
                return;
            }
            if (value is IEnumerable)
            {
                SerializeCollection(value, ownerTag);
                return;
            }
            if (value is Enum)
            {
                var intValue = (int) value;
                SerializeInt(intValue, ownerTag);
                return;
            }
            SerializeObject(value, ownerTag);
        }

        private static void SerializeObject(object value, XElement ownerTag)
        {
            Type itemType = value.GetType();
            ownerTag.Add(new XAttribute(ns+"type", itemType.SerializedName()));
            AddMetaData(ownerTag, itemType);

            PropertyInfo[] properties = itemType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                //if (!(property.IsDefined(typeof(IndexedAttribute),true) || property.DeclaringType.IsDefined(typeof(IndexedAttribute),true)))
                //    continue;

                var propertyTag = new XElement(property.Name);
                object propertyValue = property.GetValue(value, null);

                Serialize(propertyValue, propertyTag);
                ownerTag.Add(propertyTag);
            }
        }

        private static void SerializeCollection(object value, XElement ownerTag)
        {
            ownerTag.Add(new XAttribute(ns+"type", "collection"));
            foreach (object childValue in (IEnumerable)value)
            {
                var collectionElement = new XElement("element");
                Serialize(childValue, collectionElement);
                ownerTag.Add(collectionElement);
            }
        }

        private static void AddMetaData(XElement ownerTag, Type itemType)
        {
            var metaTags = new XElement("__meta");

            Type currentType = itemType;
            while (currentType != null && currentType != typeof(object))
            {
                if (currentType != itemType)
                {
                    var typeTag = new XElement(ns+"type", currentType.SerializedName());
                    metaTags.Add(typeTag);
                }

                Type[] interfaces = currentType.GetInterfaces();
                foreach (Type interfaceType in interfaces)
                {
                    var interfaceTag = new XElement(ns+"type", interfaceType.SerializedName());
                    metaTags.Add(interfaceTag);
                }
                currentType = currentType.BaseType;
            }

            //only add metadata if metadata exists
            if (metaTags.Elements().Count() > 0)
                ownerTag.Add(metaTags);
        }

        public static string GetSerializedTypeName(Type type)
        {
            if (type == typeof(bool))
                return "value";

            if (type == typeof(int))
                return "value";

            if (type == typeof(double))
                return "value";

            if (type == typeof(decimal))
                return "value";

            if (type == typeof(DateTime))
                return "value";

            if (type == typeof(string))
                return "value";

            if (type == typeof(Guid))
                return "value";

            if (typeof(IEnumerable).IsAssignableFrom(type))
                return "collection";

            return type.SerializedName();
        }


        private static void SerializeDateTime(DateTime value, XElement owner)
        {
            owner.Add(new XAttribute(ns+"type", "value"));
            owner.Add(
                new XElement(ns+"dt", XmlConvert.ToString(value, XmlDateTimeSerializationMode.Local))
            );
        }

        private static void SerializeInt(int value, XElement owner)
        {
            owner.Add(new XAttribute(ns+"type", "value"));
            owner.Add(
             new XElement(ns+"int", XmlConvert.ToString(value)));
        }

        private static void SerializeDouble(double value, XElement owner)
        {
            owner.Add(new XAttribute(ns+"type", "value"));
            owner.Add(
             new XElement(ns+"dec", XmlConvert.ToString(value)));
        }

        private static void SerializeDecimal(decimal value, XElement owner)
        {
            owner.Add(new XAttribute(ns+"type", "value"));
            owner.Add(
             new XElement(ns+"dec", XmlConvert.ToString(value)));
        }

        private static void SerializeBool(bool value, XElement owner)
        {
            owner.Add(new XAttribute(ns+"type", "value"));
            owner.Add(
             new XElement(ns+"bool", XmlConvert.ToString(value)));
        }

        private static void SerializeGuid(Guid value, XElement owner)
        {
            owner.Add(new XAttribute(ns+"type", "value"));
            owner.Add(new XElement( ns + "str",
             XmlConvert.ToString(value)));
        }

        private static void SerializeString(string value, XElement owner)
        {
            owner.Add(new XAttribute(ns+"type", "value"));
            owner.Add(new XElement(ns+"str", value));
        }
    }

    public static class TypeExtensions
    {
        public static string SerializedName(this Type self)
        {
            return string.Format("{0}, {1}", self.FullName,
                                 self.Assembly.FullName.Substring(0, self.Assembly.FullName.IndexOf(",")));
        }
    }
}