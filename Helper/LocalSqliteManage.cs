using Microsoft.Data.Sqlite;
using SqlSugar;

namespace BJ.Quartz
{
    public class LocalSqliteManage
    {
        private static SqlSugarClient _localdb = null;
        /// <summary>
        /// 本地数据库
        /// </summary>
        /// <returns></returns>
        public static string SqliteDBPath()
        {
            string dbName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"db\BJQuartz.db");
            return dbName;
        }

        public static SqlSugarClient SqliteDB
        {
            get
            {
                //  if (_localdb == null)
                {
                    string dbName = SqliteDBPath();
                    string connStr = new SqliteConnectionStringBuilder()
                    {
                        DataSource = dbName,
                        Mode = SqliteOpenMode.ReadWriteCreate
                    }.ToString();
                    _localdb = DbContext.TD(connStr, DbType.Sqlite);
                }
                return _localdb;
            }
        }

        

    }
}
