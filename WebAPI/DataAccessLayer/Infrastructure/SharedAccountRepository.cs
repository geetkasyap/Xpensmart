using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class SharedAccountRepository : ISharedAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public SharedAccountRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        /// <summary>
        /// adding create users entity with two user ids
        /// </summary>
        /// <param name="sharedAccountsEntity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SharedAccountsEntity CreateSharedUser(SharedAccountsEntity sharedAccountsEntity)
        {
            try
            {
                // Retrieve the login entity for the first user
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == sharedAccountsEntity.UserID1 && a.IsActive).FirstOrDefault();
                SharedAccountsEntity newSharedAccount = new SharedAccountsEntity
                {
                    // Create a new shared account entity
                    UserID1 = sharedAccountsEntity.UserID1,
                    UserID2 = sharedAccountsEntity.UserID2,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = loginEntity.Email,
                    ModifiedBy = loginEntity.Email,
                };
                _context.Set<SharedAccountsEntity>().Add(newSharedAccount);
                _context.SaveChanges();
                return newSharedAccount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Marks a shared user account as inactive based on the user ID.
        /// </summary>
        /// <param name="userID">The user ID of the shared account to delete.</param>
        /// <returns>The updated shared account entity.</returns>
        public SharedAccountsEntity DeleteSharedUser(int? userID)
        {
            try
            {
                SharedAccountsEntity? sharedAccountsEntity = _context.Set<SharedAccountsEntity>().Where(a => a.UserID1 == userID && a.IsActive).FirstOrDefault();
                sharedAccountsEntity.IsActive = false;
                _context.SaveChanges();
                return sharedAccountsEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Retrieves a list of all active shared user accounts.
        /// </summary>
        /// <returns>A list of active shared account entities.</returns>
        public List<SharedAccountsEntity> GetSharedUserDetails()
        {
            try
            {
                List<SharedAccountsEntity> sharedAccountsEntity = _context.Set<SharedAccountsEntity>().Where(a => a.IsActive==true).ToList();
                return sharedAccountsEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Retrieves shared user details for a specific user ID.
        /// </summary>
        /// <param name="userID">The user ID to retrieve shared accounts for.</param>
        /// <returns>A list of shared account entities for the specified user.</returns>
        public List<SharedAccountsEntity> GetSharedUserDetails(int userID)
        {
            try
            {
                List<SharedAccountsEntity> sharedAccountsEntity = _context.Set<SharedAccountsEntity>().Where(a => a.UserID1 == userID || a.UserID2 == userID && a.IsActive).Select(A => new SharedAccountsEntity
                {
                    UserID2 = A.UserID1 == userID ? A.UserID2 : A.UserID1
                }).ToList();
                return sharedAccountsEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the shared user account with a new user ID.
        /// </summary>
        /// <param name="userID1">The user ID of the account to update.</param>
        /// <param name="userID2">The new user ID to associate with the shared account.</param>
        /// <returns>The updated shared account entity.</returns>
        public SharedAccountsEntity UpdateSharedUser(int userID1, int userID2)
        {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == userID1 && a.IsActive).FirstOrDefault();
                SharedAccountsEntity? sharedAccountsEntity = _context.Set<SharedAccountsEntity>().Where(a => a.UserID1 == userID1 || a.UserID2== userID1 && a.IsActive).FirstOrDefault();
                sharedAccountsEntity.UserID2 = userID2;
                sharedAccountsEntity.ModifiedBy = loginEntity.Email;
                sharedAccountsEntity.ModifiedOn = DateTime.Now;
                _context.SaveChanges();
                return sharedAccountsEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Retrieves shared user details along with usernames for a specific user ID.
        /// </summary>
        /// <param name="userID">The user ID to retrieve shared accounts for.</param>
        /// <returns>A list of shared user details with usernames.</returns>
        public List<SharedUserWithUserName> GetSharedUserDetailsWithNames(int userID)
        {
            try
            {
                List<SharedAccountsEntity> sharedAccountsEntity = _context.Set<SharedAccountsEntity>()
                    .Where(a => (a.UserID1 == userID || a.UserID2 == userID) && a.IsActive)
                    .Select(A => new SharedAccountsEntity
                    {
                        UserID2 = A.UserID1 == userID ? A.UserID2 : A.UserID1
                    }).ToList();

                List<SharedUserWithUserName> result = new List<SharedUserWithUserName>();
                // Loop through each shared account to get usernames

                foreach (var account in sharedAccountsEntity)
                {
                    UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == account.UserID2 && a.IsActive).FirstOrDefault();
                    string userName = loginEntity.UserName;
                    result.Add(new SharedUserWithUserName
                    {
                        UserID = account.UserID2,
                        UserName = userName
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
