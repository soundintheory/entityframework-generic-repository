
using Microsoft.EntityFrameworkCore;
using SoundInTheory.GenericRepository.Tests.Migrations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.GenericRepository.Tests.Migrations
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() { }

        public DbSet<Fruit> Fruits { get; set; }

        /// <summary>
        /// Gets/sets whether the db context as been initialized. This
        /// is only performed once in the application lifecycle.
        /// </summary>
        private static volatile bool IsInitialized = false;

        /// <summary>
        /// The object mutex used for initializing the context.
        /// </summary>
        private static readonly object Mutex = new object();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
            if (!IsInitialized)
            {
                lock (Mutex)
                {
                    if (!IsInitialized)
                    {
                        // Migrate database
                        Database.Migrate();

                        IsInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Creates and configures the data model.
        /// </summary>
        /// <param name="mb">The current model builder</param>
        protected override void OnModelCreating(ModelBuilder mb)
        {

        }
    }
}
