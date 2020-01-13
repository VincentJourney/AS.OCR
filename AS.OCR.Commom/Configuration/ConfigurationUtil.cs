using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace AS.OCR.Commom.Configuration
{
    public static class ConfigurationUtil
    {
        static IConfiguration configuration;
        static ConfigurationUtil()
        {
            configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                .Build();
        }

        public static T GetConfig<T>(string key)
        {
            try
            {
                return configuration.GetValue<T>(key);
            }
            catch (Exception ex)
            {
                throw new Exception($"【key:{key}】 | 【ex:{ex.Message}】");
            }
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public static string ConnectionString => GetConfig<string>("ConnectionString");

        #region 腾讯OCR服务
        public static string TencentSecretId => GetConfig<string>("TencentConfig:SecretId");
        public static string TencentSecretKey => GetConfig<string>("TencentConfig:SecretKey");
        #endregion

        #region WebPos
        public static string WebPosUserName => GetConfig<string>("WebPosService:UserName");
        public static string WebPosPassWord => GetConfig<string>("WebPosService:PassWord");
        #endregion

        /// <summary>
        /// 文件服务
        /// </summary>
        public static string FileUploadUrl => GetConfig<string>("FileUploadUrl");

        #region ExceptionLess
        public static bool Exceptionless_Enabled => GetConfig<bool>("Exceptionless:Enabled");
        public static string Exceptionless_ApiKey => GetConfig<string>("Exceptionless:ApiKey");
        public static string Exceptionless_ServerUrl => GetConfig<string>("Exceptionless:ServerUrl");
        #endregion

        #region Redis配置项
        public static string RedisAddress => GetConfig<string>("Redis:RedisAddress");
        public static string RedisKey => GetConfig<string>("Redis:RedisKey");
        public static string RedisKeySet_OCRVerifyRuleList => "RedisKeySet_OCRVerifyRuleList";
        public static double RedisKeySet_OCRVerifyRuleList_expir => GetConfig<double>("Redis:RedisKeySet:OCRVerifyRuleList_expir");
        public static string RedisKeySet_Store => "RedisKeySet_Store";
        public static double RedisKeySet_Store_expir => GetConfig<double>("Redis:RedisKeySet:Store_expir");
        public static string RedisKeySet_Mall => "RedisKeySet_Mall";
        public static double RedisKeySet_Mall_expir => GetConfig<double>("Redis:RedisKeySet:Mall_expir");
        #endregion

        #region 企业微信配置项
        public static string EnterpriseWeChat_AppID => GetConfig<string>("EnterpriseWeChat:AppID");
        public static string EnterpriseWeChat_AppSecret => GetConfig<string>("EnterpriseWeChat:AppSecret");
        public static string EnterpriseWeChat_Touser => GetConfig<string>("EnterpriseWeChat:Touser");
        public static int EnterpriseWeChat_AgentId => GetConfig<int>("EnterpriseWeChat:AgentId");
        #endregion
        /// <summary>
        /// JWT token key
        /// </summary>
        public static string TokenKey { get => "2c85758f40ff400ab26e0616f6398c37"; }
    }
}
