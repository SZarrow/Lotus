using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lotus.Scheduler.Web.Models
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// 未调度
        /// </summary>
        None,
        /// <summary>
        /// 运行中
        /// </summary>
        Running,
        /// <summary>
        /// 暂停中
        /// </summary>
        Pause,
        /// <summary>
        /// 恢复中
        /// </summary>
        Recovering,
        /// <summary>
        /// 已停止
        /// </summary>
        Stoped
    }
}
