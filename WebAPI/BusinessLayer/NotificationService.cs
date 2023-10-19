using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class NotificationService
    {
        private readonly IUnitOfWork unitOfWork;
        public NotificationService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public NotificationsEntity AddNotifications(NotificationsEntity notificationsEntities)
        {
            return unitOfWork.Notification.AddNotifications(notificationsEntities);
        }

        public NotificationsEntity DeleteNotification(int notificationID)
        {
            return unitOfWork.Notification.DeleteNotification(notificationID);
        }

        public List<NotificationsEntity> GetnotificationDetails(int userID)
        {
            return unitOfWork.Notification.GetnotificationDetails(userID);
        }

        public NotificationsEntity GetnotificationDetails(int notificationID, int userID)
        {
            return unitOfWork.Notification.GetnotificationDetails(notificationID,userID);
        }

        public NotificationsEntity ReadNotification(int notificationID)
        {
            return unitOfWork.Notification.ReadNotification(notificationID);
        }
    }
}
