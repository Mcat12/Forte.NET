using System.ComponentModel;

namespace Forte.NET.Schema {
    public class SortParams {
        public SortBy SortBy { get; set; }

        [DefaultValue(false)]
        public bool Reverse { get; set; }

        public string? Filter { get; set; }
    }

    public enum SortBy {
        RecentlyAdded,
        Lexicographically,
        RecentlyPlayed
    }
}
