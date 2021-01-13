using System;
using System.ComponentModel.DataAnnotations.Schema;
using Forte.NET.Database;
using GraphQL.Types;

namespace Forte.NET.Schema {
    [Table("song")]
    public class Song {
        [Column("id", TypeName = "BLOB")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("track_number")]
        public int TrackNumber { get; set; }

        [Column("disk_number")]
        public int DiskNumber { get; set; }

        [Column("last_played", TypeName = "TIMESTAMP")]
        public TimeWrapper? LastPlayed { get; set; }

        [Column("play_count")]
        public int PlayCount { get; set; }

        [Column("liked", TypeName = "BOOLEAN")]
        public bool Liked { get; set; }

        [Column("duration")]
        public int Duration { get; set; }

        [Column("time_added", TypeName = "TIMESTAMP")]
        public TimeWrapper TimeAdded { get; set; }

        [Column("album_id", TypeName = "BLOB")]
        public Guid AlbumId { get; set; }

        [Column("path", TypeName = "BLOB")]
        public string Path { get; set; } = null!;
    }

    public sealed class SongType : ObjectGraphType<Song> {
        public SongType() {
            Name = "Song";
            Field(song => song.Id);
            Field(song => song.Name);
            Field(song => song.TrackNumber);
            Field(song => song.DiskNumber);
            Field(song => song.PlayCount);
            Field(song => song.Liked);
            Field(song => song.Duration);
            Field(song => song.TimeAdded);
            Field("stats", song => new UserStats(song.Id, song.LastPlayed), type: typeof(UserStatsType));
            Field<NonNullGraphType<AlbumType>>("album", resolve: context => {
                var dbContext = context.ForteDbContext();
                var song = context.Source;
                return dbContext.Albums.Find(song.AlbumId);
            });
        }
    }
}
