﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using PearAdmin.AbpTemplate.Admin.Models.Common;
using PearAdmin.AbpTemplate.Admin.Models.Notifications;
using PearAdmin.AbpTemplate.Notifications;
using PearAdmin.AbpTemplate.Notifications.Dto;

namespace PearAdmin.AbpTemplate.Admin.Controllers
{
    /// <summary>
    /// 消息管理控制器
    /// </summary>
    [AbpMvcAuthorize]
    public class NotificationsController : AbpTemplateControllerBase
    {
        private readonly INotificationAppService _notificationAppService;

        public NotificationsController(INotificationAppService notificationAppService)
        {
            _notificationAppService = notificationAppService;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取未读通知数量
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetUnreadNotificationCount()
        {
            var unreadNotificationCount = await _notificationAppService.UnreadMessageCount();
            return Json(new ResponseParamSingleViewModel<int>(unreadNotificationCount));
        }

        /// <summary>
        /// 根据分页条件获取消息列表
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetUserNotificationList(GetUserNotificationPagedViewModel viewModel)
        {
            var input = PagedViewModelMapToPagedInputDto<GetUserNotificationPagedViewModel, GetUserNotificationsPagedInput>(viewModel);
            var pagedUserNotificationDto = await _notificationAppService.GetPagedUserNotifications(input);

            return Json(new GetNotificationsResultViewModel(pagedUserNotificationDto.TotalCount, pagedUserNotificationDto.UnreadCount, pagedUserNotificationDto.Items));
        }

        /// <summary>
        /// 设置消息记录全部已读
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SetAllNotificationsAsRead()
        {
            await _notificationAppService.SetAllNotificationsAsRead();

            return Json(new ResponseParamViewModel(L("AllNotificationSetAsReadSuccessful")));
        }

        /// <summary>
        /// 设置指定消息记录已读
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SetNotificationsAsRead([FromBody]List<EntityDto<Guid>> input)
        {
            await _notificationAppService.SetNotificationAsRead(input);

            return Json(new ResponseParamViewModel(L("NotificationSetAsReadSuccessful")));
        }

        /// <summary>
        /// 删除消息记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> DeleteNotification([FromBody]List<EntityDto<Guid>> input)
        {
            await _notificationAppService.DeleteNotification(input);

            return Json(new ResponseParamViewModel(L("DeleteNotificationSuccessful")));
        }
    }
}
