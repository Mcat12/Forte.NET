using System;
using System.Linq;
using System.Linq.Expressions;
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
                .Argument<AutoRegisteringInputObjectGraphType<SortParams>>("sort", "")
                .Resolve(context => GetConnection(
                    context,
                    context.ForteDbContext().Songs,
                    name => song => song.Name.ToLower().Contains(name.ToLower()),
                    song => song.Name,
                    song => song.TimeAdded,
                    song => song.LastPlayed
                ));
            Connection<AlbumType>()
                .Name("albums")
                .Argument<AutoRegisteringInputObjectGraphType<SortParams>>("sort", "")
                .Resolve(context => GetConnection(
                    context,
                    context.ForteDbContext().Albums,
                    name => album => album.Name.ToLower().Contains(name.ToLower()),
                    album => album.Name,
                    album => album.TimeAdded,
                    album => album.LastPlayed
                ));
            Connection<ArtistType>()
                .Name("artists")
                .Argument<AutoRegisteringInputObjectGraphType<SortParams>>("sort", "")
                .Resolve(context => GetConnection(
                    context,
                    context.ForteDbContext().Artists,
                    name => artist => artist.Name.ToLower().Contains(name.ToLower()),
                    artist => artist.Name,
                    artist => artist.TimeAdded,
                    artist => artist.LastPlayed
                ));
        }

        private static Connection<T> GetConnection<T>(
            IResolveFieldContext context,
            IQueryable<T> dbSet,
            Func<string, Expression<Func<T, bool>>> filterExpression,
            Expression<Func<T, object>> orderByName,
            Expression<Func<T, object>> orderByTimeAdded,
            Expression<Func<T, object?>> orderByLastPlayed
        ) where T : class {
            var first = context.GetArgument("first", 25);
            var after = context.GetArgument<string?>("after");
            var offset = after == null ? 0 : int.Parse(after);
            var sort = context.GetArgument("sort", new SortParams { SortBy = SortBy.Lexicographically });

            var baseQuery = sort.Filter != null ? dbSet.Where(filterExpression(sort.Filter)) : dbSet;

            var query = sort.SortBy switch {
                SortBy.RecentlyAdded => sort.Reverse
                    ? baseQuery.OrderByDescending(orderByTimeAdded)
                    : baseQuery.OrderBy(orderByTimeAdded),
                SortBy.Lexicographically => sort.Reverse
                    ? baseQuery.OrderByDescending(orderByName)
                    : baseQuery.OrderBy(orderByName),
                SortBy.RecentlyPlayed => sort.Reverse
                    ? baseQuery.OrderByDescending(orderByLastPlayed)
                    : baseQuery.OrderBy(orderByLastPlayed),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(sort.SortBy),
                    "Missing a SortBy case in GetConnection"
                )
            };

            var results = query.Skip(offset).Take(first).ToList();
            var edges = results
                .Zip(Enumerable.Range(1, Int32.MaxValue))
                .Select((item) => new Edge<T> {
                    Cursor = (offset + item.Second).ToString(),
                    Node = item.First
                })
                .ToList();
            var totalCount = baseQuery.Count();
            var hasNextPage = offset + first < totalCount;

            return new Connection<T> {
                Edges = edges,
                PageInfo = new PageInfo {
                    HasNextPage = hasNextPage
                },
                TotalCount = totalCount
            };
        }

        private static Song SongById(ForteDbContext context, Guid id) => context.Songs.Find(id);

        private static Album AlbumById(ForteDbContext context, Guid id) => context.Albums.Find(id);

        private static Artist ArtistById(ForteDbContext context, Guid id) => context.Artists.Find(id);
    }
}
