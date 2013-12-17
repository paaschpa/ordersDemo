using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Redis;

namespace OrdersDemo.ServiceInterface
{
    public abstract class OrdersDemoServiceBase : Service
    {
        public IDbConnectionFactory DbConnectionFactory { get; set; }
        public IRedisClientsManager RedisClientManager { get; set; }

        protected virtual T DbConnExec<T>(Func<IDbConnection, T> dbConnFn)
        {
            using (var dbCon = DbConnectionFactory.OpenDbConnection())
            {
                return dbConnFn(dbCon);
            }
        }

        protected virtual void DbConnExec(Action<IDbConnection> dbConnFn)
        {
            using (var dbCon = DbConnectionFactory.OpenDbConnection())
            {
                dbConnFn(dbCon);
            }
        }

        protected virtual void DbConnExecTransaction(Action<IDbConnection> dbConnFn)
        {
            using (var dbCon = DbConnectionFactory.OpenDbConnection())
            {
                using (var trans = dbCon.OpenTransaction())
                {
                    try
                    {
                        dbConnFn(dbCon);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        protected virtual T RedisExec<T>(Func<IRedisClient, T> redisFn)
        {
            using (var redisClient = RedisClientManager.GetClient())
            {
                return redisFn(redisClient);
            }
        }
    }
}
