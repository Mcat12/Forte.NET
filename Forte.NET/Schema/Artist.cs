using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Forte.NET.Database;
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

        public ICollection<Song> Songs { get; set; } = null!;
    }

    public sealed class ArtistType : ObjectGraphType<Artist> {
        public ArtistType() {
            Name = "Artist";
            Field(artist => artist.Id);
            Field(artist => artist.Name);
            Field(artist => artist.TimeAdded);
            Field("stats", artist => new UserStats($"stats:{artist.Id}", artist.LastPlayed),
                type: typeof(NonNullGraphType<UserStatsType>));
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<AlbumType>>>>(
                "albums",
                resolve: context => {
                    var dbContext = context.ForteDbContext();
                    var artist = context.Source;
                    return dbContext.Albums
                        .Where(album => artist.Id == album.ArtistId)
                        .OrderByDescending(album => album.ReleaseYear ?? 0);
                }
            );
        }
    }
}
