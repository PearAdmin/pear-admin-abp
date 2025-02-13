﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using PearAdmin.AbpTemplate.Admin.Helpers;
using PearAdmin.AbpTemplate.Admin.Models.Common;
using PearAdmin.AbpTemplate.Admin.Models.Users;
using PearAdmin.AbpTemplate.Authorization.Users;
using PearAdmin.AbpTemplate.Authorization.Users.Dto;
using PearAdmin.AbpTemplate.Authorization.Users.Profile;
using PearAdmin.AbpTemplate.Authorization.Users.Profile.Dto;
using PearAdmin.AbpTemplate.CommonDto;
using PearAdmin.AbpTemplate.Net.MimeTypes;
using PearAdmin.AbpTemplate.Notifications;
using PearAdmin.AbpTemplate.Notifications.Dto;
using PearAdmin.AbpTemplate.TempFileCaches;

namespace PearAdmin.AbpTemplate.Admin.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    [AbpMvcAuthorize]
    public class UsersController : AbpTemplateControllerBase
    {
        #region 初始化
        private const int MaxProfilePictureSize = 5242880;//5MB

        private readonly IUserAppService _userAppService;
        private readonly IProfileAppService _profileAppService;
        private readonly INotificationAppService _notificationAppService;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        public UsersController(
            IUserAppService userAppService,
            IProfileAppService profileAppService,
            INotificationAppService notificationAppService,
            ITempFileCacheManager tempFileCacheManager)
        {
            _userAppService = userAppService;
            _profileAppService = profileAppService;
            _notificationAppService = notificationAppService;
            _tempFileCacheManager = tempFileCacheManager;
        }
        #endregion

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 根据分页条件获取用户列表
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetUserList(GetPagedUserViewModel viewModel)
        {
            var input = PagedViewModelMapToPagedInputDto<GetPagedUserViewModel, GetPagedUserInput>(viewModel);
            var pagedUserList = await _userAppService.GetPagedUser(input);

            return Json(new ResponseParamPagedViewModel<UserDto>(pagedUserList.TotalCount, pagedUserList.Items));
        }

        /// <summary>
        /// 创建或更新用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> CreateOrUpdateUser(NullableIdDto<long> input)
        {
            var userForEditOutput = await _userAppService.GetUserForEdit(input);

            return View(userForEditOutput);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="createOrUpdateViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateUser([FromBody]CreateUserDto input)
        {
            await _userAppService.CreateUser(input);

            return Json(new ResponseParamViewModel(L("CreateUserSuccessful")));
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="createOrUpdateViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpdateUser([FromBody]UpdateUserDto input)
        {
            await _userAppService.UpdateUser(input);

            return Json(new ResponseParamViewModel(L("UpdateUserSuccessful")));
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> DeleteUser([FromBody]List<EntityDto<long>> input)
        {
            await _userAppService.DeleteUser(input);

            return Json(new ResponseParamViewModel(L("DeleteUserSuccessful")));
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ResetUserPassword([FromBody]ResetPasswordInput input)
        {
            var success = await _userAppService.ResetPassword(input);
            var msg = success ? L("DeleteUserSuccessful") : L("DeleteUserFailed");
            return Json(new ResponseParamViewModel(msg));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ChangePassword([FromBody]ChangePasswordDto input)
        {
            await _profileAppService.ChangePassword(input);
            return Json(new ResponseParamViewModel(L("ChangePasswordSuccessful")));
        }

        /// <summary>
        /// 通知设置
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> NotificationSetting()
        {
            var notificationSettings = await _notificationAppService.GetNotificationSettings();
            return View(notificationSettings);
        }

        /// <summary>
        /// 更新通知订阅
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpdateNotificationSetting([FromBody]UpdateNotificationSettingsInput input)
        {
            await _notificationAppService.UpdateNotificationSettings(input);
            return Json(new ResponseParamViewModel("UpdateNotificationSettingsSuccessful"));
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult UserProfile()
        {
            return View();
        }

        public async Task<FileResult> GetProfilePicture()
        {
            var output = await _profileAppService.GetProfilePicture();
            if (output.ProfilePicture.IsNullOrEmpty())
            {
                return GetDefaultProfilePictureInternal();
            }

            return File(Convert.FromBase64String(output.ProfilePicture), MimeTypeNames.ImageJpeg);
        }

        public async Task<FileResult> GetProfilePictureByUser(EntityDto<long> input)
        {
            var output = await _profileAppService.GetProfilePictureByUser(input);
            if (output.ProfilePicture.IsNullOrEmpty())
            {
                return GetDefaultProfilePictureInternal();
            }

            return File(Convert.FromBase64String(output.ProfilePicture), MimeTypeNames.ImageJpeg);
        }

        public FileResult GetDefaultProfilePicture()
        {
            return GetDefaultProfilePictureInternal();
        }

        protected FileResult GetDefaultProfilePictureInternal()
        {
            return File(Path.Combine("images", "avatar.jpg"), MimeTypeNames.ImagePng);
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadProfilePicture()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UploadProfilePicture([FromForm]FileDto input)
        {
            var profilePictureFile = Request.Form.Files.First();

            //Check input
            if (profilePictureFile == null)
            {
                throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
            }

            if (profilePictureFile.Length > MaxProfilePictureSize)
            {
                throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit", AbpTemplateApplicationConsts.MaxProfilPictureBytesUserFriendlyValue));
            }

            byte[] fileBytes;
            using (var stream = profilePictureFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            if (!ImageFormatExtension.GetRawImageFormat(fileBytes).IsIn(ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif))
            {
                throw new Exception(L("IncorrectImageFormat"));
            }

            await _profileAppService.UpdateProfilePicture(new UpdateProfilePictureInput()
            {
                ImageBytes = fileBytes
            });

            return Json(new ResponseParamViewModel("UpdateProfilePictureSuccessful"));
        }
    }
}
