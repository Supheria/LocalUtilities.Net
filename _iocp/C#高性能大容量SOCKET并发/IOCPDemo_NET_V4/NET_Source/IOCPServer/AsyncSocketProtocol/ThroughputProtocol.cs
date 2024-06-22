using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Net;

class ThroughputProtocol(IocpServer asyncSocketServer, AsyncUserToken asyncSocketUserToken) : IocpProtocol(IocpProtocolTypes.Throughput, asyncSocketServer, asyncSocketUserToken)
{
    enum Command
    {
        None = 0,
        CyclePack = 1,
    }
    //public override void Close()
    //{
    //    base.Close();
    //}

    public override bool ProcessCommand(byte[] buffer, int offset, int count) //处理分完包的数据，子类从这个方法继承
    {
        CommandComposer.Clear();
        CommandComposer.AddResponse();
        CommandComposer.AddCommand(CommandParser.Command);
        var command = StrToCommand(CommandParser.Command);
        try
        {
            return command switch
            {
                Command.CyclePack => DoCyclePack(buffer, offset, count),
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

    private Command StrToCommand(string command)
    {
        if (compare(ProtocolKey.CyclePack))
            return Command.CyclePack;
        else
            return Command.None;
        bool compare(string key)
        {
            return command.Equals(key, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public bool DoCyclePack(byte[] buffer, int offset, int count)
    {
        if (!CommandParser.GetValueAsInt(ProtocolKey.Count, out var cycleCount))
            return CommandFail(ProtocolCode.ParameterError, "");
        return CommandSuccess(buffer, offset, count, new() { [ProtocolKey.Count] = [(++cycleCount)] });
    }
}
