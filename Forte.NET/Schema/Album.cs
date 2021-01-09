using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Forte.NET.Database;
using GraphQL.Types;

namespace Forte.NET.Schema {
    [Table("album")]
    public class Album {
        [Column("id", TypeName = "BLOB")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;
    }

    public sealed class AlbumType : ObjectGraphType<Album> {
        public AlbumType() {
            Field(album => album.Id);
            Field(album => album.Name);
            Field<ListGraphType<SongType>>("songs", resolve: context => {
                var dbContext = context.ForteDbContext();
                var album = context.Source;
                return dbContext.Songs.Where(song => song.AlbumId == album.Id);
            });
        }
    }
}
