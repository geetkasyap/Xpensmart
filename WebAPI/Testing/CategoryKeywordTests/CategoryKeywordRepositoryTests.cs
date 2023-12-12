using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;

namespace Testing.CategoryKeywordTests
{
    public class CategoryKeywordRepositoryTests : DbContextMock
    {
        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public void GetCategoryKeywords_Success()
        {
            var result = _categoryKeywordsRepository.GetCategoryKeywords();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }
    }
}
