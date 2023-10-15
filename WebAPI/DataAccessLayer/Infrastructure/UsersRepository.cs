using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{

    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;
        public UsersRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        private static string GetEncryptionKey()
        {
            var key = Environment.GetEnvironmentVariable("XPENSMART_ENCRYPTION_KEY");
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("Set XPENSMART_ENCRYPTION_KEY before using account operations.");
            }
            return key;
        }
        /// <summary>
        /// create user
        /// </summary>
        /// <param name="usersEntities"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UsersEntity CreateUser(UsersEntity usersEntities)
        {
            string secretKey = GetEncryptionKey();
            string HashedKey = AESEncryptData(usersEntities.Password, secretKey);
            try
            {
                UsersEntity newUser = new UsersEntity
                {
                    UserName = usersEntities.UserName,
                    Email = usersEntities.Email,
                    Password = HashedKey,
                    FullName = usersEntities.FullName,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now
                };
                _context.Set<UsersEntity>().Add(newUser);
                _context.SaveChanges();
                return newUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// list of user entities
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<UsersEntity> GetUserDetails()
        {
            try
            {
                List<UsersEntity> loginEntities = _context.Set<UsersEntity>().Where(a => a.IsActive).ToList();
                return loginEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// user entity by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UsersEntity GetUserDetails(string userName)
        {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserName == userName && a.IsActive).FirstOrDefault();
                return loginEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// update user details
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="NewPassword"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UsersEntity UpdateUser(string? userName, string password, string NewPassword)
        {
            try
            {
                UsersEntity? loginEntity = _context.Set<UsersEntity>().Where(a => a.UserName == userName && a.IsActive).FirstOrDefault();
                string secretKey = GetEncryptionKey();
                string UnHashedKey = AESDecryptData(loginEntity.Password, secretKey);
                if (password == UnHashedKey)
                {
                    string HashedKey = AESEncryptData(NewPassword, secretKey);
                    loginEntity.Password = HashedKey;
                    _context.SaveChanges();
                    return loginEntity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// soft delete user entity
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UsersEntity DeleteUser(string? userName)
        {
            try
            {
                UsersEntity? loginEntity = _context.Set<UsersEntity>().Where(a => a.UserName == userName && a.IsActive==true).FirstOrDefault();
                loginEntity.IsActive = false;
                _context.SaveChanges();
                return loginEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// check user name if that was unique or not
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UsersEntity CheckUserName(string? userName)
        {
            try
            {
                UsersEntity? loginEntity = _context.Set<UsersEntity>().Where(a => a.UserName == userName && a.IsActive).FirstOrDefault();
                return loginEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// check email if it was unique or not
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UsersEntity CheckEmail(string? Email)
        {
            try
            {
                UsersEntity? loginEntity = _context.Set<UsersEntity>().Where(a => a.Email == Email && a.IsActive).FirstOrDefault();
                return loginEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// get user entity by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<UsersEntity> GetUserByUserNameAsync(string userName)
        {
            return await _context.Set<UsersEntity>().FirstOrDefaultAsync(u => u.UserName == userName && u.IsActive);
        }
        /// <summary>
        /// login check through aes
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UsersEntity LoginCheck(string? email, string? password)
        {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.Email == email && a.IsActive).FirstOrDefault();
                if(loginEntity == null)
                {
                    return new UsersEntity();
                }
                else
                {
                    string secretKey = GetEncryptionKey();
                    string UnHashedKey = AESDecryptData(loginEntity.Password, secretKey);
                    if (UnHashedKey == password)
                    {
                        return loginEntity;
                    }
                    else
                    {
                        return new UsersEntity();
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// return username by user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetUserNamesByUserID(int userID)
        {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == userID && a.IsActive).FirstOrDefault();

                return loginEntity.UserName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// get emailID by user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetEmailByUserID(int userID)
        {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == userID && a.IsActive).FirstOrDefault();

                return loginEntity.Email;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// get a list of all user names
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string[] GetUserAllNames()
        {
            try
            {
                string[] loginEntities = _context.Set<UsersEntity>().Where(a => a.IsActive).Select(a => a.UserName).ToArray();
                return loginEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// encryption with secret key
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Encryption(string password)
        {
            string secretKey = GetEncryptionKey();
            return AESEncryptData(password, secretKey);

        }
        /// <summary>
        /// AES encryption meathod
        /// </summary>
        /// <param name="textData"></param>
        /// <param name="Encryptionkey"></param>
        /// <returns></returns>
        public static string AESEncryptData(string textData, string Encryptionkey)
        {
            RijndaelManaged objrij = new RijndaelManaged();
            objrij.Mode = CipherMode.CBC;
            objrij.Padding = PaddingMode.PKCS7;
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            byte[] EncryptionkeyBytes = new byte[0x10];
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            ICryptoTransform objtransform = objrij.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(textData);
            return Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
        }
        /// <summary>
        /// Decryption meathod with secret key
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public string Decryption(string hashedPassword)
        {

            string secretKey = GetEncryptionKey();
            return AESDecryptData(hashedPassword, secretKey);
        }
        /// <summary>
        /// decryption meathod
        /// </summary>
        /// <param name="EncryptedText"></param>
        /// <param name="Decryptionkey"></param>
        /// <returns></returns>
        public static string AESDecryptData(string EncryptedText, string Decryptionkey)
        {
                RijndaelManaged objrij = new RijndaelManaged();
                objrij.Mode = CipherMode.CBC;
                objrij.Padding = PaddingMode.PKCS7;
                byte[] encryptedTextByte = Convert.FromBase64String(EncryptedText);
                byte[] passBytes = Encoding.UTF8.GetBytes(Decryptionkey);
                byte[] EncryptionkeyBytes = new byte[0x10];
                int len = passBytes.Length;
                if (len > EncryptionkeyBytes.Length)
                {
                    len = EncryptionkeyBytes.Length;
                }
                Array.Copy(passBytes, EncryptionkeyBytes, len);
                objrij.Key = EncryptionkeyBytes;
                objrij.IV = EncryptionkeyBytes;
                byte[] TextByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);
                return Encoding.UTF8.GetString(TextByte);
            

        }

        public string ForgotPassword(string email)
        {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.Email == email && a.IsActive).FirstOrDefault();
                if (loginEntity == null)
                {
                    return null;
                }
                else
                {
                    string secretKey = GetEncryptionKey();
                    string UnHashedKey = AESDecryptData(loginEntity.Password, secretKey);
                    return UnHashedKey;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
