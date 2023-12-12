using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;

namespace Testing.CategoryTests
{
    public class CategoryRepositoryTests : DbContextMock
    {
        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public void GetCategories_Success()
        {
            var result = _categoryRepository.GetCategories();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void GetCategories_Fail()
        {
            mockContext.Setup(m => m.Set<CategoriesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.Throws<Exception>(() => _categoryRepository.GetCategories());
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public async Task GetCategoryByNameAsync_Success()
        {
            var result = await _categoryRepository.GetCategoryByNameAsync("Housing");

            Assert.IsNotNull(result);
            Assert.AreEqual("Housing", result.Name);
        }
    }
}
