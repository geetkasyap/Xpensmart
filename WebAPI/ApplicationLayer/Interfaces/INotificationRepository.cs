using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface INotificationRepository
    {
        NotificationsEntity AddNotifications(NotificationsEntity notificationsEntities);
        NotificationsEntity DeleteNotification(int notificationID);
        List<NotificationsEntity> GetnotificationDetails(int userID);
        NotificationsEntity GetnotificationDetails(int notificationID, int userID);
        NotificationsEntity ReadNotification(int notificationID);
    }
}
