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

namespace Testing.CategoryKeywordTests
{
    public class CategoryKeywordsControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<CategoryKeywordsController>> _mockLogger;
        private CategoryKeywordsController _categoryKeywordsController;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<CategoryKeywordsController>>();
            _categoryKeywordsController = new CategoryKeywordsController(_mockUnitOfWork.Object, null, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _categoryKeywordsController.Dispose();
        }


        [Test]
        public void GetCategoryKeywords_ReturnsKeywords_WhenDataExists()
        {
            var categoryKeywords = new List<CategoryKeywordsEntity>
            {
                new CategoryKeywordsEntity { CategoryKeywordID = 1, CategoryID = 1, Keyword = "Paints" },
                new CategoryKeywordsEntity { CategoryKeywordID = 2, CategoryID = 1, Keyword = "Brushes" }
            };
            _mockUnitOfWork.Setup(u => u.CategoryKeywords.GetCategoryKeywords()).Returns(categoryKeywords);

            var result = _categoryKeywordsController.GetCategoryKeywords() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(categoryKeywords, result.Value);
        }

        [Test]
        public void GetCategoryKeywords_ReturnsEmptyList_WhenNoKeywordsExist()
        {

            _mockUnitOfWork.Setup(u => u.CategoryKeywords.GetCategoryKeywords()).Returns(new List<CategoryKeywordsEntity>());

 
            var result = _categoryKeywordsController.GetCategoryKeywords() as OkObjectResult;

     
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsEmpty(result.Value as List<CategoryKeywordsEntity>);
        }

        [Test]
        public void GetCategoryKeywords_ReturnsServerError_OnException()
        {

            _mockUnitOfWork.Setup(u => u.CategoryKeywords.GetCategoryKeywords()).Throws(new Exception("Test exception"));

    
            var result = _categoryKeywordsController.GetCategoryKeywords() as ObjectResult;

       
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.AreEqual("Test exception", result.Value.GetType().GetProperty("Error").GetValue(result.Value));
        }
    }
}
