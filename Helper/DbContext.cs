using SqlSugar;

namespace BJ.Quartz
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class DbContext
    {

        public SqlSugarScope Db;   //用来处理事务多表查询和复杂的操作
 
        public static SqlSugarClient TD(string conStr, DbType dbType)
        {

            var db= new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = conStr,
                DbType = dbType,
                IsAutoCloseConnection = true,
                //IsShardSameThread = true,
                InitKeyType = InitKeyType.Attribute,
               
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    // DataInfoCacheService = new RedisCache()
                },
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoRemoveDataCache = true
                },
                
            });
            db.Aop.OnLogExecuting = (sql, args) =>
            {
                Console.WriteLine("----------------------------------");
                Console.WriteLine(sql, args);
            };
            return db;
        }
        public DbContext()
        {
       
        }

        public DbSet<T> DbTable<T>() where T : class, new()
        {
            return new DbSet<T>(Db);
        }

        // public DbSet<sysdiagrams> sysdiagramsDb => new DbSet<sysdiagrams>(Db);


    }

    /// <summary>
    /// 扩展ORM
    /// </summary>
    public class DbSet<T> : SimpleClient<T> where T : class, new()
    {
        public DbSet(SqlSugarScope context) : base(context)
        {

        }
    }
}
