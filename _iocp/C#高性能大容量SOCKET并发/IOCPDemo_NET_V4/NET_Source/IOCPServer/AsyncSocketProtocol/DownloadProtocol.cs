using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace Net;

public class DownloadProtocol(IocpServer asyncSocketServer, AsyncUserToken asyncSocketUserToken) : FileStreamProtocol(IocpProtocolTypes.Download, asyncSocketServer, asyncSocketUserToken)
{
    public enum Command
    {
        None = 0,
        Login = 1,
        Active = 2,
        Dir = 3,
        FileList = 4,
        Download = 5,
    }

    bool IsCompleted { get; set; } = false;

    int PackSize { get; set; } = 64 * 1024;

    byte[]? ReadBuffer { get; set; } = null;

    protected override void OnDispose()
    {
        IsCompleted = false;
    }

    public override bool ProcessCommand(byte[] buffer, int offset, int count) //处理分完包的数据，子类从这个方法继承
    {
        CommandComposer.Clear();
        CommandComposer.AddResponse();
        CommandComposer.AddCommand(CommandParser.Command);
        var command = StrToCommand(CommandParser.Command);
        if (!CheckLogined(command)) //检测登录
            return CommandFail(ProtocolCode.UserHasLogined, "");
        try
        {
            return command switch
            {
                Command.Login => DoLogin(),
                Command.Active => DoActive(),
                Command.Dir => DoDir(),
                Command.FileList => DoFileList(),
                Command.Download => DoDownload(),
                _ => throw new IocpException("Unknow command: " + CommandParser.Command)
            };
        }
        catch (Exception ex)
        {
            return CommandFail(ProtocolCode.ParameterError, ex.Message);
            //Program.Logger.Error("Unknow command: " + CommandParser.Command);
            //return false;
        }
    }

    private static Command StrToCommand(string command)
    {
        if (compare(ProtocolKey.Active))
            return Command.Active;
        else if (compare(ProtocolKey.Login))
            return Command.Login;
        else if (compare(ProtocolKey.Dir))
            return Command.Dir;
        else if (compare(ProtocolKey.FileList))
            return Command.FileList;
        else if (compare(ProtocolKey.Download))
            return Command.Download;
        else
            return Command.None;
        bool compare(string key)
        {
            return command.Equals(key, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    private bool CheckLogined(Command command)
    {
        if ((command == Command.Login) | (command == Command.Active))
            return true;
        else
            return IsLogin;
    }

    private bool DoFileList()
    {
        if (!CheckDirName(ProtocolKey.DirName, out var dir))
            return false;
        var files = Directory.GetFiles(dir);
        var values = new List<object>();
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            var fileSize = fileInfo.Length;
            values.Add(fileInfo.Name + ProtocolKey.TextSeperator + fileSize.ToString());
        }
        return CommandSuccess(new() { [ProtocolKey.Item] = values });
    }

    public bool DoDownload()
    {
        if (!CheckDirName(ProtocolKey.DirName, out var dir))
            return false;
        if (!CommandParser.GetValueAsString(ProtocolKey.FileName, out var fileName) ||
            !CommandParser.GetValueAsLong(ProtocolKey.FileSize, out var fileSize) ||
            !CommandParser.GetValueAsInt(ProtocolKey.PackSize, out var packSize))
            return CommandFail(ProtocolCode.ParameterError, "");
        //Program.Logger.Info("Start download file: " + fileName);
        FilePath = Path.Combine(dir, fileName);
        FileStream?.Close();
        IsCompleted = false;
        if (!File.Exists(FilePath))
        {
            FilePath = "";
            return CommandFail(ProtocolCode.FileNotExist, "");
        }
        if (CheckFileInUse(FilePath))
        {
            FilePath = "";
            return CommandFail(ProtocolCode.FileIsInUse, "");
        }
        FileStream = new(fileName, FileMode.Open, FileAccess.ReadWrite)
        {
            Position = fileSize //文件移到上次下载位置
        };
        IsCompleted = true;
        PackSize = packSize;
        return CommandSuccess([]);
    }

    public override void ProcessSend()
    {
        ActiveTime = DateTime.UtcNow;
        IsSendingAsync = false;
        var asyncSendBufferManager = UserToken.SendBuffer;
        asyncSendBufferManager.ClearFirstPack(); //清除已发送的包
        if (asyncSendBufferManager.GetFirstPack(out var offset, out var count))
        {
            IsSendingAsync = true;
            Server.SendAsync(UserToken, asyncSendBufferManager.DynamicBuffer.Buffer, offset, count);
            return;
        }
        if (FileStream is null)
            return;
        // 发送文件头
        if (IsCompleted)
        {
            CommandComposer.Clear();
            CommandComposer.AddResponse();
            CommandComposer.AddCommand(ProtocolKey.SendFile);
            CommandComposer.AddSuccess();
            CommandComposer.AddValue(ProtocolKey.FileSize, FileStream.Length - FileStream.Position);
            SendBackResult();
            IsCompleted = false;
            return;
        }
        // 发送具体数据
        if (FileStream.Position < FileStream.Length)
        {
            CommandComposer.Clear();
            CommandComposer.AddResponse();
            CommandComposer.AddCommand(ProtocolKey.WriteData);
            CommandComposer.AddSuccess();
            if (ReadBuffer == null)
                ReadBuffer = new byte[PackSize];
            else if (ReadBuffer.Length < PackSize) //避免多次申请内存
                ReadBuffer = new byte[PackSize];
            count = FileStream.Read(ReadBuffer, 0, PackSize);
            SendBackResult(ReadBuffer, 0, count);
        }
        // 发送完成
        else
        {
            //Program.Logger.Info("End download file: " + FileName);
            FileStream.Close();
            FileStream = null;
            FilePath = "";
            IsCompleted = false;
        }
    }
}
