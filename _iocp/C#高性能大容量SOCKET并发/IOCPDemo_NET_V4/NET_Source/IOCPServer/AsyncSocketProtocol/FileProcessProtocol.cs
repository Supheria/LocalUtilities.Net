using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using static Net.DownloadProtocol;

namespace Net;

public class FileProcessProtocol(IocpServer asyncSocketServer, AsyncUserToken asyncSocketUserToken) : FileStreamProtocol(IocpProtocolTypes.RemoteStream, asyncSocketServer, asyncSocketUserToken)
{
    enum Command
    {
        None = 0,
        FileExists = 1,
        OpenFile = 2,
        SetSize = 3,
        GetSize = 4,
        SetPosition = 5,
        GetPosition = 6,
        Read = 7,
        Write = 8,
        Seek = 9,
        CloseFile = 10,
    }

    byte[]? ReadBuffer { get; set; } = null;

    public override bool ProcessCommand(byte[] buffer, int offset, int count) //处理分完包的数据，子类从这个方法继承
    {
        CommandComposer.Clear();
        CommandComposer.AddResponse();
        CommandComposer.AddCommand(CommandParser.Command);
        Command command = StrToCommand(CommandParser.Command);
        try
        {
            return command switch
            {
                Command.FileExists => DoFileExists(),
                Command.OpenFile => DoOpenFile(),
                Command.SetSize => DoSetSize(),
                Command.GetSize => DoGetSize(),
                Command.SetPosition => DoSetPosition(),
                Command.Read => DoRead(),
                Command.Write => DoWrite(buffer, offset, count),
                Command.Seek => DoSeek(),
                Command.CloseFile => DoCloseFile(),
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
        if (compare(ProtocolKey.FileExists))
            return Command.FileExists;
        else if (compare(ProtocolKey.OpenFile))
            return Command.OpenFile;
        else if (compare(ProtocolKey.SetSize))
            return Command.SetSize;
        else if (compare(ProtocolKey.GetSize))
            return Command.GetSize;
        else if (compare(ProtocolKey.SetPosition))
            return Command.SetPosition;
        else if (compare(ProtocolKey.GetPosition))
            return Command.GetPosition;
        else if (compare(ProtocolKey.Read))
            return Command.Read;
        else if (compare(ProtocolKey.Write))
            return Command.Write;
        else if (compare(ProtocolKey.Seek))
            return Command.Seek;
        else if (compare(ProtocolKey.CloseFile))
            return Command.CloseFile;
        else
            return Command.None;
        bool compare(string key)
        {
            return command.Equals(key, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public bool DoFileExists()
    {
        if (!CommandParser.GetValueAsString(ProtocolKey.FileName, out var filename))
            return CommandFail(ProtocolCode.ParameterError, "");
        if (File.Exists(filename))
            return CommandSuccess([]);
        else
            return CommandFail(ProtocolCode.FileNotExist, "file not exists");
    }

    public bool DoOpenFile()
    {
        if (!CommandParser.GetValueAsString(ProtocolKey.FileName, out var filename) || !CommandParser.GetValueAsShort(ProtocolKey.Mode, out var fileAccess))
            return CommandFail(ProtocolCode.ParameterError, "");
        if (!File.Exists(filename))
        {
            FileStream = new(filename, FileMode.Create, FileAccess.ReadWrite);
            return CommandSuccess([]);
        }
        try
        {
            FileStream = new(filename, FileMode.Open, (FileAccess)fileAccess);
            return CommandSuccess([]);
        }
        catch
        {
            return CommandFail(ProtocolCode.ParameterError, "wrong file access mode");
        }
    }

    public bool DoSetSize()
    {
        if (!CommandParser.GetValueAsLong(ProtocolKey.Size, out var fileSize))
            return CommandFail(ProtocolCode.ParameterError, "");
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        FileStream.SetLength(fileSize);
        return CommandSuccess([]);
    }

    public bool DoGetSize()
    {
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        return CommandSuccess(new() { [ProtocolKey.Size] = [FileStream.Length] });
    }

    public bool DoSetPosition()
    {
        if (!CommandParser.GetValueAsLong(ProtocolKey.Position, out var position))
            return CommandFail(ProtocolCode.ParameterError, "");
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        FileStream.Position = position;
        return CommandSuccess([]);
    }

    public bool DoGetPosition()
    {
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        return CommandSuccess(new() { [ProtocolKey.Position] = [FileStream.Position] });
    }

    public bool DoRead()
    {
        if (!CommandParser.GetValueAsInt(ProtocolKey.Count, out var count))
            return CommandFail(ProtocolCode.ParameterError, "");
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        if (ReadBuffer is null)
            ReadBuffer = new byte[count];
        else if (ReadBuffer.Length < count) //避免多次申请内存
            ReadBuffer = new byte[count];
        count = FileStream.Read(ReadBuffer, 0, count);
        return CommandSuccess(ReadBuffer, 0, count, new() { [ProtocolKey.Count] = [count] });
    }

    public bool DoWrite(byte[] buffer, int offset, int count)
    {
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        FileStream.Write(buffer, offset, count);
        return CommandSuccess(new() { [ProtocolKey.Count] = [count] });
    }

    public bool DoSeek()
    {
        if (!CommandParser.GetValueAsLong(ProtocolKey.Offset, out var offset) || !CommandParser.GetValueAsInt(ProtocolKey.SeekOrigin, out var seekOrign))
            return CommandFail(ProtocolCode.ParameterError, "");
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        try
        {
            offset = FileStream.Seek(offset, (SeekOrigin)seekOrign);
            return CommandSuccess(new() { [ProtocolKey.Offset] = [offset] });
        }
        catch
        {
            return CommandFail(ProtocolCode.ParameterError, "wrong seek origin mode");
        }
    }

    public bool DoCloseFile()
    {
        if (FileStream is null)
            return CommandFail(ProtocolCode.NotOpenFile, "");
        FileStream.Close();
        FileStream = null;
        return CommandSuccess([]);
    }
}
