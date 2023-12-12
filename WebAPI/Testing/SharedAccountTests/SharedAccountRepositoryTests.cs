using CoreModels.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Utils;

namespace Testing.SharedAccountTests
{
    public class SharedAccountRepositoryTests : DbContextMock
    {
        [SetUp]
        public void Setup()
        {
            ReinitializeTest();
        }

        [Test]
        public void CreateSharedUser_Success()
        {
            var sharedAccount = new SharedAccountsEntity
            {
                UserID1 = 1,
                UserID2 = 2
            };

            var result = _sharedAccountRepository.CreateSharedUser(sharedAccount);

            Assert.IsNotNull(result);
            Assert.AreEqual(sharedAccount.UserID1, result.UserID1);
        }
        [Test]
        public void CreateSharedUser_Fail ()
        {
            var sharedAccount = new SharedAccountsEntity
            {
                UserID1 = 1,
                UserID2 = 2
            };

            mockContext.Setup(m => m.Set<SharedAccountsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _sharedAccountRepository.CreateSharedUser(sharedAccount));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void DeleteSharedUser_Success()
        {
            var result = _sharedAccountRepository.DeleteSharedUser(4);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
        }


        [Test]
        public void DeleteSharedUser_Fail()
        {
            var result = _sharedAccountRepository.DeleteSharedUser(1);
            
            mockContext.Setup(m => m.Set<SharedAccountsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _sharedAccountRepository.DeleteSharedUser(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));

        }


        [Test]
        public void GetSharedUserDetails_Success()
        {
            var result = _sharedAccountRepository.GetSharedUserDetails();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void GetSharedUserDetails_Fail()
        {
            mockContext.Setup(m => m.Set<SharedAccountsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _sharedAccountRepository.GetSharedUserDetails());
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void GetSharedUserDetails_ByUserId_Success()
        {
            var result = _sharedAccountRepository.GetSharedUserDetails(1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void GetSharedUserDetails_ByUserId_Fail()
        {
            mockContext.Setup(m => m.Set<SharedAccountsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _sharedAccountRepository.GetSharedUserDetails(1));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        [Test]
        public void UpdateSharedUser_Success()
        {
            var result = _sharedAccountRepository.UpdateSharedUser(1, 3);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.UserID2);
        }

        [Test]
        public void UpdateSharedUser_Fail()
        {
            mockContext.Setup(m => m.Set<SharedAccountsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _sharedAccountRepository.UpdateSharedUser(1, 3));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }


        [Test]
        public void GetSharedUserDetailsWithNames_Success()
        {
            var result = _sharedAccountRepository.GetSharedUserDetailsWithNames(4);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
        [Test]
        public void GetSharedUserDetailsWithNames_Fail()
        {
            mockContext.Setup(m => m.Set<SharedAccountsEntity>()).Throws(new Exception("Database error"));
            var ex = Assert.ThrowsAsync<Exception>(async () => _sharedAccountRepository.GetSharedUserDetailsWithNames(4));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }
    }
}
