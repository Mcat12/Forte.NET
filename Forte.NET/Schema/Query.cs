using System.Collections.Generic;
using System.Linq;
using Forte.NET.Database;
using GraphQL.Types;

namespace Forte.NET.Schema {
    public class Query : ObjectGraphType {
        public Query() {
            Field<ListGraphType<SongType>>(
                "songs",
                resolve: context => Songs(context.ForteDbContext())
            );
            Field<ListGraphType<AlbumType>>(
                "albums",
                resolve: context => Albums(context.ForteDbContext())
            );
            Field<ListGraphType<ArtistType>>(
                "artists",
                resolve: context => Artists(context.ForteDbContext())
            );
        }

        private static List<Song> Songs(ForteDbContext context) => context.Songs.ToList();

        private static List<Album> Albums(ForteDbContext context) => context.Albums.ToList();

        private static List<Artist> Artists(ForteDbContext context) => context.Artists.ToList();
    }
}
