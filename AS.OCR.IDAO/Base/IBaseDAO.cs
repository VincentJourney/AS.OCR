using System;
using System.Collections.Generic;

namespace AS.OCR.IDAO.Base
{
    public interface IBaseDAO<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        bool Add(T Entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        bool Delete(T Entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        bool Update(T Entity);
        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        T Get(Guid Id);
        /// <summary>
        /// 根据Id获取实体列表
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        List<T> GetList(Guid Id);

        #region 不开放给Service层使用
        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        //T GetModel(string whereStr = "");
        ///// <summary>
        /////  根据条件获取实体列表
        ///// </summary>
        ///// <param name="whereStr"></param>
        ///// <returns></returns>
        //List<T> GetList(string whereStr = "");
        ///// <summary>
        ///// 通过sql获取业务实体
        ///// </summary>
        ///// <typeparam name="S">返回类型</typeparam>
        ///// <param name="sql"></param>
        ///// <returns></returns>
        //S GetModelFromSql<S>(string sql);
        ///// <summary>
        ///// 通过sql获取业务实体列表
        ///// </summary>
        ///// <typeparam name="S">返回类型</typeparam>
        ///// <param name="sql"></param>
        ///// <returns></returns>
        //List<S> GetListFromSql<S>(string sql);
        #endregion

    }
}
