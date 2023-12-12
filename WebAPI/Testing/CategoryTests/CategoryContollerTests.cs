using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpensmart.Controllers;

namespace Testing.CategoryTests
{

    public class CategoryContollerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<CategoryController>> _mockLogger;
        private CategoryController _categoryController;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<CategoryController>>();
            _categoryController = new CategoryController(_mockUnitOfWork.Object, null, _mockLogger.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _categoryController.Dispose();
        }

        [Test]
        public async Task GetCategories_ReturnsCategories_WhenDataExists()
        {
            var categories = new List<CategoriesEntity>
            {
                new CategoriesEntity { CategoryID = 1, Name = "Housing", IsActive = true },
                new CategoriesEntity { CategoryID = 2, Name = "Food", IsActive = true }
            };
            _mockUnitOfWork.Setup(u => u.Category.GetCategories()).Returns(categories);

            var result = await _categoryController.GetCategories() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(categories, result.Value);
        }

        [Test]
        public async Task GetCategories_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            _mockUnitOfWork.Setup(u => u.Category.GetCategories()).Returns(new List<CategoriesEntity>());

            var result = await _categoryController.GetCategories() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsEmpty(result.Value as List<CategoriesEntity>);
        }

        [Test]
        public async Task GetCategories_ReturnsServerError_OnException()
        {
 
            _mockUnitOfWork.Setup(u => u.Category.GetCategories()).Throws(new Exception("Test exception"));


            var result = await _categoryController.GetCategories() as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.AreEqual("Test exception", result.Value.GetType().GetProperty("Error").GetValue(result.Value));
        }
    }
}