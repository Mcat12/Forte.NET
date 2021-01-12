using System;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Forte.NET.Schema {
    /// <summary>
    /// Expose time as a unix timestamp under the GraphQL scalar type "Time"
    /// </summary>
    public struct TimeWrapper {
        private DateTime _time;

        public TimeWrapper(DateTime time) {
            _time = time;
        }

        public DateTime GetTime() {
            return _time;
        }
    }

    public class TimeWrapperType : ScalarGraphType {
        public TimeWrapperType() {
            Name = "Time";
        }

        public override object? ParseLiteral(IValue value) {
            return value switch {
                TimeWrapperValue timeValue => timeValue.Value,
                StringValue stringValue => ParseValue(stringValue.Value),
                _ => null
            };
        }

        public override object ParseValue(object value) {
            return ValueConverter.ConvertTo(value, typeof(TimeWrapper));
        }

        public override object Serialize(object value) {
            var time = (TimeWrapper) ParseValue(value);
            return Math.Floor((time.GetTime() - DateTime.UnixEpoch).TotalSeconds);
        }
    }

    public class TimeWrapperValue : ValueNode<TimeWrapper> {
        public TimeWrapperValue(TimeWrapper value) {
            Value = value;
        }

        protected override bool Equals(ValueNode<TimeWrapper> node) {
            return Value.Equals(node.Value);
        }
    }

    public class TimeWrapperAstValueConverter : IAstFromValueConverter {
        public bool Matches(object value, IGraphType type) {
            return value is TimeWrapper;
        }

        public IValue Convert(object value, IGraphType type) {
            return new TimeWrapperValue((TimeWrapper) value);
        }
    }
}
