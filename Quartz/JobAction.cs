namespace BJ.Quartz.Quartz
{
    public enum JobAction
    {
        新增 = 1,
        删除 = 2,
        修改 = 3,
        暂停 = 4,
        停止,
        开启,
        立即执行
    }
    public enum JobStatus
    {

        删除 = -1,
        停止 = 1,
        正常 = 0
    }
}
