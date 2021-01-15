using System;
using System.Linq;
using Forte.NET.Database;
using GraphQL;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;

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
            Field<NonNullGraphType<AlbumType>>(
                "album",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }
                ),
                resolve: context => AlbumById(context.ForteDbContext(), context.GetArgument<Guid>("id"))
            );
            Field<NonNullGraphType<ArtistType>>(
                "artist",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }
                ),
                resolve: context => ArtistById(context.ForteDbContext(), context.GetArgument<Guid>("id"))
            );

            Connection<SongType>()
                .Name("songs")
                .Resolve(context => GetConnection(context, context.ForteDbContext().Songs));
            Connection<AlbumType>()
                .Name("albums")
                .Resolve(context => GetConnection(context, context.ForteDbContext().Albums));
            Connection<ArtistType>()
                .Name("artists")
                .Resolve(context => GetConnection(context, context.ForteDbContext().Artists));
        }

        private static Connection<T> GetConnection<T>(
            IResolveFieldContext context,
            IQueryable<T> dbSet
        ) where T : class {
            var first = context.GetArgument("first", 25);
            var after = context.GetArgument<string?>("after");
            var offset = after == null ? 0 : int.Parse(after);

            var tableSize = dbSet.Count();
            var hasNextPage = offset + first < tableSize;
            var results = dbSet.Skip(offset).Take(first).ToList();
            var edges = results
                .Zip(Enumerable.Range(1, Int32.MaxValue))
                .Select((item) => new Edge<T> {
                    Cursor = (offset + item.Second).ToString(),
                    Node = item.First
                })
                .ToList();

            return new Connection<T> {
                Edges = edges,
                PageInfo = new PageInfo {
                    HasNextPage = hasNextPage
                },
                TotalCount = tableSize
            };
        }

        private static Song SongById(ForteDbContext context, Guid id) => context.Songs.Find(id);

        private static Album AlbumById(ForteDbContext context, Guid id) => context.Albums.Find(id);

        private static Artist ArtistById(ForteDbContext context, Guid id) => context.Artists.Find(id);
    }
}
