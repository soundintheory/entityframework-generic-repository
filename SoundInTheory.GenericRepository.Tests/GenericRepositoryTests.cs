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

        [Fact]
        public void SqlRaw()
        {
            var fruit = _fruits.Create(new Fruit() { Name = "SelectableFruit", Colour = "Multicoloured" });
            var fruitList = _fruits.Query<List<Fruit>>("SELECT * FROM Fruits WHERE Id = {0}", fruit.Id);
            Assert.True(fruitList.Any());
            Assert.True(fruitList.First().Id == fruit.Id);
        }
    }
}
