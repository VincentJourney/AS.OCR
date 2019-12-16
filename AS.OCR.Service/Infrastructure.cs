using AS.OCR.Commom.Attributes;
using AS.OCR.Commom.Util;
using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AS.OCR.Model.Entity;

namespace AS.OCR.Service
{
    public class Infrastructure
    {
        private static CacheHelper cacheHelper = new CacheHelper();
        /// <summary>
        /// 缓存锁  避免并发设置缓存增加DB压力
        /// </summary>
        private static readonly object cacheLocker = new object();


        public static DateTime DefaultDateTime = DateTime.Parse("1900-01-01");

        /// <summary>
        /// 根据传入T与sqlwhere获取结果，转化为Tretrun
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="Tretrun">返回类型</typeparam>
        /// <param name="Key">缓存键</param>
        /// <param name="Hour">缓存绝对时间</param>
        /// <param name="sqlWhere">sql where</param>
        /// <returns></returns>
        public static Tretrun GetCacheFromEntity<T, Tretrun>(string Key, double Hour, string sqlWhere = "")
            where T : AbstractEntity where Tretrun : class
        {
            Tretrun Value = default;
            if (cacheHelper.Exists(Key)) Value = cacheHelper.Get<T>(Key) as Tretrun;
            else
            {
                lock (cacheLocker)//避免缓存并发
                {
                    if (!cacheHelper.Exists(Key))
                    {
                        AS.OCR.Dapper.Base.Infrastructure<T> infrastructure = new Dapper.Base.Infrastructure<T>();
                        if (typeof(Tretrun).IsGenericType)
                            Value = infrastructure.GetList(sqlWhere) as Tretrun;
                        else
                            Value = infrastructure.GetModel(sqlWhere) as Tretrun;
                    }
                }
                if (Hour != 0) //有效时长设置为0时 不设置缓存
                    if (Value != null) cacheHelper.Set(Key, Value, TimeSpan.FromHours(Hour));
            }
            return Value;
        }

        /// <summary>
        /// 根据委托取数据
        /// </summary>
        /// <typeparam name="P">委托参数类型</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="Key">缓存键</param>
        /// <param name="Hour">缓存绝对时间</param>
        /// <param name="func">委托</param>
        /// <param name="param">委托参数</param>
        /// <returns></returns>
        public static T GetCacheData<P, T>(string Key, double Hour, Func<P, T> func, P param) where T : class
        {
            T Value = default;
            if (cacheHelper.Exists(Key))
                Value = cacheHelper.Get<T>(Key);
            else
                Value = func(param);
            if (Hour != 0) //有效时长设置为0时 不设置缓存
                if (Value != null)
                    cacheHelper.Set(Key, Value, TimeSpan.FromHours(Hour));
            return Value;
        }


        protected static void TError(string mes) => throw new Exception(mes);

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        public Result ImageUpload(string base64Str)
        {
            ImageRequest request = new ImageRequest()
            {
                fileName = $"{Guid.NewGuid().ToString()}.jpg",
                base64Str = base64Str,
                sourceSystem = "crm-ocr",
                fileDescription = "资源上传图片"
            };
            var result = HttpHelper.HttpPost(ConfigurationUtil.FileUploadUrl, JsonConvert.SerializeObject(request));
            return new Result(true, "", result);
        }

        /// <summary>
        /// 对比同一类型Model的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldModel"></param>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public static Result CompareModel<T>(T oldModel, T newModel) where T : class
        {
            var oldInfo = oldModel.GetType().GetProperties();
            var newInfo = newModel.GetType().GetProperties();

            if (oldInfo.Count() == 0 || newInfo.Count() == 0)
                return new Result(false, "校验失败：Entity Data Error");

            foreach (var item in oldInfo)
            {
                var attr = (ValidateAttr)item.GetCustomAttribute(typeof(ValidateAttr));
                if (attr != null && attr.IgnoreKey == item.Name)
                    continue;
                var newinfoObj = newInfo.Where(s => s.Name == item.Name).FirstOrDefault();
                if (newinfoObj == null && !item.GetValue(oldModel, null).ToString().Contains(newinfoObj.GetValue(newModel, null).ToString()))
                    return new Result(false, "校验失败：OCR数据与提交数据不一致");
            }
            return new Result(true);
        }
    }
}
