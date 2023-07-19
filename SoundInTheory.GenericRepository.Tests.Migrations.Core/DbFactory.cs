using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.GenericRepository.Tests.Migrations.Core
{
    [ExcludeFromCodeCoverage]
    internal class DbFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TestDbContext>();
            builder.UseSqlite("Filename=./tests.db", b => b.MigrationsAssembly("SoundInTheory.GenericRepository.Tests.Migrations.Core"));
            return new TestDbContext(builder.Options);
        }
    }
}
