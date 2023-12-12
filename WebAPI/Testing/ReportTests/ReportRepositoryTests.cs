using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;

namespace Testing.ReportTests
{
    public class ReportRepositoryTests : DbContextMock
    {

        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public void GenerateTotalExpenseReport_Success()
        {
            var result = _reportRepository.GenerateTotalExpenseReport(1, DateTime.Now.AddDays(-30), DateTime.Now, 1);

            Assert.IsInstanceOf<decimal>(result);
        }
        [Test]
        public void GenerateTotalExpenseReport_False()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception());
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GenerateTotalExpenseReport(1, DateTime.Now.AddDays(-30), DateTime.Now, 1));
            Assert.That(ex.Message, Is.EqualTo("Error generating report: Exception of type 'System.Exception' was thrown."));
        }

        [Test]
        public void GenerateTotalExpenseReport_Credited_Success()
        {
            var result = _reportRepository.GenerateTotalExpenseReport(7, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30), 1);

            Assert.IsInstanceOf<decimal>(result);
        }

        [Test]
        public void GenerateTotalExpenseReport_CategoryIdNull_Success()
        {
            var result = _reportRepository.GenerateTotalExpenseReport(7, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30), 0);

            Assert.IsInstanceOf<decimal>(result);
        }

        [Test]
        public void GenerateTotalExpenseReport_CategoryNameNull_Success()
        {
            try
            {
                var result = _reportRepository.GenerateTotalExpenseReport(7, DateTime.Now.AddDays(-30), DateTime.Now, 100);

            }
            catch(Exception ex)
            {
                Assert.AreEqual(ex.Message, "Error generating report: Category not found or inactive.");

            }
        }

        [Test]
        public void GenerateAverageExpenseReport_Success()
        {
            var result = _reportRepository.GenerateAverageExpenseReport(1, DateTime.Now.AddDays(-30), DateTime.Now, 1);

            Assert.IsInstanceOf<decimal>(result);
        }
        [Test]
        public void GenerateAverageExpenseReport_False()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception());
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GenerateAverageExpenseReport(1, DateTime.Now.AddDays(-30), DateTime.Now, 1));
            Assert.That(ex.Message, Is.EqualTo("Error generating report: Exception of type 'System.Exception' was thrown."));
        }

        [Test]
        public void GenerateAverageExpenseReport_Credited_Success()
        {
            var result = _reportRepository.GenerateAverageExpenseReport(7, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30), 1);

            Assert.IsInstanceOf<decimal>(result);
        }
        [Test]
        public void GenerateAverageExpenseReport_CategoryNameNull_Success()
        {
            try
            {
                var result = _reportRepository.GenerateAverageExpenseReport(7, DateTime.Now.AddDays(-30), DateTime.Now, 100);

            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Error generating report: Category not found or inactive.");

            }
        }

        [Test]
        public void GetTopThreeRecentExpenses_Success()
        {
            var result = _reportRepository.GetTopThreeRecentExpenses(1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
        [Test]
        public void GetTopThreeRecentExpenses_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception());
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetTopThreeRecentExpenses(1));
            Assert.That(ex.Message, Is.EqualTo("Error fetching recent expenses: Exception of type 'System.Exception' was thrown."));
        }

        [Test]
        public void GetExpensesByDates_Success()
        {
            var result = _reportRepository.GetExpensesByDates(1, DateTime.Now.AddDays(-30), DateTime.Now);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
        [Test]
        public void GetExpensesByDates_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetExpensesByDates(1, DateTime.Now.AddDays(-30), DateTime.Now));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetBudgetByDates_Success()
        {
            var result = _reportRepository.GetBudgetByDates(1, DateTime.Now.AddDays(-30), DateTime.Now);

            Assert.IsInstanceOf<decimal>(result);
        }
        [Test]
        public void GetBudgetByDates_NotNull_Success()
        {
            var result = _reportRepository.GetBudgetByDates(1, DateTime.Now.AddDays(-60), DateTime.Now.AddDays(60));

            Assert.IsInstanceOf<decimal>(result);
        }
        [Test]
        public void GetBudgetByDates_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetBudgetByDates(1, DateTime.Now.AddDays(-60), DateTime.Now.AddDays(60)));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }
        [Test]
        public void GetCategoryExpensesByDates_Success()
        {
            var result = _reportRepository.GetCategoryExpensesByDates(1, DateTime.Now.AddDays(-30), DateTime.Now, 1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
        [Test]
        public void GetCategoryExpensesByDates_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetCategoryExpensesByDates(1, DateTime.Now.AddDays(-30), DateTime.Now, 1));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetMonthlyBudgetsByDateRange_Success()
        {
            var result = _reportRepository.GetMonthlyBudgetsByDateRange(1, DateTime.Now.AddDays(-30), DateTime.Now);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
        [Test]
        public void GetMonthlyBudgetsByDateRange_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception());
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetMonthlyBudgetsByDateRange(1, DateTime.Now.AddDays(-30), DateTime.Now));
            Assert.That(ex.Message, Is.EqualTo("Error fetching monthly budgets: Exception of type 'System.Exception' was thrown."));
        }
        [Test]
        public void DummyTestForGetRemainingExpenses()
        {
            var result = _reportRepository.GetRemainingExpenses(1);
        }
        [Test]
        public void GetRemainingExpenses_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception());
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetRemainingExpenses(1));
            Assert.That(ex.Message, Is.EqualTo("Error fetching remaining expenses: Exception of type 'System.Exception' was thrown."));
        }

        [Test]
        public void DummyTestForGetUpcomingExpenses()
        {
            var result = _reportRepository.GetUpcomingExpenses(1);
        }
        [Test]
        public void GetUpcomingExpenses_Fail()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception());
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetUpcomingExpenses(1));
            Assert.That(ex.Message, Is.EqualTo("Error fetching upcoming expenses: Exception of type 'System.Exception' was thrown."));
        }
        [Test]
        public void DummyTestForGetCategorySummary()
        {
            var result = _reportRepository.GetCategorySummary(1,DateTime.Now, DateTime.Now);
        }
        [Test]
        public void GetCategorySummary_False()
        {
            mockContext.Setup(m => m.Set<ExpensesEntity>()).Throws(new Exception());
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetCategorySummary(1, DateTime.Now, DateTime.Now));
            Assert.That(ex.Message, Is.EqualTo("Exception of type 'System.Exception' was thrown."));
        }

        public void DummyTestForGetCategoryBudgets()
        {
            var result = _reportRepository.GetCategoryBudgets(1, DateTime.Now, DateTime.Now, 1);
        }

        [Test]
        public void GetCategoryBudgets_Success()
        {
            var userId = 1;
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now.AddMonths(1);
            var categoryId = 1;

            var result = _reportRepository.GetCategoryBudgets(userId, startDate, endDate, categoryId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result.All(b => b.CategoryId == categoryId));
        }
        [Test]
        public void GetCategoryBudgets_Fail()
        {
            var userId = 1;
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now.AddMonths(1);
            var categoryId = 1;

            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception());
            var ex = Assert.ThrowsAsync<Exception>(async () => _reportRepository.GetCategoryBudgets(userId, startDate, endDate, categoryId));
            Assert.That(ex.Message, Is.EqualTo("Error retrieving category budgets: Exception of type 'System.Exception' was thrown."));
        }

        [Test]
        public void GetCategoryBudgets_NoExpensesForUser_ReturnsZeroAmounts()
        {
            var userId = 1;
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;
            var categoryId = 1; 
            var result = _reportRepository.GetCategoryBudgets(userId, startDate, endDate, categoryId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.All(b => b.Amount == 0));
        }
    }
}