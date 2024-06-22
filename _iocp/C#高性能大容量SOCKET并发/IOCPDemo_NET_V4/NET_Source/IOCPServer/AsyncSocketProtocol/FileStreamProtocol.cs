using IOCPServer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net;

public abstract class FileStreamProtocol(IocpProtocolTypes type, IocpServer server, AsyncUserToken userToken) : IocpProtocol(type, server, userToken)
{
    public string RootDirectory { get; set; } = "Files";

    public string FilePath { get; protected set; } = "";

    protected FileStream? FileStream { get; set; } = null;

    protected bool CheckDirName(string protocolKey, [NotNullWhen(true)] out string? dir)
    {
        if (!CommandParser.GetValueAsString(protocolKey, out dir))
            return CommandFail(ProtocolCode.ParameterError, "");
        dir = dir is "" ? RootDirectory : Path.Combine(RootDirectory, dir);
        if (!Directory.Exists(dir))
            return CommandFail(ProtocolCode.DirNotExist, "");
        return true;
    }

    public bool DoDir()
    {
        if (!CheckDirName(ProtocolKey.ParentDir, out var dir))
            return false;
        var subDirectorys = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
        var separator = new char[Path.DirectorySeparatorChar];
        var values = new List<object>();
        foreach(var subDir in subDirectorys)
        {
            string[] directoryName = subDir.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            values.Add(directoryName[directoryName.Length - 1]);
        }
        return CommandSuccess(new() { [ProtocolKey.Item] = values });
    }

    /// <summary>
    /// 检测文件是否正在使用中，如果正在使用中则检测是否被上传协议占用，如果占用则关闭,真表示正在使用中，并没有关闭
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected bool CheckFileInUse(string fileName)
    {
        if (IsFileInUse(fileName))
        {
            bool result = true;
            lock (Server.UploadProtocolManager)
            {
                foreach (var protocol in Server.UploadProtocolManager)
                {
                    if (fileName.Equals(protocol.FilePath, StringComparison.CurrentCultureIgnoreCase))
                    {
                        lock (protocol.UserToken) //AsyncSocketUserToken有多个
                        {
                            Server.CloseClient(protocol.UserToken);
                        }
                        result = false;
                    }
                }
            }
            return result;
        }
        else
            return false;
    }

    private static bool IsFileInUse(string fileName)
    {
        try
        {
            using var _ = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch
        {
            return true;
        }
    }

    public override void Dispose()
    {
        FilePath = "";
        FileStream?.Close();
        FileStream = null;
        OnDispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose()
    {

    }
}
