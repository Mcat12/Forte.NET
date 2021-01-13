using System;
using System.ComponentModel.DataAnnotations.Schema;
using GraphQL.Types;

namespace Forte.NET.Schema {
    [Table("artist")]
    public class Artist {
        [Column("id", TypeName = "BLOB")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("time_added", TypeName = "TIMESTAMP")]
        public TimeWrapper TimeAdded { get; set; }

        [Column("last_played", TypeName = "TIMESTAMP")]
        public TimeWrapper? LastPlayed { get; set; }
    }

    public sealed class ArtistType : ObjectGraphType<Artist> {
        public ArtistType() {
            Name = "Artist";
            Field(artist => artist.Id);
            Field(artist => artist.Name);
            Field(artist => artist.TimeAdded);
            Field(artist => artist.LastPlayed, true);
        }
    }
}
