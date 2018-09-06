using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common
{
    /// <summary>
    /// 表示九宫格方位
    /// </summary>
    public enum GridPosition
    {
        /// <summary>
        /// 左上角
        /// </summary>
        NW,
        /// <summary>
        /// 正上方
        /// </summary>
        North,
        /// <summary>
        /// 右上角
        /// </summary>
        NE,
        /// <summary>
        /// 正左方
        /// </summary>
        West,
        /// <summary>
        /// 正中心
        /// </summary>
        Center,
        /// <summary>
        /// 正右方
        /// </summary>
        East,
        /// <summary>
        /// 左下角
        /// </summary>
        SW,
        /// <summary>
        /// 正下方
        /// </summary>
        South,
        /// <summary>
        /// 右下方
        /// </summary>
        SE
    }
}
