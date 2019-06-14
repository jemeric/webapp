using HotChocolate.Language;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace webapp.Services.GraphQL
{
    public class TimeSpanType : ScalarType
    {
        Regex durationRx = new Regex(@"^(-){0,1}P(?!$)((\d+Y)|(\d+\.\d+Y$))?((\d+M)|(\d+\.\d+M$))?((\d+W)|(\d+\.\d+W$))?((\d+D)|(\d+\.\d+D$))?(T(?=\d)((\d+H)|(\d+\.\d+H$))?((\d+M)|(\d+\.\d+M$))?(\d+(\.\d+)?S)?)??$",
            RegexOptions.Compiled);

        public TimeSpanType() : base("TimeSpan")
        {

        }

        public override Type ClrType => typeof(TimeSpan);

        public override bool IsInstanceOfType(IValueNode literal)
        {
            if (literal == null)
            {
                throw new ArgumentNullException(nameof(literal));
            }

            if (literal is NullValueNode)
            {
                return true;
            }

            return literal is StringValueNode stringLiteral
                && TryParseLiteral(stringLiteral, out _);
        }

        public override object ParseLiteral(IValueNode literal)
        {
            if (literal == null)
            {
                throw new ArgumentNullException(nameof(literal));
            }

            if (literal is StringValueNode stringLiteral
                && TryParseLiteral(stringLiteral, out object obj))
            {
                return obj;
            }

            if(literal is NullValueNode)
            {
                return null;
            }

            throw new ScalarSerializationException($"Cannot parse literal {literal.GetType()}");
        }

        private bool TryParseLiteral(StringValueNode literal, out object obj)
        {
            if(literal.Value != null && durationRx.IsMatch(literal.Value))
            {
                obj = XmlConvert.ToTimeSpan(literal.Value);
                return true;
            }
            obj = null;
            return false;
        }

        public override IValueNode ParseValue(object value)
        {
            if (TryParseValue(value, out IValueNode valueNode))
            {
                return valueNode;
            }

            throw new ScalarSerializationException($"Cannot parse value {value.GetType()}");
        }

        protected bool TryParseValue(object value, out IValueNode valueNode)
        {
            if (value == null)
            {
                valueNode = new NullValueNode(null);
                return true;
            }

            if (TrySerialize(value, out string serializedValue))
            {
                valueNode = new StringValueNode(serializedValue);
                return true;
            }

            valueNode = null;
            return false;

        }

        private bool TrySerialize(object value, out string serializedValue)
        {
            if (value == null)
            {
                serializedValue = null;
                return true;
            }

            if (value is TimeSpan timeSpan)
            {
                serializedValue = XmlConvert.ToString(timeSpan);
                return true;
            }

            serializedValue = null;
            return false;
        }

        public override object Serialize(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (TrySerialize(value, out string serializedValue))
            {
                return serializedValue;
            }

            throw new ScalarSerializationException("The specified value cannot be serialized by the ISODurationType");
        }

        public override bool TryDeserialize(object serialized, out object value)
        {
            if (serialized is null)
            {
                value = null;
                return true;
            }

            if (serialized is string s && TryParseLiteral(new StringValueNode(s), out object d))
            {
                value = d;
                return true;
            }

            value = null;
            return false;
        }
    }
}
