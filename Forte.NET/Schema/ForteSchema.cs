namespace Forte.NET.Schema {
    public class ForteSchema : GraphQL.Types.Schema {
        public ForteSchema() {
            Query = new Query();
        }
    }
}
