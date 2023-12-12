using CoreModels.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;

namespace Testing.AggregateBudgetTests
{
    public class AggregateBudgetRespositoryTests : DbContextMock
    {
        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public async Task GetCategoryBudgetAsync_Success()
        {
            var result = await _aggregateBudgetRepository.GetCategoryBudgetAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CategoryId);
        }

        [Test]
        public async Task GetAllCategoryBudgetsAsync_Success()
        {
            var result = await _aggregateBudgetRepository.GetAllCategoryBudgetsAsync();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public async Task GetCategoryBudgetAsync_Fail()
        {
            mockContext.Setup(m => m.Set<AggregateBudgetEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _aggregateBudgetRepository.GetCategoryBudgetAsync(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }
        [Test]
        public async Task GetAllCategoryBudgetAsync_Fail()
        {
            mockContext.Setup(m => m.Set<AggregateBudgetEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => await _aggregateBudgetRepository.GetAllCategoryBudgetsAsync());
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }
    }
}
