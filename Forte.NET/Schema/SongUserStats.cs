using GraphQL.Types;

namespace Forte.NET.Schema {
    public class SongUserStats {
        public string Id { get; set; }

        public int PlayCount { get; set; }

        public bool Liked { get; set; }

        public SongUserStats(string id, int playCount, bool liked) {
            Id = id;
            PlayCount = playCount;
            Liked = liked;
        }
    }

    public sealed class SongUserStatsType : ObjectGraphType<SongUserStats> {
        public SongUserStatsType() {
            Name = "SongUserStats";
            Field(stats => stats.Id, type: typeof(IdGraphType));
            Field(stats => stats.PlayCount);
            Field(stats => stats.Liked);
        }
    }
}
