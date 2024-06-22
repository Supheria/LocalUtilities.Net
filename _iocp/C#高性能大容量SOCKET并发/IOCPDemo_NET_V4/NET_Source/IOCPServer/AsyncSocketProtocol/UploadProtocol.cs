using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using IOCPServer;

namespace Net;

public class UploadProtocol(IocpServer asyncSocketServer, AsyncUserToken asyncSocketUserToken) : FileStreamProtocol(IocpProtocolTypes.Upload, asyncSocketServer, asyncSocketUserToken)
{
    enum Command
    {
        None = 0,
        Login = 1,
        Active = 2,
        Dir = 3,
        CreateDir = 4,
        DeleteDir = 5,
        FileList = 6,
        DeleteFile = 7,
        Upload = 8,
        WriteData = 9,
        Eof = 10,
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
                Command.CreateDir => DoCreateDir(),
                Command.DeleteDir => DoDeleteDir(),
                Command.FileList => DoFileList(),
                Command.DeleteFile => DoDeleteFile(),
                Command.Upload => DoUpload(),
                Command.WriteData => DoWriteData(buffer, offset, count),
                Command.Eof => DoEof(),
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
        else if (compare(ProtocolKey.CreateDir))
            return Command.CreateDir;
        else if (compare(ProtocolKey.DeleteDir))
            return Command.DeleteDir;
        else if (compare(ProtocolKey.FileList))
            return Command.FileList;
        else if (compare(ProtocolKey.DeleteFile))
            return Command.DeleteFile;
        else if (compare(ProtocolKey.Upload))
            return Command.Upload;
        else if (compare(ProtocolKey.WriteData))
            return Command.WriteData;
        else if (compare(ProtocolKey.Eof))
            return Command.Eof;
        else
            return Command.None;
        bool compare(string key)
        {
            return command.Equals(key, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    private bool CheckLogined(Command command)
    {
        if (command is Command.Login || command is Command.Active)
            return true;
        else
            return IsLogin;
    }

    private bool DoCreateDir()
    {
        if (!CheckDirName(ProtocolKey.ParentDir, out var parentDir))
            return false;
        if (!CommandParser.GetValueAsString(ProtocolKey.DirName, out var dirName))
            return CommandFail(ProtocolCode.ParameterError, "");
        try
        {
            parentDir = Path.Combine(parentDir, dirName);
            Directory.CreateDirectory(parentDir);
            return CommandSuccess([]);
        }
        catch (Exception ex)
        {
            return CommandFail(ProtocolCode.CreateDirError, ex.Message);
        }
    }

    private bool DoDeleteDir()
    {
        if (!CheckDirName(ProtocolKey.ParentDir, out var parentDir))
            return false;
        if (!CommandParser.GetValueAsString(ProtocolKey.DirName, out var dirName))
            return CommandFail(ProtocolCode.ParameterError, "");
        try
        {
            parentDir = Path.Combine(parentDir, dirName);
            Directory.Delete(parentDir, true);
            CommandComposer.AddSuccess();
            return CommandSuccess([]);
        }
        catch (Exception E)
        {
            return CommandFail(ProtocolCode.DeleteDirError, E.Message);
        }
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

    private bool DoDeleteFile()
    {
        if (!CheckDirName(ProtocolKey.DirName, out var dir))
            return false;
        try
        {
            var files = CommandParser.GetValue(ProtocolKey.Item);
            foreach(var file in files)
            {
                var fileName = Path.Combine(dir, file);
                File.Delete(fileName);
            }
            return CommandSuccess([]);
        }
        catch (Exception E)
        {
            return CommandFail(ProtocolCode.DeleteFileFailed, E.Message);
        }
    }

    private bool DoUpload()
    {
        if (!CheckDirName(ProtocolKey.DirName, out var dir))
            return false;
        if (!CommandParser.GetValueAsString(ProtocolKey.FileName, out var fileName))
            return CommandFail(ProtocolCode.ParameterError, "");
        //Program.Logger.Info("Start upload file: " + fileName);
        FilePath = Path.Combine(dir, fileName);
        FileStream?.Close();
        if (!File.Exists(FilePath))
            return CommandFail(ProtocolCode.FileNotExist, "");
        if (CheckFileInUse(FilePath))
            return CommandFail(ProtocolCode.FileIsInUse, "");
        FileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        FileStream.Position = FileStream.Length; //文件移到末尾
        return CommandSuccess(new() { [ProtocolKey.FileSize] = [FileStream.Length] });
    }

    private bool DoWriteData(byte[] buffer, int offset, int count)
    {
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "", false);
        FileStream.Write(buffer, offset, count);
        return CommandSuccess([]);
        //return true;
        //CommandComposer.AddSuccess();
        //CommandComposer.AddValue(ProtocolKey.Count, count); //返回读取个数
        //return DoSendResult(); //接收数据不发回响应
    }

    private bool DoEof()
    {
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        //Program.Logger.Info("End upload file: " + FileName);
        FileStream.Close();
        FileStream = null;
        FilePath = "";
        return CommandSuccess([]);
    }
}
