using System;
using GraphQL;
using GraphQL.Utilities;

namespace Forte.NET.Schema {
    public class ForteSchema : GraphQL.Types.Schema {
        public ForteSchema() {
            ValueConverter.Register<DateTime, TimeWrapper>(time => new TimeWrapper(time));
            RegisterValueConverter(new TimeWrapperAstValueConverter());
            GraphTypeTypeRegistry.Register<TimeWrapper, TimeWrapperType>();

            Query = new Query();
        }
    }
}
