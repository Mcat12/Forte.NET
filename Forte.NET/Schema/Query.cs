using System.Collections.Generic;
using System.Linq;
using Forte.NET.Database;
using GraphQL;

namespace Forte.NET.Schema {
    public class Query {
        public string Test() => "test";

        public int IntTest() => 42;

        public List<Song> Songs(IResolveFieldContext<Query> context) {
            var dbContext = context.ForteDbContext();
            return dbContext.Songs.ToList();
        }
    }
}
