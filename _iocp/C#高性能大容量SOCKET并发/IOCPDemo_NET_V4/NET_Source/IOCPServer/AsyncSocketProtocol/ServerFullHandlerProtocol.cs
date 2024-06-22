using Microsoft.VisualBasic.Devices;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Net;

public class ServerFullHandlerProtocol(IocpServer server, AsyncUserToken userToken) : IocpServerProtocol(IocpProtocolTypes.FullHandler, server, userToken)
{
    public enum Command
    {
        None = 0,
        Login = 1,
        Active = 2,
        Dir = 3,
        FileList = 4,
        Download = 5,
        WriteData = 6,
        HandleMessage = 7,
        Upload = 8,
        SendFile = 9
    }

    int PacketSize { get; set; } = 64 * 1024;

    byte[]? ReadBuffer { get; set; } = null;

    public string FilePath { get; private set; } = "";

    FileStream? FileStream { get; set; } = null;

    bool IsSendingFile { get; set; } = false;

    bool IsReceivingFile { get; set; } = false;

    int ReceviedLength { get; set; } = 0;

    long ReceivedFileSize { get; set; } = 0;

    public DirectoryInfo RootDirectory { get; set; } = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Files"));

    public string RootDirectoryPath => RootDirectory.FullName;

    public override void Dispose()
    {
        FilePath = "";
        FileStream?.Close();
        FileStream = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 发送消息到客户端，由消息来驱动业务逻辑，接收方必须返回应答，否则认为发送不成功
    /// </summary>
    /// <param name="message">消息</param>
    public void SendMessage(string message)
    {
        // TODO: logic here is strange, cause no judge for responsing of receiver
        CommandComposer.Clear();
        CommandComposer.AddResponse();
        CommandComposer.AddCommand(ProtocolKey.Message);
        CommandComposer.AddSuccess();
        var Buffer = Encoding.UTF8.GetBytes(message);
        SendBackResult(Buffer, 0, Buffer.Length);
    }

    /// <summary>
    /// 处理分完包的数据，子类从这个方法继承,服务端在此处处理所有的客户端命令请求，返回结果必须加入CommandComposer.AddResponse();
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public override bool ProcessCommand(byte[] buffer, int offset, int count)
    {
        CommandComposer.Clear();
        CommandComposer.AddResponse();
        CommandComposer.AddCommand(CommandParser.Command);
        var command = StrToCommand(CommandParser.Command);
        if (!CheckLogin(command)) //检测登录
            return CommandFail(ProtocolCode.UserHasLogined, "");
        try
        {
            return command switch
            {
                Command.Login => DoLogin(),
                Command.Active => DoActive(),
                Command.HandleMessage => DoHandleMessage(buffer, offset, count),
                Command.Dir => DoDir(),
                Command.FileList => DoFileList(),
                Command.Download => DoDownload(),
                Command.Upload => DoUpload(),
                Command.SendFile => DoSendFile(),
                Command.WriteData => DoWriteData(buffer, offset, count),
                _ => throw new IocpException("Unknow command: " + CommandParser.Command)
            };
        }
        catch (Exception ex)
        {
            return CommandFail(ProtocolCode.ParameterError, ex.Message);
            //ServerInstance.Logger.Error("Unknow command: " + CommandParser.Command);
            //return false;
        }
    }

    /// <summary>
    /// 关键代码
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Command StrToCommand(string command)
    {
        if (compare(ProtocolKey.Active))
            return Command.Active;
        else if (compare(ProtocolKey.Login))
            return Command.Login;
        else if (compare(ProtocolKey.Message))
            return Command.HandleMessage;
        else if (compare(ProtocolKey.Dir))
            return Command.Dir;
        else if (compare(ProtocolKey.FileList))
            return Command.FileList;
        else if (compare(ProtocolKey.Download))
            return Command.Download;
        else if (compare(ProtocolKey.Upload))
            return Command.Upload;
        else if (compare(ProtocolKey.SendFile))
            return Command.SendFile;
        else if (compare(ProtocolKey.Data))
            return Command.WriteData;
        else
            return Command.None;
        bool compare(string key)
        {
            return command.Equals(key, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public bool CheckLogin(Command command)
    {
        if (command is Command.Login || command is Command.Active)
            return true;
        else
            return IsLogin;
    }

    public bool DoSendFile()
    {
        return CommandSucceed([]);
    }

    public bool DoWriteData(byte[] buffer, int offset, int count)
    {
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "", false);
        FileStream.Write(buffer, offset, count);
        ReceviedLength += count;
        if (ReceviedLength == ReceivedFileSize)
        {
            FileStream.Close();
            FileStream.Dispose();
            ReceviedLength = 0;
            IsReceivingFile = false;
#if DEBUG
            MessageBox.Show($"文件接收成功，完成时间{DateTime.Now}");
#endif
        }
        CommandComposer.Clear();
        CommandComposer.AddResponse();
        CommandComposer.AddCommand(ProtocolKey.Data);
        return CommandSucceed([]);
        //CommandComposer.AddSuccess();
        ////CommandComposer.AddValue(ProtocolKey.FileSize, receive_fileSize - m_fileStream.Position);//将当前的文件流位置发给客户端
        //return SendBackResult();
    }

    /// <summary>
    /// 处理客户端文件上传
    /// </summary>
    /// <returns></returns>
    public bool DoUpload()
    {
        if (!CommandParser.GetValueAsString(ProtocolKey.DirName, out var dir) ||
            !CommandParser.GetValueAsString(ProtocolKey.FileName, out var filePath) ||
            !CommandParser.GetValueAsLong(ProtocolKey.FileSize, out var fileSize) ||
            !CommandParser.GetValueAsInt(ProtocolKey.PacketSize, out var packetSize))
            return CommandFail(ProtocolCode.ParameterError, "");
        // TODO: modified here for uniform
        //dir = dir is "" ? DefaultRootDirectory : dir;
        dir = dir is "" ? RootDirectoryPath : Path.Combine(RootDirectoryPath, dir);
        if (!Directory.Exists(dir))
            return CommandFail(ProtocolCode.DirNotExist, dir);
        //ServerInstance.Logger.Info("Start Receive file: " + filePath);
        FilePath = Path.Combine(dir, filePath);
        FileStream?.Close();
        ReceivedFileSize = fileSize;
        // 本地存在，则删除重建
        if (File.Exists(FilePath))
        {
            if (UserToken.Server.CheckFileInUse(FilePath))
            {
                //ServerInstance.Logger.Error("Start Receive file error, file is in use: " + filePath);
                return CommandFail(ProtocolCode.FileIsInUse, FilePath);
            }
            File.Delete(FilePath);
        }
        FileStream = new(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        IsReceivingFile = true;
        return CommandSucceed([]);
    }

    /// <summary>
    /// 处理客户端文件下载
    /// </summary>
    /// <returns></returns>
    public bool DoDownload()
    {
        if (!CommandParser.GetValueAsString(ProtocolKey.DirName, out var dir) ||
            !CommandParser.GetValueAsString(ProtocolKey.FileName, out var filePath) ||
            !CommandParser.GetValueAsLong(ProtocolKey.FileSize, out var fileSize) ||
            !CommandParser.GetValueAsInt(ProtocolKey.PacketSize, out var packetSize))
            return CommandFail(ProtocolCode.ParameterError, "");
        dir = dir is "" ? RootDirectoryPath : Path.Combine(RootDirectoryPath, dir);
        if (!Directory.Exists(dir))
            return CommandFail(ProtocolCode.DirNotExist, dir);
        //ServerInstance.Logger.Info("Start download file: " + filePath);
        FilePath = Path.Combine(dir, filePath);
        FileStream?.Close();
        IsSendingFile = false;
        if (File.Exists(FilePath))
        {
            FilePath = "";
            return CommandFail(ProtocolCode.FileNotExist, "");
        }
        if (UserToken.Server.CheckFileInUse(FilePath))
        {
            FilePath = "";
            //ServerInstance.Logger.Error("Start download file error, file is in use: " + filePath);
            return CommandFail(ProtocolCode.FileIsInUse, "");
        }
        FileStream = new(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read) // 文件以共享只读方式打开，方便多个客户端下载同一个文件。
        {
            Position = fileSize // 文件移到上次下载位置                        
        };
        IsSendingFile = true;
        PacketSize = packetSize;
        return CommandSucceed([]);
    }

    private bool DoHandleMessage(byte[] buffer, int offset, int count)
    {
        var message = Encoding.UTF8.GetString(buffer, offset, count);
        UserToken.Server.HandleReceiveMessage(message, this);
        return CommandSucceed([]);
    }

    public override bool DoLogin()
    {
        if (!CommandParser.GetValueAsString(ProtocolKey.UserID, out var userID) || 
            !CommandParser.GetValueAsString(ProtocolKey.Password, out var password))
            return CommandFail(ProtocolCode.ParameterError, "");
        var success = userID is "admin" && password is "password";
        if (!success)
        {
            //ServerInstance.Logger.ErrorFormat("{0} login failure,password error", userID);
            return CommandFail(ProtocolCode.UserOrPasswordError, "");
        }
        UserID = "admin";
        UserName = "admin";
        IsLogin = true;
        //ServerInstance.Logger.InfoFormat("{0} login success", userID);
        return CommandSucceed(new()
        {
            [ProtocolKey.UserID] = [UserID],
            [ProtocolKey.UserName] = [UserName]
        });
    }
    
    public bool DoDir()
    {
        if (!CommandParser.GetValueAsString(ProtocolKey.ParentDir, out var dir))
            return CommandFail(ProtocolCode.ParameterError, $"{dir} is not exist");
        dir = dir is "" ? RootDirectoryPath : Path.Combine(RootDirectoryPath, dir);
        if (!Directory.Exists(dir))
            return CommandFail(ProtocolCode.DirNotExist, dir);
        var subDirectorys = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
        char[] separator = [Path.DirectorySeparatorChar];
        var values = new List<object>();
        foreach(var subDir in  subDirectorys)
        {
            string[] directoryName = subDir.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            values.Add(directoryName[directoryName.Length - 1]);
        }
        return CommandSucceed(new() { [ProtocolKey.Item] = values });
    }

    public bool DoFileList()
    {
        if (!CommandParser.GetValueAsString(ProtocolKey.DirName, out var dir))
            return CommandFail(ProtocolCode.ParameterError, "");
        dir = dir is "" ? RootDirectoryPath : Path.Combine(RootDirectoryPath, dir);
        if (!Directory.Exists(dir))
            return CommandFail(ProtocolCode.DirNotExist, dir);
        try
        {
            var values = new List<object>();
            foreach (var file in Directory.GetFiles(dir))
            {
                var fileInfo = new FileInfo(file);
                values.Add($"{fileInfo.Name}{ProtocolKey.TextSeperator}{fileInfo.Length}");
            }
            return CommandSucceed(new() { [ProtocolKey.Item] = values });
        }
        catch(Exception ex)
        {
            return CommandFail(ProtocolCode.UnknowError, ex.Message);
        }
    }

    // TODO: override ProcessSend
    public override void ProcessSend()
    {
        ActiveTime = DateTime.UtcNow;
        IsSendingAsync = false;
        var sendBuffer = UserToken.SendBuffer;
        sendBuffer.ClearFirstPacket(); //清除已发送的包
        if (sendBuffer.GetFirstPacket(out var offset, out var count))
        {
            IsSendingAsync = true;
            UserToken.SendAsync(sendBuffer.DynamicBuffer.Buffer, offset, count);
            return;
        }
        if (FileStream is null)
            return;
        if (IsSendingFile) // 发送文件头
        {
            CommandComposer.Clear();
            CommandComposer.AddResponse();
            CommandComposer.AddCommand(ProtocolKey.SendFile);
            _ = CommandSucceed(new() { [ProtocolKey.FileSize] = [FileStream.Length - FileStream.Position] });
            IsSendingFile = false;
            return;
        }
        if (IsReceivingFile)
            return;
        // 没有接收文件时
        // 发送具体数据,加m_fileStream.CanSeek是防止上传文件结束后，文件流被释放而出错
        if (FileStream.CanSeek && FileStream.Position < FileStream.Length)
        {
            CommandComposer.Clear();
            CommandComposer.AddResponse();
            CommandComposer.AddCommand(ProtocolKey.Data);
            ReadBuffer ??= new byte[PacketSize];
            // 避免多次申请内存
            if (ReadBuffer.Length < PacketSize)
                ReadBuffer = new byte[PacketSize];
            count = FileStream.Read(ReadBuffer, 0, PacketSize);
            _ = CommandSucceed(ReadBuffer, 0, count, []);
            return;
        }
        // 发送完成
        //ServerInstance.Logger.Info("End Upload file: " + FilePath);
        FileStream.Close();
        FileStream = null;
        FilePath = "";
        IsSendingFile = false;
    }
}
