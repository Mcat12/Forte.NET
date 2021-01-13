using System;
using GraphQL.Types;

namespace Forte.NET.Schema {
    public class UserStats {
        public Guid Id { get; set; }

        public TimeWrapper? LastPlayed { get; set; }

        public UserStats(Guid id, TimeWrapper? lastPlayed) {
            Id = id;
            LastPlayed = lastPlayed;
        }
    }

    public sealed class UserStatsType : ObjectGraphType<UserStats> {
        public UserStatsType() {
            Name = "UserStats";
            Field(stats => stats.Id);
            Field(stats => stats.LastPlayed, true);
        }
    }
}
