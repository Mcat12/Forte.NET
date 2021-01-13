using System;
using System.Collections.Generic;
using System.Linq;
using Forte.NET.Database;
using GraphQL;
using GraphQL.Types;

namespace Forte.NET.Schema {
    public class Query : ObjectGraphType {
        public Query() {
            Field<NonNullGraphType<SongType>>(
                "song",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }
                ),
                resolve: context => SongById(context.ForteDbContext(), context.GetArgument<Guid>("id"))
            );
            Field<ListGraphType<SongType>>(
                "songs",
                resolve: context => Songs(context.ForteDbContext())
            );
            Field<NonNullGraphType<AlbumType>>(
                "album",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }
                ),
                resolve: context => AlbumById(context.ForteDbContext(), context.GetArgument<Guid>("id"))
            );
            Field<ListGraphType<AlbumType>>(
                "albums",
                resolve: context => Albums(context.ForteDbContext())
            );
            Field<NonNullGraphType<ArtistType>>(
                "artist",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }
                ),
                resolve: context => ArtistById(context.ForteDbContext(), context.GetArgument<Guid>("id"))
            );
            Field<ListGraphType<ArtistType>>(
                "artists",
                resolve: context => Artists(context.ForteDbContext())
            );
        }

        private static Song SongById(ForteDbContext context, Guid id) => context.Songs.Find(id);

        private static List<Song> Songs(ForteDbContext context) => context.Songs.ToList();

        private static Album AlbumById(ForteDbContext context, Guid id) => context.Albums.Find(id);

        private static List<Album> Albums(ForteDbContext context) => context.Albums.ToList();

        private static Artist ArtistById(ForteDbContext context, Guid id) => context.Artists.Find(id);

        private static List<Artist> Artists(ForteDbContext context) => context.Artists.ToList();
    }
}
