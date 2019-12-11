using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace AS.OCR.Commom
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

        public static string GetConfig(string key)
        {
            try
            {
                return configuration.GetSection(key).Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"【key:{key}】 | 【ex:{ex.Message}】");
            }
        }


        public static string ConnectionStringSqlServer => GetConfig("ConnectionString:SqlServer");





    }
}
