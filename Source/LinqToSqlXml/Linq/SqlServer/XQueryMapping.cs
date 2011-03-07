using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml;

namespace LinqToSqlXml.SqlServer
{
    public class OperatorInfo
    {
        public string Code { get; set; }
        public bool IsBool { get; set; }
    }
    public static class XQueryMapping
    {
        public static readonly Dictionary<ExpressionType, OperatorInfo> Operators = new Dictionary<ExpressionType, OperatorInfo>
                {
                    {ExpressionType.AndAlso, new OperatorInfo{Code = "and" , IsBool = true }},
                    {ExpressionType.OrElse, new OperatorInfo{Code = "or" , IsBool = true }},
                    {ExpressionType.NotEqual, new OperatorInfo{Code = "!=" , IsBool = true }},
                    {ExpressionType.LessThan, new OperatorInfo{Code = "<" , IsBool = true }},
                    {ExpressionType.LessThanOrEqual, new OperatorInfo{Code = "<=" , IsBool = true }},
                    {ExpressionType.GreaterThan, new OperatorInfo{Code = ">" , IsBool = true }},
                    {ExpressionType.GreaterThanOrEqual, new OperatorInfo{Code = ">=" , IsBool = true }},
                    {ExpressionType.Equal, new OperatorInfo{Code = "=" , IsBool = true }},
                    {ExpressionType.Add, new OperatorInfo{Code = "+" , IsBool = false }},
                    {ExpressionType.Subtract, new OperatorInfo{Code = "-" , IsBool = false }},
                    {ExpressionType.Divide, new OperatorInfo{Code = "/" , IsBool = false }},
                    {ExpressionType.Multiply, new OperatorInfo{Code = "*" , IsBool = false }},
                };


        public static readonly Dictionary<string, string> Functions = new Dictionary<string, string>
                                                                           {
                                                                               {"Sum", "fn:sum"},
                                                                               {"Max", "fn:max"},
                                                                               {"Min", "fn:min"},
                                                                               {"Average", "fn:avg"},
                                                                           };

        public static readonly string xsTrue = "fn:true()";
        public static readonly string xsFalse = "fn:false()";

        public static string BuildLiteral(object value)
        {
            if (value is string)
                return "\"" + SerializeString((string)value) + "\"";
            if (value is int)
                return string.Format("xs:int({0})", SerializeDecimal((int)value));
            if (value is decimal)
                return string.Format("xs:decimal({0})", SerializeDecimal((decimal)value));
            if (value is DateTime)
                return string.Format("xs:dateTime({0})", SerializeDateTime((DateTime)value));
            if (value is bool)
                if ((bool)value)
                    return XQueryMapping.xsTrue;
                else
                    return XQueryMapping.xsFalse;

            return value.ToString();
        }



        public static string SerializeDateTime(DateTime value)
        {
            return XmlConvert.ToString(value, XmlDateTimeSerializationMode.Local);
        }

        public static string SerializeInt(int value)
        {
            return XmlConvert.ToString(value);
        }

        public static string SerializeDouble(double value)
        {
            return XmlConvert.ToString(value);
        }

        public static string SerializeDecimal(decimal value)
        {
            return XmlConvert.ToString(value);
        }

        public static string SerializeBool(bool value)
        {
            return XmlConvert.ToString(value);
        }

        public static string SerializeGuid(Guid value)
        {
            return XmlConvert.ToString(value);
        }

        public static string SerializeString(string value)
        {
            return value;
        }
    }
}
