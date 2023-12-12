using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;

namespace Testing.BudgetSuggestionsTests
{
    public class BudgetSuggestionsRepositoryTests : DbContextMock
    {
        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public async Task GetExpensesByUserIdAsync_Success()
        {
            var result = await _budgetSuggestionsRepository.GetExpensesByUserIdAsync(1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public async Task GetBudgetsByUserIdAsync_Success()
        {
            var result = await _budgetSuggestionsRepository.GetBudgetsByUserIdAsync(3);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetCategoryByIdAsync_Success()
        {
            var result = await _budgetSuggestionsRepository.GetCategoryByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CategoryID);
        }

        [Test]
        public void CheckPresentBudgets_Success()
        {
            var result = _budgetSuggestionsRepository.CheckPresentBudgets(1);

            Assert.IsNotNull(result);
        }

        [Test]
        public void CheckPresentBudgets_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.Throws<Exception>(() => _budgetSuggestionsRepository.CheckPresentBudgets(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


    }
}
