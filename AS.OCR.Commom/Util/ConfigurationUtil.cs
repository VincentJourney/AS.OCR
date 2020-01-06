using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace AS.OCR.Commom.Util
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

        #region MQ
        public static string RabbitMqHostName => GetConfig<string>("RabbitMq:HostName");
        public static string RabbitMqPort => GetConfig<string>("RabbitMq:Port");
        public static string RabbitMqUserName => GetConfig<string>("RabbitMq:UserName");
        public static string RabbitMqPassword => GetConfig<string>("RabbitMq:Password");
        #endregion

        #region 缓存有效时间
        //public static string CacheExpiration => GetConfig("CacheExpiration:AllMallOCRRule");
        public static double CacheExpiration_StoreOCRDetailRuleList => GetConfig<double>("CacheExpiration:AllStoreOCRDetailRuleList");
        public static double CacheExpiration_StoreOCR => GetConfig<double>("CacheExpiration:StoreOCR");
        public static double CacheExpiration_Store => GetConfig<double>("CacheExpiration:Store");
        public static double CacheExpiration_Company => GetConfig<double>("CacheExpiration:Company");
        public static double CacheExpiration_OrgInfo => GetConfig<double>("CacheExpiration:OrgInfo");
        public static double CacheExpiration_Mall => GetConfig<double>("CacheExpiration:Mall");
        #endregion

        #region ExceptionLess
        public static bool Exceptionless_Enabled => GetConfig<bool>("Exceptionless:Enabled");
        public static string Exceptionless_ApiKey => GetConfig<string>("Exceptionless:ApiKey");
        public static string Exceptionless_ServerUrl => GetConfig<string>("Exceptionless:ServerUrl");
        #endregion

        #region Redis配置项
        public static string RedisAddress => GetConfig<string>("Redis:RedisAddress");
        public static string RedisKey => GetConfig<string>("Redis:RedisKey");
        #endregion

        #region 企业微信配置项
        public static string EnterpriseWeChat_AppID => GetConfig<string>("EnterpriseWeChat:AppID");
        public static string EnterpriseWeChat_AppSecret => GetConfig<string>("EnterpriseWeChat:AppSecret");
        public static string EnterpriseWeChat_Touser => GetConfig<string>("EnterpriseWeChat:Touser");
        public static int EnterpriseWeChat_AgentId => GetConfig<int>("EnterpriseWeChat:AgentId");
        #endregion

        public static string TencentOCR_KingKey_HashId => GetConfig<string>("TencentOCR_KingKey:HashId");
        public static string TencentOCR_KingKey_RedisKey => GetConfig<string>("TencentOCR_KingKey:RedisKey");
        /// <summary>
        /// 京基购买总次数
        /// </summary>
        public static int TencentOCR_KingKey_Total => GetConfig<int>("TencentOCR_KingKey:Total");
        /// <summary>
        /// 京基剩余N次数警告
        /// </summary>
        public static int TencentOCR_KingKey_Warning => GetConfig<int>("TencentOCR_KingKey:Warning");
        /// <summary>
        /// 京基警告评率（警告线下N次提醒一次）
        /// </summary>
        public static int TencentOCR_KingKey_WarningHz => GetConfig<int>("TencentOCR_KingKey:WarningHz");
        /// <summary>
        /// JWT token key
        /// </summary>
        public static string TokenKey { get => "2c85758f40ff400ab26e0616f6398c37"; }





    }
}
