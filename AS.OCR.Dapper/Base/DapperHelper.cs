using AS.OCR.Commom.Util;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AS.OCR.Dapper.Base
{
    public class DapperHelper
    {
        /// 数据库连接名
        public static string _connection;
        /// 获取连接名        
        public static string Connection { get => _connection; }
        private DapperHelper()
        {
            _connection = ConfigurationUtil.ConnectionString;
        }

        /// 返回连接实例        
        public static IDbConnection dbConnection = null;

        // 静态变量保存类的实例
        public static DapperHelper uniqueInstance;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        // <summary>
        // 获取实例，这里为单例模式，保证只存在一个实例
        // </summary>
        // <returns></returns>
        public static DapperHelper GetInstance()
        {
            // 双重锁定实现单例模式，在外层加个判空条件主要是为了减少加锁、释放锁的不必要的损耗
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new DapperHelper();
                    }
                }
            }
            return uniqueInstance;
        }


        /// <summary>
        /// 创建数据库连接对象并打开链接
        /// </summary>
        /// <returns></returns>
        public static IDbConnection OpenCurrentDbConnection()
        {
            if (dbConnection == null)
                dbConnection = new SqlConnection(Connection);
            //判断连接状态
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();
            return dbConnection;
        }
    }
}
