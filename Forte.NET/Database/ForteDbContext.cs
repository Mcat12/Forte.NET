using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forte.NET.Schema;
using GraphQL;
using GraphQL.Server.Transports.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Forte.NET.Database {
    public class ForteDbContext : DbContext, IUserContextBuilder {
        public DbSet<Song> Songs { get; set; } = null!;
        public DbSet<Album> Albums { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=db.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Store GUIDs as blobs
            modelBuilder
                .Entity<Song>()
                .Property(e => e.Id)
                .HasConversion<byte[]>();
            modelBuilder
                .Entity<Song>()
                .Property(e => e.AlbumId)
                .HasConversion<byte[]>();
            modelBuilder
                .Entity<Album>()
                .Property(e => e.Id)
                .HasConversion<byte[]>();
        }

        public Task<IDictionary<string, object>> BuildUserContext(HttpContext httpContext) {
            var dbContext = httpContext.RequestServices.GetService(typeof(ForteDbContext)) ??
                            throw new ArgumentNullException();

            return Task.FromResult<IDictionary<string, object>>(new Dictionary<string, object> {
                ["ForteDbContext"] = dbContext
            });
        }
    }

    public static class ForteDbContextExtension {
        /// <summary>
        /// Get our DB context from the general GraphQL context
        /// </summary>
        /// <param name="context">The GraphQL context</param>
        /// <returns>Our DB context</returns>
        public static ForteDbContext ForteDbContext(this IResolveFieldContext<object> context) =>
            (context.UserContext["ForteDbContext"] as ForteDbContext)!;
    }
}
