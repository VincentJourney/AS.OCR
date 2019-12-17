using AS.OCR.Commom.Util;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AS.OCR.Dapper.Base.DapperHelper
{
    public abstract class AbstractDapperHelper
    {

        /// <summary>
        /// 
        /// </summary>
        public AbstractDapperHelper(string ConnectionString)
        {
            _connection = ConnectionString;
        }

        /// 数据库连接名
        public string _connection = "";

        /// 获取连接名        
        public string Connection { get => _connection; }

        /// 返回连接实例        
        public IDbConnection dbConnection = null;

        /// 静态变量保存类的实例        
        //public AbstractDapperHelper uniqueInstance;

        /// 定义一个标识确保线程同步        
        //private readonly object locker = new object();

        /// <summary>
        /// 获取实例，这里为单例模式，保证只存在一个实例
        /// </summary>
        /// <returns></returns>
        //public abstract AbstractDapperHelper GetInstance();
        //{
        //    // 双重锁定实现单例模式，在外层加个判空条件主要是为了减少加锁、释放锁的不必要的损耗
        //    if (uniqueInstance == null)
        //    {
        //        lock (locker)
        //        {
        //            if (uniqueInstance == null)
        //            {
        //                uniqueInstance = new AbstractDapperHelper();
        //            }
        //        }
        //    }
        //    return uniqueInstance;
        //}


        /// <summary>
        /// 创建数据库连接对象并打开链接
        /// </summary>
        /// <returns></returns>
        public IDbConnection OpenCurrentDbConnection()
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
