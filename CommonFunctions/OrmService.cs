using System.Diagnostics;
using FreeSql;

namespace CommonFunctions
{
    public static class OrmService
    {
        public static IFreeSql Db = new FreeSqlBuilder()
            .UseConnectionString(Common.MySystemConfig.DbType, Common.MySystemConfig.Db)
            // .UseConnectionString(DataType.MySql,"Data Source=192.168.2.35;Port=3306;User ID=root;Password=cdtnb...; Initial Catalog=srswebapi;Charset=utf8; SslMode=none;Min pool size=1")
            .UseMonitorCommand(cmd => Trace.WriteLine($"线程：{cmd.CommandText}\r\n"))
            .UseAutoSyncStructure(true) //自动创建、迁移实体表结构
            .UseNoneCommandParameter(true)
            .Build();
    }
}