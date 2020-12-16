﻿namespace PearAdmin.AbpTemplate.Storage.Aliyun
{
    public class AliyunConfig
    {
        /// <summary>
        /// OSS的访问ID
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// OSS的访问密钥
        /// </summary>
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// OSS的访问地址
        /// </summary>
        public string Endpoint { get; set; }
    }
}
