﻿using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Dapper.Base
{
    public static class DBContext
    {
        // 获取开启数据库的连接
        public static IDbConnection Db
        {
            get
            {
                DapperHelper.GetInstance();
                return DapperHelper.OpenCurrentDbConnection();
            }
        }


        /// <summary>
        /// 查出一条记录的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T QueryFirstOrDefault<T>(string sql, object param = null)
        {
            return Db.QueryFirstOrDefault<T>(sql, param);
        }

        public static Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
        {
            return Db.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        /// <summary>
        /// 查出多条记录的实体泛型集合
        /// </summary>
        /// <typeparam name="T">泛型T</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Db.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
        }

        public static Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Db.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public static int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Db.Execute(sql, param, transaction, commandTimeout, commandType);
        }

        public static Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Db.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public static T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Db.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public static Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Db.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// 同时查询多张表数据（高级查询）
        /// "select *from K_City;select *from K_Area";
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlMapper.GridReader QueryMultiple(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Db.QueryMultiple(sql, param, transaction, commandTimeout, commandType);
        }
        public static Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Db.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// 增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static long Add<T>(T model) where T : class
        {
            return Db.Insert(model);
        }
        /// <summary>
        /// 删
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Delete<T>(T model) where T : class
        {
            return Db.Delete(model);
        }
        /// <summary>
        /// 改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Update<T>(T model) where T : class
        {
            return Db.Update(model);
        }

        /// <summary>
        /// Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public static T GetModel<T>(string whereStr = "")
        {
            var sqlCondition = string.IsNullOrWhiteSpace(whereStr) ? "" : $" where {whereStr}";
            return DBContext.Query<T>($@"SELECT * FROM {typeof(T).Name} {sqlCondition}").FirstOrDefault();
        }


        /// <summary>
        /// List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(string whereStr = "")
        {
            var sqlCondition = string.IsNullOrWhiteSpace(whereStr) ? "" : $" where {whereStr}";
            return DBContext.Query<T>($@"SELECT * FROM {typeof(T).Name} {sqlCondition}").ToList();
        }

    

    }
}

