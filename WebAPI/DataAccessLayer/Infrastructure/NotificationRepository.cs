using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class NotificationRepository:INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        // Method to add a new notification
        public NotificationsEntity AddNotifications(NotificationsEntity notificationsEntities)
        {
            try
            {
                // Create a new notification entity with the provided details
                NotificationsEntity newNotifications = new NotificationsEntity
                {
                    UserID = notificationsEntities.UserID,
                    Type = notificationsEntities.Type,
                    Message = notificationsEntities.Message,
                    IsActive = true,
                    ReadMessage = false,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
                _context.Set<NotificationsEntity>().Add(newNotifications);
                _context.SaveChanges();
                return newNotifications;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to delete a notification by its ID
        public NotificationsEntity DeleteNotification(int notificationID)
        {
            try
            {
                NotificationsEntity? notificationsEntity = _context.Set<NotificationsEntity>().Where(a => a.NotificationID == notificationID && a.IsActive).FirstOrDefault();
                if (notificationsEntity == null)
                {
                    throw new Exception("Notification not found or already deleted.");
                }
                notificationsEntity.IsActive = false;
                notificationsEntity.ModifiedOn = DateTime.Now;
                _context.SaveChanges();
                return notificationsEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get all active notifications for a specific user
        public List<NotificationsEntity> GetnotificationDetails(int userID)
        {
            try
            {
                List<NotificationsEntity> notificationsEntities = _context.Set<NotificationsEntity>().Where(a => a.IsActive && a.UserID==userID).ToList();
                return notificationsEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get a specific notification by its ID and user ID
        public NotificationsEntity GetnotificationDetails(int notificationID, int userID)
        {
            try
            {
                NotificationsEntity notificationsEntities = _context.Set<NotificationsEntity>().Where(a => a.NotificationID == notificationID && a.UserID==userID).FirstOrDefault();
                return notificationsEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to mark a notification as read
        public NotificationsEntity ReadNotification(int notificationID)
        {
            try
            {
                // Find the notification by ID and check if it is active
                NotificationsEntity? notificationsEntity = _context.Set<NotificationsEntity>().Where(a => a.NotificationID == notificationID && a.IsActive == true).FirstOrDefault();
                if (notificationsEntity == null)
                {
                    throw new Exception("Notification not found or already deleted.");
                }
                notificationsEntity.ReadMessage = true;
                notificationsEntity.ModifiedOn = DateTime.Now;
                _context.SaveChanges();
                return notificationsEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
