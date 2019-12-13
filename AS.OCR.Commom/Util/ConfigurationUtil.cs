using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace AS.OCR.Commom.Util
{
    public static class ConfigurationUtil
    {
        static IConfigurationRoot configuration;
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
                return (T)(configuration.GetSection(key).Value as object);
            }
            catch (Exception ex)
            {
                throw new Exception($"【key:{key}】 | 【ex:{ex.Message}】");
            }
        }


        public static string ConnectionStringSqlServer => GetConfig<string>("ConnectionString");
        public static string TencentSecretId => GetConfig<string>("TencentConfig:SecretId");
        public static string TencentSecretKey => GetConfig<string>("TencentConfig:SecretKey");
        public static string WebPosUserName => GetConfig<string>("WebPosService:UserName");
        public static string WebPosPassWord => GetConfig<string>("WebPosService:PassWord");
        public static string FileUploadUrl => GetConfig<string>("FileUploadUrl");

        public static string RabbitMqHostName => GetConfig<string>("RabbitMq:HostName");
        public static string RabbitMqPort => GetConfig<string>("RabbitMq:Port");
        public static string RabbitMqUserName => GetConfig<string>("RabbitMq:UserName");
        public static string RabbitMqPassword => GetConfig<string>("RabbitMq:Password");

        //public static string CacheExpiration => GetConfig("CacheExpiration:AllMallOCRRule");
        public static double CacheExpiration_StoreOCRDetailRuleList => GetConfig<double>("CacheExpiration:AllStoreOCRDetailRuleList");
        public static double CacheExpiration_StoreOCR => GetConfig<double>("CacheExpiration:StoreOCR");
        public static double CacheExpiration_Store => GetConfig<double>("CacheExpiration:Store");
        public static double CacheExpiration_Company => GetConfig<double>("CacheExpiration:Company");
        public static double CacheExpiration_OrgInfo => GetConfig<double>("CacheExpiration:OrgInfo");
        public static double CacheExpiration_Mall => GetConfig<double>("CacheExpiration:Mall");

        public static bool Exceptionless_Enabled => GetConfig<bool>("Exceptionless:Enabled");
        public static string Exceptionless_ApiKey => GetConfig<string>("Exceptionless:ApiKey");
        public static string Exceptionless_ServerUrl => GetConfig<string>("Exceptionless:ServerUrl");








    }
}
