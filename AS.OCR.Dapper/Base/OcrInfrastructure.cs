using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using AS.OCR.Dapper.Base.DBContext;
using Dapper.Contrib.Extensions;

namespace AS.OCR.Dapper.Base
{
    /// <summary>
    /// 数据访问层 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OcrInfrastructure<T> : BaseInterFace<T> where T : class
    {
        /// <summary>
        /// 增
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public bool Add(T Entity) => OcrDBContext.Add(Entity) == 0 ? true : false;
        /// <summary>
        /// 删
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public bool Delete(T Entity) => OcrDBContext.Delete(Entity);
        /// <summary>
        /// 改
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public bool Update(T Entity) => OcrDBContext.Update(Entity);

        /// <summary>
        /// 通过主键获取实体
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public T Get(Guid Id) => GetModel($"{GetKey()} = '{Id}'");
        /// <summary>
        /// 通过主键获取实体列表
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<T> GetList(Guid Id) => GetList($"{GetKey()} = '{Id}'");

        public string GetKey()
        {
            var properties = typeof(T).GetProperties();
            var keyProperty = properties
                .Where(s => s.GetCustomAttributes(typeof(ExplicitKeyAttribute), false)
                                .FirstOrDefault()?.GetType() == typeof(ExplicitKeyAttribute)).FirstOrDefault();
            if (keyProperty == null)
                throw new Exception("未明确实体主键");
            return keyProperty.Name;
        }
        /// <summary>
        /// 获取当前子类实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public T GetModel(string whereStr = "")
        {
            var sqlCondition = string.IsNullOrWhiteSpace(whereStr) ? "" : $" where {whereStr}";
            return OcrDBContext.Query<T>($@"SELECT * FROM {typeof(T).Name} {sqlCondition}").FirstOrDefault();
        }
        /// <summary>
        /// 获取当前子类实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public List<T> GetList(string whereStr = "")
        {
            var sqlCondition = string.IsNullOrWhiteSpace(whereStr) ? "" : $" where {whereStr}";
            return OcrDBContext.Query<T>($@"SELECT * FROM {typeof(T).Name} {sqlCondition}").ToList();
        }
        /// <summary>
        /// 可通过业务实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public S GetModelFromSql<S>(string sql) => OcrDBContext.QueryFirstOrDefault<S>(sql);

        /// <summary>
        /// 可通过业务实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public List<S> GetListFromSql<S>(string sql) => OcrDBContext.Query<S>(sql).ToList();

    }
}
