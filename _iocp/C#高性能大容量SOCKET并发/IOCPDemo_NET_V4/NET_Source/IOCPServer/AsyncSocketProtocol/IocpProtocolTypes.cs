using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net;

public enum IocpProtocolTypes
{
    None = 0,
    /// <summary>
    /// SQL查询协议
    /// </summary>
    SQL = 1,
    /// <summary>
    /// 上传协议
    /// </summary>
    Upload = 2,
    /// <summary>
    /// 下载协议
    /// </summary>
    Download = 3,
    /// <summary>
    /// 远程文件流协议
    /// </summary>
    RemoteStream = 4,
    /// <summary>
    /// 吞吐量测试协议
    /// </summary>
    Throughput = 5,
    Control = 8,
    LogOutput = 9,
}
