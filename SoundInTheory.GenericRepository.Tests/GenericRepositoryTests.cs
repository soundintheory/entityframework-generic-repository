using SoundInTheory.GenericRepository.Tests.Migrations;
using SoundInTheory.GenericRepository.Tests.Migrations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoundInTheory.GenericRepository.Tests
{
    public class GenericRepositoryTests
    {
        private readonly IGenericRepository<TestDbContext, Fruit> _fruits;

        public GenericRepositoryTests(IGenericRepository<TestDbContext, Fruit> fruits)
        {
            _fruits = fruits;
        }


        [Fact]
        public void Create()
        {
            var fruit = _fruits.Create(new Fruit() { Name = "Apple", Colour = "Red" });
            Assert.True(fruit.Id != 0);
        }
    }
}
