using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;
using ApplicationLayer.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Testing.BudgetsTests
{
    public class BudgetRepositoryTests : DbContextMock
    {
        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }
        [Test]
        public void AddBudget_Success()
        {
            var budgetEntity = new BudgetsEntity
            {
                UserID = 2,
                Amount = 1000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Frequency = "Monthly",
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CreatedBy = "kasyap123@example.invalid",
                ModifiedBy = "kasyap123@example.invalid"
            };

            var budgetRepository = new BudgetRepository(mockContext.Object);
            var result = budgetRepository.AddBudget(budgetEntity);

            Assert.IsInstanceOf<BudgetsEntity>(result);
            Assert.AreEqual(budgetEntity.UserID, result.UserID);
            Assert.AreEqual(budgetEntity.Amount, result.Amount);
            Assert.AreEqual(budgetEntity.StartDate, result.StartDate);
            Assert.AreEqual(budgetEntity.EndDate, result.EndDate);
            Assert.AreEqual(budgetEntity.Frequency, result.Frequency);
            Assert.AreEqual(budgetEntity.IsActive, result.IsActive);
            Assert.AreEqual(budgetEntity.CreatedBy, result.CreatedBy);
            Assert.AreEqual(budgetEntity.ModifiedBy, result.ModifiedBy);
            Assert.That(result.CreatedOn, Is.EqualTo(budgetEntity.CreatedOn).Within(TimeSpan.FromSeconds(1)));
            Assert.That(result.ModifiedOn, Is.EqualTo(budgetEntity.ModifiedOn).Within(TimeSpan.FromSeconds(1)));

            mockContext.Verify(a => a.Set<BudgetsEntity>().Add(It.IsAny<BudgetsEntity>()), Times.Once);
            mockContext.Verify(a => a.SaveChanges(), Times.Once);
        }
        [Test]
        public void AddBudget_Successfull()
        {
            var newBudget = new BudgetsEntity
            {
                UserID = 1,
                Amount = 5000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Frequency = "Monthly"
            };

            var result = _budgetRepository.AddBudget(newBudget);

            Assert.IsNotNull(result);
            Assert.AreEqual(newBudget.Amount, result.Amount);
        }
        [Test]
        public void AddBudget_UserNull_Successfull()
        {
            try
            {
                var newBudget = new BudgetsEntity
                {
                    UserID = 1000,
                    Amount = 5000,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    Frequency = "Monthly"
                };

                var result = _budgetRepository.AddBudget(newBudget);

            }
            catch(Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("User  not found."));
            }

            }

        [Test]
        public void AddBudget_Fail()
        {
            var newBudget = new BudgetsEntity
            {
                UserID = 1,
                Amount = 5000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Frequency = "Monthly"
            };

            mockContext.Setup(m => m.Set<UsersEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _budgetRepository.AddBudget(newBudget));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public void DeleteBudget_Success()
        {
            var result = _budgetRepository.DeleteBudget(1);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
        }

        [Test]
        public void DeleteBudget_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _budgetRepository.DeleteBudget(1));

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public void GetBudgetDetails_Success()
        {
            var result = _budgetRepository.GetBudgetDetails();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
        [Test]
        public void GetBudgetDetails_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _budgetRepository.GetBudgetDetails());

            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetBudgetsDetailsByBudgetID_Success()
        {
            var result = _budgetRepository.GetBudgetDetails(2);
            Assert.IsNotNull(result);
        }
        [Test]
        public void GetBudgetsDetailsByBudgetID_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _budgetRepository.GetBudgetDetails(2));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public void GetBudgetsByUser_Success()
        {
            var result = _budgetRepository.GetBudgetsByUser(3);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void GetBudgetsByUser_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _budgetRepository.GetBudgetsByUser(3));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void UpdateBudget_Success()
        {
            var result = _budgetRepository.UpdateBudget(3, 6000);

            Assert.IsNotNull(result);
            Assert.AreEqual(6000, result.Amount);
        }
        [Test]
        public void UpdateBudget_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _budgetRepository.UpdateBudget(3, 6000));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void CheckPreviousBudgets_Success()
        {
            var result = _budgetRepository.CheckPreviousBudgets(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), 1);
            Assert.Null(result);
        }
        [Test]
        public void CheckPreviousBudgets_Fail()
        {
            mockContext.Setup(m => m.Set<BudgetsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _budgetRepository.CheckPreviousBudgets(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), 1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

    }
}