using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoundInTheory.GenericRepository.Tests.Migrations;
using SoundInTheory.GenericRepository.Tests.Migrations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.GenericRepository.Tests
{
    public class Startup
    {
#if NET472_OR_GREATER
        public void ConfigureHost(IHostBuilder hostBuilder) =>
            hostBuilder.ConfigureServices(services =>
            {
                services.AddDbContext<TestDbContext>(builder => builder.UseSqlite("Filename=./tests.db", b => b.MigrationsAssembly("SoundInTheory.GenericRepository.Tests.Migrations.Framework")));
                services.AddScoped<IGenericRepository<TestDbContext, Fruit>, GenericRepository<TestDbContext, Fruit>>();
            });
#endif

#if NET6_0

        public void ConfigureHost(IHostBuilder hostBuilder) =>
           hostBuilder.ConfigureWebHost(webHostBuilder => webHostBuilder
               .UseTestServer(options => options.PreserveExecutionContext = true)
               .UseStartup<TestStartup>());

        private class TestStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDbContext<TestDbContext>(builder =>
                {
                    builder.UseSqlite("Filename=./tests.db", b => b.MigrationsAssembly("SoundInTheory.GenericRepository.Tests.Migrations.Core"));
                });

                services.AddScoped<IGenericRepository<TestDbContext, Fruit>, GenericRepository<TestDbContext, Fruit>>();
            }

            public void Configure(IApplicationBuilder app)
            {

            }

        }

#endif
    }
}
