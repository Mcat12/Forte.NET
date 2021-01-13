using GraphQL.Types;

namespace Forte.NET.Schema {
    public class UserStats {
        public string Id { get; set; }

        public TimeWrapper? LastPlayed { get; set; }

        public UserStats(string id, TimeWrapper? lastPlayed) {
            Id = id;
            LastPlayed = lastPlayed;
        }
    }

    public sealed class UserStatsType : ObjectGraphType<UserStats> {
        public UserStatsType() {
            Name = "UserStats";
            Field(stats => stats.Id, type: typeof(IdGraphType));
            Field(stats => stats.LastPlayed, true);
        }
    }
}
