﻿using Abp.Application.Services.Dto;
using Abp.Notifications;
using System.Collections.Generic;

namespace PearAdmin.AbpTemplate.Notifications.Dto
{
    public class GetNotificationsOutput : PagedResultDto<UserNotification>
    {
        public int UnreadCount { get; set; }

        public GetNotificationsOutput(int totalCount, int unreadCount, List<UserNotification> notifications)
            : base(totalCount, notifications)
        {
            UnreadCount = unreadCount;
        }
    }
}
